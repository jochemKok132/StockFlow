using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Stock.Product;

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
