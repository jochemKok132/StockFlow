using StockFlow.Application.Common;
using StockFlow.Application.DTOs.AuditLogs.CashRegister;
using StockFlow.Application.DTOs.AuditLogs.Customer;
using StockFlow.Application.DTOs.CashRegister;
using StockFlow.Application.DTOs.Enums;
using StockFlow.Application.Interfaces.CashRegister;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Mappings.AuditLogs;
using StockFlow.Application.Services.ManagementApp.AuditLogs;
using StockFlow.Domain.Entities.AuditLogs;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;
using StockFlow.Application.Validations.CashRegister;
using StockFlow.Application.Interfaces.Repositories.ManagementApp;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;

namespace StockFlow.Application.Services.CashRegister
{
    public class RegisterDataService(IProductService productService, ICashRegisterLogsService cashRegisterLogsService, ICustomerRepository customerRepository, ICustomerLogsService customerLogsService, ICashRegisterLogsRepository cashRegisterLogsRepository) : IRegisterDataService
    {
        public async Task<string> FinalizePurchase(Purchase purchase, Guid employeeId)
        {
            if (purchase == null) throw new ArgumentException("Purchase is required.");

            Customer? customer = null;
            if (purchase.CustomerId is Guid customerId)
            {
                customer = await customerRepository.GetByIdAsync(customerId);
                if (customer is not null && customer.SoftDeleted)
                    customer = null;
            }

            purchase.Validate(customer is not null, customer?.SavedPoints ?? 0);
            
            foreach(var stock in purchase.ProductsSold)
            {
                await productService.UpdateProductStockAsync(stock.Product.Id, -stock.Amount, employeeId);
            }
            decimal amountPaid = purchase.ProductsSold.Sum(l => l.PriceAtTimeOfSale * l.Amount) - LoyaltyPoints.ToEuros(purchase.PointsUsed);
            int pointsEarned = customer is null ? 0 : LoyaltyPoints.PointsEarned(amountPaid);

            var log = purchase.ToCashRegisterLogsDto(employeeId);
            log.PointsEarned = pointsEarned;
            string receiptId = await cashRegisterLogsService.CreateCashRegisterLogsAsync(log);

            if (customer is not null && (purchase.PointsUsed > 0 || pointsEarned > 0))
            {
                List<string> details = [$"Receipt: {receiptId}"];
                if (purchase.PointsUsed > 0) details.Add($"PointsUsed: {purchase.PointsUsed} (-€{LoyaltyPoints.ToEuros(purchase.PointsUsed):0.00})");
                if (pointsEarned > 0) details.Add($"PointsEarned: {pointsEarned}");

                await ChangeSavedPointsAsync(customer, pointsEarned - purchase.PointsUsed, details);
            }

            return receiptId;
        }

        private async Task ChangeSavedPointsAsync(Customer customer, int change, List<string> details)
        {
            int oldPoints = customer.SavedPoints;
            customer.SavedPoints = Math.Max(0, oldPoints + change);

            if (customer.SavedPoints != oldPoints)
                await customerRepository.UpdateAsync(customer);

            List<string> messages = customer.SavedPoints != oldPoints
                ? [$"SavedPoints: New: {customer.SavedPoints}. Old: {oldPoints}", .. details]
                : [$"SavedPoints: Unchanged ({oldPoints})", .. details];

            await customerLogsService.CreateCustomerLogsAsync(new CreateCustomerLogsDto
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                LogType = LogTypeDto.Update,
                Messages = messages,
            });
        }

        public async Task<ReturnResult> ProcessReturn(ReturnRequest request, Guid employeeId)
        {
            request.Validate();

            CashRegisterLogs? log = await cashRegisterLogsRepository.GetByReceiptIdAsync(request.ReceiptId, track: true);
            if (log is null) throw new KeyNotFoundException($"Receipt {request.ReceiptId} not found.");

            var items = request.Items
                .GroupBy(i => i.ProductId)
                .Select(g => new ReturnItem { ProductId = g.Key, Amount = g.Sum(i => i.Amount) })
                .ToList();

            foreach (var item in items)
            {
                var lines = log.ProductsSold.Where(l => l.ProductId == item.ProductId).ToList();
                if (lines.Count == 0)
                    throw new ArgumentException($"This product is not on receipt {log.ReceiptId}.");

                string name = lines[0].Product?.ProductName ?? "this product";
                int returnable = lines.Sum(l => l.Amount - l.ReturnedAmount);

                if (returnable <= 0)
                    throw new ArgumentException($"All of {name} on receipt {log.ReceiptId} has already been returned.");

                if (item.Amount > returnable)
                    throw new ArgumentException($"Only {returnable} of {name} can still be returned on receipt {log.ReceiptId}.");
            }

            decimal linesTotal = log.ProductsSold.Sum(l => l.PriceAtTimeOfSale * l.Amount);
            decimal totalPaid = (decimal)log.TotalPrice;
            decimal refund = 0m;

            foreach (var item in items)
            {
                int left = item.Amount;
                foreach (var line in log.ProductsSold.Where(l => l.ProductId == item.ProductId))
                {
                    int take = Math.Min(left, line.Amount - line.ReturnedAmount);
                    if (take <= 0) continue;

                    line.ReturnedAmount += take;
                    refund += ReturnRules.RefundFor(line.PriceAtTimeOfSale, take, totalPaid, linesTotal);
                    left -= take;
                    if (left == 0) break;
                }
            }

            decimal keptValue = log.ProductsSold.Sum(l => l.PriceAtTimeOfSale * (l.Amount - l.ReturnedAmount))
                * ReturnRules.PaidShare(totalPaid, linesTotal);
            int pointsStillEarned = Math.Min(log.PointsEarned, LoyaltyPoints.PointsEarned(keptValue));
            int pointsDeducted = log.PointsEarned - pointsStillEarned;
            log.PointsEarned = pointsStillEarned;

            await cashRegisterLogsRepository.SaveChangesAsync();

            foreach (var item in items)
                await productService.UpdateProductStockAsync(item.ProductId, item.Amount, employeeId, LogTypeDto.Returned);

            if (pointsDeducted > 0 && log.CustomerId is Guid customerId)
            {
                Customer? customer = await customerRepository.GetByIdAsync(customerId);
                if (customer is not null && !customer.SoftDeleted)
                    await ChangeSavedPointsAsync(customer, -pointsDeducted,
                    [
                        $"Return: Receipt {log.ReceiptId}",
                        $"PointsDeducted: {pointsDeducted}",
                    ]);
            }

            return new ReturnResult
            {
                ReceiptId = log.ReceiptId,
                ItemsReturned = items.Sum(i => i.Amount),
                RefundAmount = refund,
                PointsDeducted = pointsDeducted,
            };
        }
    }
}
