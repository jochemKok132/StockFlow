using StockFlow.Application.DTOs.Stock.Shelf;

namespace StockFlow.Application.Validations.Stock
{
    public static class ShelfValidation
    {
        public static void Validate(this CreateShelfDto dto)
        {
            Guard.NotNull(dto, "Shelf");
            ValidateFields(dto.ShelfName, dto.ShelfHallway, dto.ShelfLocation);
        }

        public static void Validate(this UpdateShelfDto dto)
        {
            Guard.NotNull(dto, "Shelf");
            Guard.NotEmpty(dto.Id, "Shelf id");
            ValidateFields(dto.ShelfName, dto.ShelfHallway, dto.ShelfLocation);
        }

        private static void ValidateFields(string? name, string? hallway, string? location)
        {
            Guard.Required(name, "Shelf name");
            Guard.Optional(hallway, "Hallway", 50);
            Guard.Required(location, "Shelf location", 50);
        }
    }
}
