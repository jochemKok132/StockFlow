using System.ComponentModel.DataAnnotations;

namespace StockFlow.Application.Validations
{
    internal static class Guard
    {
        public static void Required(string? value, string field, int maxLength = 100)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{field} is required.");

            if (value.Trim().Length > maxLength)
                throw new ArgumentException($"{field} can be at most {maxLength} characters.");
        }

        public static void Optional(string? value, string field, int maxLength = 100)
        {
            if (value is not null && value.Trim().Length > maxLength)
                throw new ArgumentException($"{field} can be at most {maxLength} characters.");
        }

        public static void NotEmpty(Guid value, string field)
        {
            if (value == Guid.Empty)
                throw new ArgumentException($"{field} is required.");
        }

        public static void Range(decimal value, decimal min, decimal max, string field)
        {
            if (value < min || value > max)
                throw new ArgumentException($"{field} must be between {min} and {max}.");
        }

        public static void Email(string? value, string field = "Email")
        {
            Required(value, field, 254);
            if (!new EmailAddressAttribute().IsValid(value))
                throw new ArgumentException($"{field} is not a valid email address.");
        }

        public static void Tags(IEnumerable<string>? tags, string field = "Sales tags")
        {
            if (tags is null) return;
            foreach (var tag in tags)
                Required(tag, $"Every value in {field}", 50);
        }

        public static void NotNull(object? value, string field)
        {
            if (value is null)
                throw new ArgumentException($"{field} is required.");
        }
    }
}
