using StockFlow.Shared.DTOs.AuditLogs.CashRegister;

namespace StockFlow.Application.Interfaces
{
    public interface IReceiptPdfService
    {
        byte[] Create(CashRegisterLogsDto receipt);
    }
}
