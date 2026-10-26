using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Stock.Product;
using StockFlow.Shared.DTOs.Enums;

namespace StockFlow.Application.Validations.Stock
{
    public static class ProductValidation
    {
        public static void Validate(this CreateProductDto dto)
        {
            Guard.NotNull(dto, "Product");
            ValidateFields(dto.ProductName, dto.Description, dto.Price, dto.Stock, dto.Location, dto.ProductImage, dto.SalesTags, dto.BrandId, dto.ShelfId);
            ValidateStockSettings(dto.Status, dto.SupplierId, dto.MinimumStock, dto.MaximumStock, dto.Orderable, dto.AutoOrder);
        }

        public static void Validate(this UpdateProductDto dto)
        {
            Guard.NotNull(dto, "Product");
            Guard.NotEmpty(dto.Id, "Product id");
            ValidateFields(dto.ProductName, dto.Description, dto.Price, dto.Stock, dto.Location, dto.ProductImage, dto.SalesTags, dto.BrandId, dto.ShelfId);
            ValidateStockSettings(dto.Status, dto.SupplierId, dto.MinimumStock, dto.MaximumStock, dto.Orderable, dto.AutoOrder);
        }

        public static void Validate(this SchedulePriceRequest request, DateTime today)
        {
            Guard.NotNull(request, "Price");
            Guard.NotEmpty(request.ProductId, "Product");
            ValidatePrice(request.Price);
            if (request.ValidFrom.Date < today.Date)
                throw new ArgumentException("A new price can't start in the past.");
            if (request.ValidFrom.Date > today.Date.AddYears(2))
                throw new ArgumentException("A new price can start at most 2 years ahead.");
        }

        public static void Validate(this RecallRequest request)
        {
            Guard.NotNull(request, "Recall");
            Guard.NotEmpty(request.ProductId, "Product");
            Guard.Required(request.Reason, "Reason", FieldLimits.Reason);
        }

        public static void Validate(this OrderBlockRequest request, DateTime today)
        {
            Guard.NotNull(request, "Order block");
            Guard.NotEmpty(request.ProductId, "Product");
            Guard.Required(request.Reason, "Reason", FieldLimits.Reason);
            if (request.Until is { } until && until.Date < today.Date)
                throw new ArgumentException("The end date of an order block can't be in the past.");
        }

        private static void ValidateStockSettings(ProductStatusDto status, Guid supplierId, int minimumStock, int maximumStock, bool orderable, bool autoOrder)
        {
            if (!Enum.IsDefined(status))
                throw new ArgumentException("Choose a valid status.");
            Guard.NotEmpty(supplierId, "Supplier");
            if (minimumStock < 0 || minimumStock > FieldLimits.MaxStockLevel)
                throw new ArgumentException($"Minimum stock must be between 0 and {FieldLimits.MaxStockLevel}.");
            if (maximumStock < 0 || maximumStock > FieldLimits.MaxStockLevel)
                throw new ArgumentException($"Maximum stock must be between 0 and {FieldLimits.MaxStockLevel}.");
            if (maximumStock > 0 && maximumStock < minimumStock)
                throw new ArgumentException("Maximum stock can't be lower than the minimum stock.");
            if (autoOrder && minimumStock == 0)
                throw new ArgumentException("Set a minimum stock to order automatically.");
            if (autoOrder && !orderable)
                throw new ArgumentException("A non-orderable product can't be ordered automatically.");
        }

        private static void ValidatePrice(decimal price)
        {
            Guard.Range(price, 0, 100_000, "Price");
            if (decimal.Round(price, 2) != price)
                throw new ArgumentException("Price can have at most 2 decimals.");
        }

        public static void Validate(this WriteOffRequest request)
        {
            Guard.NotNull(request, "Write-off");
            Guard.NotEmpty(request.ProductId, "Product");

            if (request.Amount < 1 || request.Amount > FieldLimits.MaxWriteOffAmount)
                throw new ArgumentException($"The amount to write off must be between 1 and {FieldLimits.MaxWriteOffAmount}.");

            if (!Enum.IsDefined(request.Reason))
                throw new ArgumentException("Choose a reason for the write-off.");

            Guard.Optional(request.Note, "Note", FieldLimits.WriteOffNote);

            if (request.Reason == WriteOffReasonDto.Other && string.IsNullOrWhiteSpace(request.Note))
                throw new ArgumentException("Explain the reason in the note when you choose Other.");
        }

        public static void ValidateStockChange(int amount)
        {
            if (amount == 0)
                throw new ArgumentException("A stock change can't be 0.");
        }

        private static void ValidateFields(string? name, string? description, decimal price, int stock, int location,
            byte[]? image, IEnumerable<string>? tags, Guid brandId, Guid shelfId)
        {
            Guard.Required(name, "Product name");
            Guard.Required(description, "Description", FieldLimits.Description);
            Guard.Range(price, 0, 100_000, "Price");

            if (decimal.Round(price, 2) != price)
                throw new ArgumentException("Price can have at most 2 decimals.");

            if (location < 0)
                throw new ArgumentException("Location can't be negative.");

            if (image is { Length: > FieldLimits.MaxImageBytes })
                throw new ArgumentException("The product image can be at most 5 MB.");

            if (image is { Length: > 0 } && !ImageFormats.IsImage(image))
                throw new ArgumentException("The product image must be a PNG, JPEG, GIF or WebP image.");

            Guard.Tags(tags);
            Guard.NotEmpty(brandId, "Brand");
            Guard.NotEmpty(shelfId, "Shelf");
        }
    }
}
