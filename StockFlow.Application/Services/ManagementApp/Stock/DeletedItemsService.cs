using StockFlow.Application.Interfaces;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.Repositories;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Shared.DTOs.AuditLogs.Stock;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Stock;

namespace StockFlow.Application.Services.ManagementApp.Stock
{
    public class DeletedItemsService(
        IDeletedItemsRepository deletedItemsRepository,
        IProductRepository productRepository,
        IProductBrandRepository productBrandRepository,
        ISalesRepository salesRepository,
        IShelfRepository shelfRepository,
        IStockLogsService stockLogsService) : IDeletedItemsService
    {
        public const string NotDeleted = "This item isn't deleted.";

        public async Task<List<DeletedItemDto>> GetDeletedItemsAsync(DeletedItemTypeDto? type)
        {
            if (type is { } value && !Enum.IsDefined(value))
                throw new ArgumentException("Choose a valid item type.");

            var items = new List<DeletedItemDto>();

            if (type is null or DeletedItemTypeDto.Product)
                items.AddRange((await deletedItemsRepository.GetDeletedProductsAsync()).Select(ToDto));
            if (type is null or DeletedItemTypeDto.ProductBrand)
                items.AddRange((await deletedItemsRepository.GetDeletedBrandsAsync()).Select(ToDto));
            if (type is null or DeletedItemTypeDto.Sale)
                items.AddRange((await deletedItemsRepository.GetDeletedSalesAsync()).Select(ToDto));
            if (type is null or DeletedItemTypeDto.Shelf)
                items.AddRange((await deletedItemsRepository.GetDeletedShelvesAsync()).Select(ToDto));

            return items.OrderByDescending(i => i.DeletedAt).ToList();
        }

        public async Task<DeletedItemDto> RestoreAsync(DeletedItemTypeDto type, Guid id, Guid employeeId)
        {
            DeletedItemDto restored;
            StockLogTypeDto logType;

            switch (type)
            {
                case DeletedItemTypeDto.Product:
                {
                    var product = Deleted(await productRepository.GetByIdAsync(id), "Product");
                    var brand = await productBrandRepository.GetByIdAsync(product.BrandId);
                    if (brand is null || brand.SoftDeleted)
                        throw new ArgumentException($"Restore the brand {brand?.BrandName ?? "of this product"} first.");
                    var shelf = await shelfRepository.GetByIdAsync(product.ShelfId);
                    if (shelf is null || shelf.SoftDeleted)
                        throw new ArgumentException($"Restore the shelf {shelf?.ShelfName ?? "of this product"} first.");

                    restored = ToDto(product);
                    await deletedItemsRepository.RestoreAsync(product);
                    logType = StockLogTypeDto.Product;
                    break;
                }
                case DeletedItemTypeDto.ProductBrand:
                {
                    var brand = Deleted(await productBrandRepository.GetByIdAsync(id), "Brand");
                    restored = ToDto(brand);
                    await deletedItemsRepository.RestoreAsync(brand);
                    logType = StockLogTypeDto.ProductBrand;
                    break;
                }
                case DeletedItemTypeDto.Sale:
                {
                    var sale = Deleted(await salesRepository.GetByIdAsync(id), "Sale");
                    restored = ToDto(sale);
                    await deletedItemsRepository.RestoreAsync(sale);
                    logType = StockLogTypeDto.Sales;
                    break;
                }
                case DeletedItemTypeDto.Shelf:
                {
                    var shelf = Deleted(await shelfRepository.GetByIdAsync(id), "Shelf");
                    restored = ToDto(shelf);
                    await deletedItemsRepository.RestoreAsync(shelf);
                    logType = StockLogTypeDto.Shelf;
                    break;
                }
                default:
                    throw new ArgumentException("Choose a valid item type.");
            }

            await stockLogsService.CreateStockLogsAsync(new CreateStockLogsDto
            {
                Id = Guid.NewGuid(),
                Identifier = restored.Identifier,
                LogType = LogTypeDto.Restored,
                StockLogType = logType,
                Messages = [$"Restored: {restored.Name}"],
                EmployeeId = employeeId,
            });

            return restored;
        }

        private static T Deleted<T>(T? entity, string name) where T : class, Domain.Entities.IEntities.ISoftDeletable
        {
            if (entity is null) throw new KeyNotFoundException($"{name} not found.");
            if (!entity.SoftDeleted) throw new ArgumentException(NotDeleted);
            return entity;
        }

        private static DeletedItemDto ToDto(Product product) => new()
        {
            Id = product.Id,
            Type = DeletedItemTypeDto.Product,
            Identifier = product.Barcode.ToString(),
            Name = product.ProductName,
            Details = product.Brand is null ? null : $"{product.Brand.BrandName} · stock {product.Stock}",
            DeletedAt = product.SoftDeletedAt,
        };

        private static DeletedItemDto ToDto(ProductBrand brand) => new()
        {
            Id = brand.Id,
            Type = DeletedItemTypeDto.ProductBrand,
            Identifier = brand.BrandName,
            Name = brand.BrandName,
            DeletedAt = brand.SoftDeletedAt,
        };

        private static DeletedItemDto ToDto(Sales sale) => new()
        {
            Id = sale.Id,
            Type = DeletedItemTypeDto.Sale,
            Identifier = sale.SaleName,
            Name = sale.SaleName,
            Details = string.Join(", ", sale.SalesTags),
            DeletedAt = sale.SoftDeletedAt,
        };

        private static DeletedItemDto ToDto(Shelf shelf) => new()
        {
            Id = shelf.Id,
            Type = DeletedItemTypeDto.Shelf,
            Identifier = shelf.ShelfName,
            Name = shelf.ShelfName,
            Details = $"Location {shelf.ShelfLocation}",
            DeletedAt = shelf.SoftDeletedAt,
        };
    }
}
