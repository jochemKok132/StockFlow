using StockFlow.Application.DTOs.Enums;

namespace StockFlow.Application.DTOs.Pagination
{
    public class ProductPaginationDto : Pagination<ProductOrderBy> 
    {
        public string? ProductName { get; set; }
        public int Barcode { get; set; }
        public int Location { get; set; }
    }
    public class ProductBrandPaginationDto : Pagination<ProductBrandOrderBy>
    {
        public string? BrandName { get; set; }
    }
    public class SalesPaginationDto : Pagination<SalesOrderBy>
    {
        public string? SalesName { get; set; }
    }
    public class ShelfPaginationDto : Pagination<ShelfOrderBy>
    {
        public string? ShelfName { get; set; }
        public int Location { get; set; }
    }
    public class CustomerPaginationDto : Pagination<CustomerOrderBy>
    {
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public string? PostalCode { get; set; }
        public string? HouseNumber { get; set; }
    }
    public class EmployeePaginationDto : Pagination<EmployeeOrderBy>
    {
        public string? EmployeeName { get; set; }
        public string? EmployeeId { get; set; }
        public EmployeeRoleDto? Role { get; set; }
    }
    public class CashRegisterLogsPaginationDto : Pagination<CashRegisterLogsOrderBy>
    {
        public string? EmployeeName { get; set; }
        public string? EmployeeId { get; set; }
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
    }
    public class StockLogsPaginationDto : Pagination<StockLogsOrderBy>
    {
        public string? EmployeeName { get; set; }
        public string? EmployeeId { get; set; }
        public int Barcode { get; set; }
        public StockLogTypeDto? StockLogType { get; set; }
        public LogTypeDto? LogType { get; set; }
    }
    public class CustomerLogsPaginationDto : Pagination<CustomerLogsOrderBy>
    {
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public LogTypeDto? LogType { get; set; }

    }
}