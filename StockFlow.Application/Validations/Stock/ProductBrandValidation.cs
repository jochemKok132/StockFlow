using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Stock.ProductBrand;

namespace StockFlow.Application.Validations.Stock
{
    public static class ProductBrandValidation
    {
        public static void Validate(this CreateProductBrandDto dto)
        {
            Guard.NotNull(dto, "Brand");
            Guard.Required(dto.BrandName, "Brand name");
            ValidateInfo(dto.Description, dto.Website, dto.Country, dto.ContactEmail, dto.CustomerServicePhone, dto.Logo);
        }

        public static void Validate(this UpdateProductBrandDto dto)
        {
            Guard.NotNull(dto, "Brand");
            Guard.NotEmpty(dto.Id, "Brand id");
            Guard.Required(dto.BrandName, "Brand name");
            ValidateInfo(dto.Description, dto.Website, dto.Country, dto.ContactEmail, dto.CustomerServicePhone, dto.Logo);
        }

        private static void ValidateInfo(string? description, string? website, string? country, string? email, string? phone, byte[]? logo)
        {
            Guard.Optional(description, "Description", FieldLimits.Description);
            Guard.Optional(website, "Website", FieldLimits.Url);
            if (!string.IsNullOrWhiteSpace(website) && !Uri.TryCreate(website.Trim(), UriKind.Absolute, out _) && !website.Trim().Contains('.'))
                throw new ArgumentException("Website is not a valid address.");
            Guard.Optional(country, "Country", FieldLimits.Country);
            if (!string.IsNullOrWhiteSpace(email))
                Guard.Email(email.Trim(), "Contact email");
            Guard.Optional(phone, "Customer service phone", FieldLimits.Phone);
            if (logo is { Length: > FieldLimits.MaxImageBytes })
                throw new ArgumentException("The logo can be at most 5 MB.");
            if (logo is { Length: > 0 } && !ImageFormats.IsImage(logo))
                throw new ArgumentException("The logo must be a PNG, JPEG, GIF or WebP image.");
        }
    }
}
