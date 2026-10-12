using StockFlow.Application.DTOs.Stock.Product;

namespace StockFlow.Application.Validations.Stock
{
    public static class ProductValidation
    {
        private const int MaxImageBytes = 5 * 1024 * 1024;

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
            Guard.Required(description, "Description", 1000);
            Guard.Range(price, 0, 100_000, "Price");

            if (decimal.Round(price, 2) != price)
                throw new ArgumentException("Price can have at most 2 decimals.");


            if (location < 0)
                throw new ArgumentException("Location can't be negative.");

            if (image is { Length: > MaxImageBytes })
                throw new ArgumentException("The product image can be at most 5 MB.");

            Guard.Tags(tags);
            Guard.NotEmpty(brandId, "Brand");
            Guard.NotEmpty(shelfId, "Shelf");
        }
    }
}
