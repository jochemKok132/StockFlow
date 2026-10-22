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
        }

        public static void Validate(this UpdateProductDto dto)
        {
            Guard.NotNull(dto, "Product");
            Guard.NotEmpty(dto.Id, "Product id");
            ValidateFields(dto.ProductName, dto.Description, dto.Price, dto.Stock, dto.Location, dto.ProductImage, dto.SalesTags, dto.BrandId, dto.ShelfId);
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
