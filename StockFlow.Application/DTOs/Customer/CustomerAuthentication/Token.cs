namespace StockFlow.Application.DTOs.Customer.CustomerAuthentication
{
    public class Token
    {
        public string JWT { get; set; } = string.Empty;
        public DateTime ExpiresOn { get; set; }
    }
}
