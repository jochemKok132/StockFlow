using StockFlow.Shared.DTOs.Stock.ProductBrand;

namespace StockFlow.Application.Validations.Stock
{
    public static class ProductBrandValidation
    {
        public static void Validate(this CreateProductBrandDto dto)
        {
            Guard.NotNull(dto, "Brand");
            Guard.Required(dto.BrandName, "Brand name");
        }

        public static void Validate(this UpdateProductBrandDto dto)
        {
            Guard.NotNull(dto, "Brand");
            Guard.NotEmpty(dto.Id, "Brand id");
            Guard.Required(dto.BrandName, "Brand name");
        }
    }
}
