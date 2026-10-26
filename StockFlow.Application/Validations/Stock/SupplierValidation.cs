using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Stock.Supplier;

namespace StockFlow.Application.Validations.Stock
{
    public static class SupplierValidation
    {
        public static void Validate(this CreateSupplierDto dto)
        {
            Guard.NotNull(dto, "Supplier");
            Guard.Required(dto.SupplierName, "Supplier name");
            Guard.Optional(dto.ContactName, "Contact name");
            if (!string.IsNullOrWhiteSpace(dto.Email))
                Guard.Email(dto.Email.Trim());
            Guard.Optional(dto.Phone, "Phone", FieldLimits.Phone);
            Guard.Optional(dto.Website, "Website", FieldLimits.Url);
            if (!Enum.IsDefined(dto.OrderDay))
                throw new ArgumentException("Choose a valid order day.");
            if (dto.LeadTimeDays < 0 || dto.LeadTimeDays > FieldLimits.MaxLeadTimeDays)
                throw new ArgumentException($"Lead time must be between 0 and {FieldLimits.MaxLeadTimeDays} days.");
        }

        public static void Validate(this UpdateSupplierDto dto)
        {
            Guard.NotNull(dto, "Supplier");
            Guard.NotEmpty(dto.Id, "Supplier id");
            ((CreateSupplierDto)dto).Validate();
        }
    }
}
