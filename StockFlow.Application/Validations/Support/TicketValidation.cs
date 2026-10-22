using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Support;

namespace StockFlow.Application.Validations.Support
{
    public static class TicketValidation
    {
        public static void Validate(this CreateTicketRequest request)
        {
            Guard.NotNull(request, "Ticket");

            if (!Enum.IsDefined(request.Type))
                throw new ArgumentException("Choose a valid ticket type.");

            Guard.Required(request.Title, "Title", FieldLimits.Name);
            Guard.Required(request.Description, "Description", FieldLimits.Description);
        }

        public static void Validate(this HandleTicketRequest request)
        {
            Guard.NotNull(request, "Ticket");

            if (!Enum.IsDefined(request.Status))
                throw new ArgumentException("Choose a valid status.");

            Guard.Optional(request.Response, "Response", FieldLimits.Description);
        }
    }
}
