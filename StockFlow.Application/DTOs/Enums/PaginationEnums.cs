using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Application.DTOs.Enums
{
    public enum OrderType
    {
        Descending,
        Ascending
    }

    public enum ProductOrderBy
    {
        CreatedAt,
        UpdatedAt,
        ProductName,
        Stock,
    }

    public enum ProductBrandOrderBy
    {
        CreatedAt,
        UpdatedAt,
        BrandName,
        ProductAmount,
    }

    public enum SalesOrderBy 
    {
        CreatedAt,
        UpdatedAt,
        SaleName,
        PercentageOff,
        ProductAmount,
    }
    public enum ShelfOrderBy
    {
        CreatedAt,
        UpdatedAt,
        ShelfName,
        ShelfLocation,
    }

    public enum CustomerOrderBy
    {
        CreatedAt,
        UpdatedAt,
        FullName,
        SavedPoints
    }

    public enum EmployeeOrderBy
    {
        CreatedAt,
        UpdatedAt,
        EmployeeName,
        EmployeeId,
        Role
    }

    public enum CashRegisterLogsOrderBy
    {
        CreatedAt,
        TotalPrice,
        TotalOff,
        TotalProducts,
        EmployeeId,
        CustomerName
    }

    public enum StockLogsOrderBy
    {
        CreatedAt,
        StockLogType,
        EmployeeId,
    }
    public enum CustomerLogsOrderBy 
    {
        CreatedAt,
        CustomerName
    }
}
