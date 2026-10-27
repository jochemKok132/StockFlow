using StockFlow.Domain.Entities.AuditLogs;
using StockFlow.Shared.DTOs.AuditLogs.Return;

namespace StockFlow.Application.Mappings.AuditLogs
{
    public static class ReturnLogsMappings
    {
        public static ReturnLogDto ToReturnLogDto(this ReturnLogs log) => new()
        {
            Id = log.Id,
            CreatedAt = log.CreatedAt,
            ReceiptId = log.ReceiptId,
            EmployeeId = log.Employee?.EmployeeId ?? "-",
            EmployeeName = log.Employee?.FullName ?? "-",
            CustomerId = log.Customer?.CustomerId,
            CustomerName = log.Customer?.FullName,
            RefundAmount = log.RefundAmount,
            PointsDeducted = log.PointsDeducted,
            ItemsReturned = log.Lines.Sum(l => l.Amount),
            Lines = log.Lines
                .OrderBy(l => l.Product?.ProductName)
                .Select(l => new ReturnLogLineDto
                {
                    Barcode = l.Product?.Barcode ?? 0,
                    ProductName = l.Product?.ProductName ?? "-",
                    Amount = l.Amount,
                    PriceAtTimeOfSale = l.PriceAtTimeOfSale,
                    RefundAmount = l.RefundAmount,
                })
                .ToList(),
        };
    }
}
