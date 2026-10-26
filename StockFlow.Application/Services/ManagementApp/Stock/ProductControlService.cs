using System.Globalization;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Repositories.Ordering;
using StockFlow.Application.Mappings.AuditLogs;
using StockFlow.Application.Mappings.Stock;
using StockFlow.Application.Validations.Stock;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Shared.DTOs.AuditLogs.Stock;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Stock.Product;

namespace StockFlow.Application.Services.ManagementApp.Stock
{
    public class ProductControlService(IProductRepository productRepository, IPurchaseOrderRepository purchaseOrderRepository, IStockLogsService stockLogsService, IProductService productService) : IProductControlService
    {
        public const string NotAFuturePrice = "Only a price that hasn't started yet can be cancelled.";

        public async Task<ProductDto> SchedulePriceAsync(SchedulePriceRequest request, Guid employeeId)
        {
            DateTime today = DateTime.Today;
            request.Validate(today);
            var product = await GetActiveProductAsync(request.ProductId);

            var price = new ProductPrice
            {
                            ProductId = product.Id,
                Price = request.Price,
                ValidFrom = request.ValidFrom.Date,
                CreatedAt = DateTime.UtcNow,
                CreatedById = employeeId,
            };
            product.Prices.Add(price);

            List<string> messages;
            if (price.ValidFrom <= today)
            {
                messages = [$"Price: New: {Money(request.Price)}. Old: {Money(product.Price)}"];
                product.Price = request.Price;
            }
            else
            {
                messages = [$"ScheduledPrice: {Money(request.Price)} from {price.ValidFrom:dd-MM-yyyy}"];
            }

            await productRepository.UpdateAsync(product);
            await LogAsync(product, employeeId, LogTypeDto.Update, messages);
            return await productService.GetProductDetailsAsync(product.Id);
        }

        public async Task<ProductDto> CancelScheduledPriceAsync(Guid productId, Guid priceId, Guid employeeId)
        {
            var product = await GetActiveProductAsync(productId);
            var price = product.Prices.FirstOrDefault(p => p.Id == priceId)
                ?? throw new KeyNotFoundException("Price not found.");
            if (price.ValidFrom.Date <= DateTime.Today)
                throw new ArgumentException(NotAFuturePrice);

            product.Prices.Remove(price);
            await productRepository.UpdateAsync(product);
            await LogAsync(product, employeeId, LogTypeDto.Update, [$"ScheduledPrice: Cancelled {Money(price.Price)} from {price.ValidFrom:dd-MM-yyyy}"]);
            return await productService.GetProductDetailsAsync(product.Id);
        }

        public async Task<ProductDto> RecallAsync(RecallRequest request, Guid employeeId)
        {
            request.Validate();
            var product = await GetActiveProductAsync(request.ProductId);
            if (product.Recalled)
                throw new ArgumentException($"{product.ProductName} is already recalled.");

            product.Recalled = true;
            product.RecallReason = request.Reason.Trim();
            product.RecalledAt = DateTime.UtcNow;

            CreateStockLogsDto? writeOff = null;
            if (request.WriteOffStock && product.Stock > 0)
            {
                writeOff = product.ChangeStock(-product.Stock, employeeId, LogTypeDto.WrittenOff);
                writeOff.Messages.Add($"Reason: {WriteOffReasonDto.Recalled}");
                product.Stock = 0;
                product.Batches.Clear();
            }

            foreach (var line in await purchaseOrderRepository.GetOpenLinesForProductAsync(product.Id))
                purchaseOrderRepository.RemoveLine(line);

            await productRepository.UpdateAsync(product);
            await LogAsync(product, employeeId, LogTypeDto.Recalled, [$"Reason: {product.RecallReason}"]);
            if (writeOff is not null)
                await stockLogsService.CreateStockLogsAsync(writeOff);
            return await productService.GetProductDetailsAsync(product.Id);
        }

        public async Task<ProductDto> UndoRecallAsync(Guid productId, Guid employeeId)
        {
            var product = await GetActiveProductAsync(productId);
            if (!product.Recalled)
                throw new ArgumentException($"{product.ProductName} is not recalled.");

            product.Recalled = false;
            product.RecallReason = null;
            product.RecalledAt = null;
            await productRepository.UpdateAsync(product);
            await LogAsync(product, employeeId, LogTypeDto.Update, ["Recalled: New: False. Old: True"]);
            return await productService.GetProductDetailsAsync(product.Id);
        }

        public async Task<ProductDto> BlockOrdersAsync(OrderBlockRequest request, Guid employeeId)
        {
            request.Validate(DateTime.Today);
            var product = await GetActiveProductAsync(request.ProductId);

            product.OrderBlocked = true;
            product.OrderBlockReason = request.Reason.Trim();
            product.OrderBlockedUntil = request.Until?.Date;
            await productRepository.UpdateAsync(product);

            string until = product.OrderBlockedUntil is { } date ? $" until {date:dd-MM-yyyy}" : "";
            await LogAsync(product, employeeId, LogTypeDto.Update, [$"OrderBlock: Blocked{until}", $"Reason: {product.OrderBlockReason}"]);
            return await productService.GetProductDetailsAsync(product.Id);
        }

        public async Task<ProductDto> UnblockOrdersAsync(Guid productId, Guid employeeId)
        {
            var product = await GetActiveProductAsync(productId);
            if (!product.OrderBlocked)
                throw new ArgumentException($"{product.ProductName} is not blocked for ordering.");

            product.OrderBlocked = false;
            product.OrderBlockReason = null;
            product.OrderBlockedUntil = null;
            await productRepository.UpdateAsync(product);
            await LogAsync(product, employeeId, LogTypeDto.Update, ["OrderBlock: Removed"]);
            return await productService.GetProductDetailsAsync(product.Id);
        }

        public async Task<int> ApplyDuePricesAsync(DateTime today)
        {
            int applied = 0;
            foreach (var product in await productRepository.GetProductsWithPricesAsync())
            {
                var current = product.Prices
                    .Where(p => p.ValidFrom.Date <= today.Date)
                    .OrderByDescending(p => p.ValidFrom.Date)
                    .ThenByDescending(p => p.CreatedAt)
                    .FirstOrDefault();
                if (current is null || current.Price == product.Price) continue;

                string message = $"Price: New: {Money(current.Price)}. Old: {Money(product.Price)}";
                product.Price = current.Price;
                await productRepository.UpdateAsync(product);
                if (current.CreatedById is Guid employeeId)
                    await LogAsync(product, employeeId, LogTypeDto.Update, [message, $"ScheduledPrice: Started on {current.ValidFrom:dd-MM-yyyy}"]);
                applied++;
            }
            return applied;
        }

        private async Task<Product> GetActiveProductAsync(Guid productId)
        {
            var product = await productRepository.GetByIdAsync(productId);
            if (product is null || product.SoftDeleted)
                throw new KeyNotFoundException("Product not found.");
            return product;
        }

        private Task LogAsync(Product product, Guid employeeId, LogTypeDto type, List<string> messages) =>
            stockLogsService.CreateStockLogsAsync(new CreateStockLogsDto
            {
                Id = Guid.NewGuid(),
                Identifier = product.Barcode.ToString(),
                LogType = type,
                StockLogType = StockLogTypeDto.Product,
                EmployeeId = employeeId,
                Messages = messages,
            });

        private static string Money(decimal value) => value.ToString("0.00", CultureInfo.InvariantCulture);
    }
}
