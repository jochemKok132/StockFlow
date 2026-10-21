using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Stock.Sales;

namespace StockFlow.Application.Validations.Stock
{
    public static class SalesValidation
    {
        public const int MaxMultiBuyAmount = 99;
        public const decimal MaxBundlePrice = 100_000m;

        public static void Validate(this CreateSalesDto dto)
        {
            Guard.NotNull(dto, "Sale");

            if (dto.SaleType == SaleTypeDto.Moving)
                throw new ArgumentException("The moving sale already exists and can't be created again.");

            ValidateFields(dto.SaleName, dto.Description, dto.SaleType, dto.PercentageOff, dto.BuyAmount, dto.FreeAmount, dto.BundlePrice, dto.StartDate, dto.EndDate);
            ValidateTags(dto.SalesTags);
        }

        public static void Validate(this UpdateSalesDto dto, bool isMovingSale = false)
        {
            Guard.NotNull(dto, "Sale");
            Guard.NotEmpty(dto.Id, "Sale id");

            if (isMovingSale)
            {
                ValidateFields(dto.SaleName, dto.Description, SaleTypeDto.Moving, dto.PercentageOff, 0, 0, 0, dto.StartDate, dto.EndDate);
                return;
            }

            if (dto.SaleType == SaleTypeDto.Moving)
                throw new ArgumentException("A sale can't be turned into the moving sale.");

            ValidateFields(dto.SaleName, dto.Description, dto.SaleType, dto.PercentageOff, dto.BuyAmount, dto.FreeAmount, dto.BundlePrice, dto.StartDate, dto.EndDate);
            ValidateTags(dto.SalesTags);
        }

        private static void ValidateFields(string? name, string? description, SaleTypeDto type, int percentageOff,
            int buyAmount, int freeAmount, decimal bundlePrice, DateTime? startDate, DateTime? endDate)
        {
            Guard.Required(name, "Sale name");
            Guard.Required(description, "Description", FieldLimits.Description);

            if (!Enum.IsDefined(type))
                throw new ArgumentException("Choose a valid sale type.");

            if (type == SaleTypeDto.MultiBuy)
            {
                if (buyAmount < 1 || buyAmount > MaxMultiBuyAmount)
                    throw new ArgumentException($"Buy amount must be between 1 and {MaxMultiBuyAmount}.");

                if (freeAmount < 1 || freeAmount > MaxMultiBuyAmount)
                    throw new ArgumentException($"Free amount must be between 1 and {MaxMultiBuyAmount}.");
            }
            else if (type == SaleTypeDto.Bundle)
            {
                if (buyAmount < 2 || buyAmount > MaxMultiBuyAmount)
                    throw new ArgumentException($"A bundle needs a buy amount between 2 and {MaxMultiBuyAmount}.");

                if (bundlePrice <= 0 || bundlePrice > MaxBundlePrice)
                    throw new ArgumentException($"The bundle price must be between 0.01 and {MaxBundlePrice}.");

                if (decimal.Round(bundlePrice, 2) != bundlePrice)
                    throw new ArgumentException("The bundle price can have at most 2 decimals.");
            }
            else if (type != SaleTypeDto.SecondItemHalfPrice && (percentageOff < 1 || percentageOff > 99))
            {
                throw new ArgumentException("Percentage off must be between 1 and 99.");
            }

            if (startDate is not null && endDate is not null && endDate.Value.Date < startDate.Value.Date)
                throw new ArgumentException("The end date can't be before the start date.");
        }

        private static void ValidateTags(IEnumerable<string>? tags)
        {
            if (tags is null || !tags.Any())
                throw new ArgumentException("A sale needs at least one sales tag.");

            Guard.Tags(tags);
        }
    }
}
