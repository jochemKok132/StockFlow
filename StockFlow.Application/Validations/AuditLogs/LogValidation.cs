using StockFlow.Application.DTOs.AuditLogs.CashRegister;
using StockFlow.Application.DTOs.AuditLogs.Customer;
using StockFlow.Application.DTOs.AuditLogs.Employee;
using StockFlow.Application.DTOs.AuditLogs.Stock;

namespace StockFlow.Application.Validations.AuditLogs
{
    public static class LogValidation
    {
        public static void Validate(this CreateStockLogsDto log)
        {
            Guard.NotNull(log, "Stock log");
            Guard.Required(log.Identifier, "Identifier");
            Guard.NotEmpty(log.EmployeeId, "Employee");
            ValidateEnum(log.LogType, "Log type");
            ValidateEnum(log.StockLogType, "Stock log type");
        }

        public static void Validate(this CreateEmployeeLogsDto log)
        {
            Guard.NotNull(log, "Employee log");
            Guard.Required(log.Identifier, "Identifier");
            Guard.NotEmpty(log.EmployeeId, "Employee");
            ValidateEnum(log.LogType, "Log type");
        }

        public static void Validate(this CreateCustomerLogsDto log)
        {
            Guard.NotNull(log, "Customer log");
            Guard.NotEmpty(log.CustomerId, "Customer");
            ValidateEnum(log.LogType, "Log type");
        }

        public static void Validate(this CreateCashRegisterLogsDto log)
        {
            Guard.NotNull(log, "Cash register log");
            Guard.NotEmpty(log.EmployeeId, "Employee");

            if (log.PointsUsed < 0 || log.PointsEarned < 0)
                throw new ArgumentException("Points used and points earned can't be negative.");
        }

        private static void ValidateEnum<TEnum>(TEnum value, string field) where TEnum : struct, Enum
        {
            if (!Enum.IsDefined(value))
                throw new ArgumentException($"{field} is invalid.");
        }
    }
}
