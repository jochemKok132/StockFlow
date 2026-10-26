using StockFlow.Application.Mappings.Enums;
using StockFlow.Shared.DTOs.Stock.Product;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Domain.Enums;
using StockFlow.Shared.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Mappings.Stock
{
    public static class ProductMappings
    {
        public static ProductDto ToProductDto(this Product product, bool includeSales = true)
        {
            return new ProductDto()
            {
                Description = product.Description,
                Id = product.Id,
                ProductName = product.ProductName,
                Barcode = product.Barcode,
                BrandId = product.BrandId,
                Location = product.Location,
                Price = product.Price,
                ProductImage = product.ProductImage,
                Sales = includeSales ? product.Sales.Select(i => i.ToSalesDto(false)) : [],
                ShelfId = product.ShelfId,
                CreatedAt = product.CreatedAt,
                SoftDeletedAt = product.SoftDeletedAt,
                SoftDeleted = product.SoftDeleted,
                Stock = product.Stock,
                UpdatedAt = product.UpdatedAt,
                SalesTags = product.SalesTags,
                Status = product.Status.ToDto(),
                MinimumStock = product.MinimumStock,
                MaximumStock = product.MaximumStock,
                Orderable = product.Orderable,
                AutoOrder = product.AutoOrder,
                CanExpire = product.CanExpire,
                Recalled = product.Recalled,
                RecallReason = product.RecallReason,
                RecalledAt = product.RecalledAt,
                OrderBlocked = product.OrderBlocked,
                OrderBlockActive = ProductRules.OrderBlockActive(product.OrderBlocked, product.OrderBlockedUntil, DateTime.Today),
                OrderBlockReason = product.OrderBlockReason,
                OrderBlockedUntil = product.OrderBlockedUntil,
                SupplierId = product.SupplierId,
                SupplierName = product.Supplier?.SupplierName,
                Prices = product.Prices.ToPriceDtos(DateTime.Today),
                Batches = product.Batches
                    .Where(b => b.Amount > 0)
                    .OrderBy(b => b.ExpiryDate)
                    .Select(b => new StockBatchDto { Id = b.Id, Amount = b.Amount, ExpiryDate = b.ExpiryDate })
                    .ToList(),
            };
        }

        public static List<ProductPriceDto> ToPriceDtos(this IEnumerable<ProductPrice> prices, DateTime today)
        {
            var list = prices.ToList();
            var current = PricePlanner.Current(list.Select(p => (p.ValidFrom, p.CreatedAt, p.Price)), today);
            return list
                .OrderByDescending(p => p.ValidFrom)
                .ThenByDescending(p => p.CreatedAt)
                .Select(p => new ProductPriceDto
                {
                    Id = p.Id,
                    Price = p.Price,
                    ValidFrom = p.ValidFrom,
                    CreatedAt = p.CreatedAt,
                    CreatedByName = p.CreatedBy?.FullName,
                    IsFuture = p.ValidFrom.Date > today.Date,
                    IsCurrent = current is { } c && c.ValidFrom == p.ValidFrom && c.CreatedAt == p.CreatedAt && c.Price == p.Price,
                })
                .ToList();
        }

        public static decimal CurrentPrice(this Product product, DateTime today) =>
            PricePlanner.CurrentPrice(product.Prices.Select(p => (p.ValidFrom, p.CreatedAt, p.Price)), product.Price, today);

        public static void ConsumeBatches(this Product product, int amount)
        {
            int left = amount;
            foreach (var batch in product.Batches.Where(b => b.Amount > 0).OrderBy(b => b.ExpiryDate).ToList())
            {
                if (left <= 0) break;
                int take = Math.Min(left, batch.Amount);
                batch.Amount -= take;
                left -= take;
            }
            foreach (var empty in product.Batches.Where(b => b.Amount <= 0).ToList())
                product.Batches.Remove(empty);
        }

        public static string? SaleRefusal(this Product product) =>
            ProductRules.SaleRefusal(product.ProductName, product.Status == ProductStatus.Active, product.Status == ProductStatus.Draft, product.Recalled);

        public static string? OrderRefusal(this Product product, DateTime today) =>
            ProductRules.OrderRefusal(product.ProductName, product.Status == ProductStatus.Active, product.Orderable, product.Recalled,
                product.OrderBlocked, product.OrderBlockedUntil, product.OrderBlockReason, today);

        public static Product ToProductEntity(this CreateProductDto dto)
        {
            return new Product()
            {
                Description = dto.Description,
                Id = Guid.NewGuid(),
                ProductName = dto.ProductName,
                Price = dto.Price,
                BrandId = dto.BrandId,
                Location = dto.Location,
                ProductImage = dto.ProductImage,
                SalesTags = dto.SalesTags,
                ShelfId = dto.ShelfId,
                Stock = dto.Stock,
                Status = dto.Status.ToDomain(),
                SupplierId = dto.SupplierId,
                MinimumStock = dto.MinimumStock,
                MaximumStock = dto.MaximumStock,
                Orderable = dto.Orderable,
                AutoOrder = dto.AutoOrder,
                CanExpire = dto.CanExpire,
            };
        }
        public static void ToProductEntity(this Product entity, UpdateProductDto dto)
        {
            entity.Description = dto.Description;
            entity.Location = dto.Location;
            entity.ProductName = dto.ProductName;
            entity.BrandId = dto.BrandId;
            entity.Price = dto.Price;
            entity.ProductImage = dto.ProductImage;
            entity.SalesTags = dto.SalesTags;
            entity.ShelfId = dto.ShelfId;
            entity.Stock = dto.Stock;
            entity.Status = dto.Status.ToDomain();
            entity.SupplierId = dto.SupplierId;
            entity.MinimumStock = dto.MinimumStock;
            entity.MaximumStock = dto.MaximumStock;
            entity.Orderable = dto.Orderable;
            entity.AutoOrder = dto.AutoOrder;
            entity.CanExpire = dto.CanExpire;
        }
        public static void ChangeStock(this Product entity, int amount)
        {
            entity.Stock += amount;
        }
        public static IEnumerable<Sales> ActiveSales(this Product product) =>
            product.Sales.Where(s => !s.SoftDeleted && s.SaleType != SaleType.Moving &&
                                     SalePricing.IsInPeriod(s.StartDate, s.EndDate, DateTime.Today));

        public static ProductBulkViewDto ToProductBulkViewDto(this Product product)
        {
            return new ProductBulkViewDto()
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Barcode = product.Barcode,
                Location = $"{product.Shelf?.ShelfLocation}-{product.Location}",
                Price = product.ActiveSales()
                    .Where(s => s.SaleType == SaleType.Percentage)
                    .Select(s => SalePricing.DiscountedPrice(product.Price, s.PercentageOff))
                    .DefaultIfEmpty(product.Price)
                    .Min(),
                Sale = product.ActiveSales().Any(),
                Stock = product.Stock,
                BrandName = product.Brand?.BrandName ?? string.Empty,
                Status = product.Status.ToDto(),
                MinimumStock = product.MinimumStock,
                Recalled = product.Recalled,
                OrderBlocked = ProductRules.OrderBlockActive(product.OrderBlocked, product.OrderBlockedUntil, DateTime.Today),
                Orderable = product.Orderable,
                CanExpire = product.CanExpire,
            };
        }
    }
}
