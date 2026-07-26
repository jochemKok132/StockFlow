namespace StockFlow.Application.DTOs.Employee.EmployeeAuthentication
{
    public class Token
    {
        public string JWT { get; set; } = string.Empty;
        public DateTime ExpiresOn { get; set; }
    }
}
