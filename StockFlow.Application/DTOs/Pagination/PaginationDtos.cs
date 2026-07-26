using StockFlow.Application.DTOs.Enums;

namespace StockFlow.Application.DTOs.Pagination
{
    public class ProductPaginationDto : Pagination<ProductOrderBy> { }
    public class ProductBrandPaginationDto : Pagination<ProductBrandOrderBy> { }
    public class SalesPaginationDto : Pagination<SalesOrderBy> { }
    public class ShelfPaginationDto : Pagination<ShelfOrderBy> { }
    public class CustomerPaginationDto : Pagination<CustomerOrderBy> { }
    public class EmployeePaginationDto : Pagination<EmployeeOrderBy> { }
    public class CashRegisterLogsPaginationDto : Pagination<CashRegisterLogsOrderBy> { }
    public class StockLogsPaginationDto : Pagination<StockLogsOrderBy> { }
    public class CustomerLogsPaginationDto : Pagination<CustomerLogsOrderBy> { }
}