using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.ManagementApp.Components;
using StockFlow.ManagementApp.Interfaces;
using StockFlow.ManagementApp.Interfaces.Stock;

namespace StockFlow.ManagementApp.Services
{
    public class LookupService(IProductBrandService brandService, IShelfService shelfService, ISalesService salesService, ISupplierService supplierService) : ILookupService
    {
        private const int PageSize = 100;
        private const int MaxPages = 50;

        public async Task<(List<SelectOption>, string error)> GetBrandOptionsAsync()
        {
            var (brands, error) = await LoadAllAsync(page => brandService.GetPaginatedProductBrandsAsync(new ProductBrandPaginationDto
            {
                PageNumber = page,
                PageSize = PageSize,
                OrderBy = ProductBrandOrderBy.BrandName,
                OrderType = OrderType.Ascending,
            }));

            return (brands.Select(b => new SelectOption(b.Id.ToString(), b.BrandName)).ToList(), error);
        }

        public async Task<(List<SelectOption>, string error)> GetShelfOptionsAsync()
        {
            var (shelves, error) = await LoadAllAsync(page => shelfService.GetPaginatedShelfAsync(new ShelfPaginationDto
            {
                PageNumber = page,
                PageSize = PageSize,
                OrderBy = ShelfOrderBy.ShelfName,
                OrderType = OrderType.Ascending,
            }));

            return (shelves
                .Select(s => new SelectOption(
                    s.Id.ToString(),
                    s.ShelfName,
                    string.IsNullOrWhiteSpace(s.ShelfHallway)
                        ? $"Location {s.ShelfLocation}"
                        : $"Location {s.ShelfLocation} · Hallway {s.ShelfHallway}"))
                .ToList(), error);
        }

        public async Task<(List<SelectOption>, string error)> GetSalesTagOptionsAsync()
        {
            var (sales, error) = await LoadAllAsync(page => salesService.GetPaginatedSalesAsync(new SalesPaginationDto
            {
                PageNumber = page,
                PageSize = PageSize,
                OrderBy = SalesOrderBy.SaleName,
                OrderType = OrderType.Ascending,
            }));

            var options = sales
                .SelectMany(s => s.SalesTags.Select(tag => (Tag: tag.Trim(), Sale: s)))
                .Where(x => x.Tag.Length > 0)
                .GroupBy(x => x.Tag, StringComparer.OrdinalIgnoreCase)
                .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
                .Select(g => new SelectOption(
                    g.Key,
                    g.Key,
                    "Used by: " + string.Join(", ", g.Select(x => $"{x.Sale.SaleName} ({SalePricing.Describe(x.Sale)})").Distinct())))
                .ToList();

            return (options, error);
        }

        public async Task<(List<SelectOption>, string error)> GetSupplierOptionsAsync()
        {
            var (suppliers, error) = await LoadAllAsync(page => supplierService.GetPaginatedSuppliersAsync(new SupplierPaginationDto
            {
                PageNumber = page,
                PageSize = PageSize,
                OrderBy = SupplierOrderBy.SupplierName,
                OrderType = OrderType.Ascending,
            }));
            return (suppliers.Select(s => new SelectOption(s.Id.ToString(), s.SupplierName, $"Order day {s.OrderDay}")).ToList(), error);
        }

        public async Task<(List<SelectOption>, string error)> GetSaleOptionsAsync()
        {
            var (sales, error) = await LoadAllAsync(page => salesService.GetPaginatedSalesAsync(new SalesPaginationDto
            {
                PageNumber = page,
                PageSize = PageSize,
                OrderBy = SalesOrderBy.SaleName,
                OrderType = OrderType.Ascending,
            }));
            return (sales
                .Where(s => s.SaleType != SaleTypeDto.Moving)
                .Select(s => new SelectOption(s.Id.ToString(), s.SaleName, SalePricing.Describe(s)))
                .ToList(), error);
        }

        private static async Task<(List<T>, string error)> LoadAllAsync<T>(Func<int, Task<(List<T>, string error)>> loadPage)
        {
            var all = new List<T>();

            for (int page = 1; page <= MaxPages; page++)
            {
                var (items, error) = await loadPage(page);
                if (!string.IsNullOrEmpty(error))
                    return (all, error);

                all.AddRange(items);
                if (items.Count < PageSize)
                    break;
            }

            return (all, "");
        }
    }
}
