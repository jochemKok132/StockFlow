using StockFlow.Shared.DTOs.Stock.Sales;

namespace StockFlow.Application.Validations.Stock
{
    public static class SalesValidation
    {
        public static void Validate(this CreateSalesDto dto)
        {
            Guard.NotNull(dto, "Sale");
            ValidateFields(dto.SaleName, dto.Description, dto.PercentageOff, dto.SalesTags);
        }

        public static void Validate(this UpdateSalesDto dto)
        {
            Guard.NotNull(dto, "Sale");
            Guard.NotEmpty(dto.Id, "Sale id");
            ValidateFields(dto.SaleName, dto.Description, dto.PercentageOff, dto.SalesTags);
        }

        private static void ValidateFields(string? name, string? description, int percentageOff, IEnumerable<string>? tags)
        {
            Guard.Required(name, "Sale name");
            Guard.Required(description, "Description", 1000);

            if (percentageOff < 1 || percentageOff > 99)
                throw new ArgumentException("Percentage off must be between 1 and 99.");

            if (tags is null || !tags.Any())
                throw new ArgumentException("A sale needs at least one sales tag.");

            Guard.Tags(tags);
        }
    }
}
