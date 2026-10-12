using System.Collections;
using System.Reflection;

public static class DifferenceFinder
{
    public static List<string> FindDifferences<TNew, TOld>(this TNew newValues, TOld oldValues)
    {
        var differences = new List<string>();
        var oldProperties = typeof(TOld).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var newProperties = typeof(TNew).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var oldProp in oldProperties)
        {
            var newProp = newProperties.FirstOrDefault(p => p.Name == oldProp.Name);
            if (newProp == null) continue;

            if (IsModelType(oldProp.PropertyType) || IsModelType(newProp.PropertyType)) continue;

            var oldRaw = oldProp.GetValue(oldValues);
            var newRaw = newProp.GetValue(newValues);

            if (oldProp.IsEnumerable() || newProp.IsEnumerable())
            {
                if (oldRaw is byte[] || newRaw is byte[])
                {
                    var oldBytes = oldRaw as byte[];
                    var newBytes = newRaw as byte[];
                    var same = oldBytes is null ? newBytes is null
                             : newBytes is not null && oldBytes.AsSpan().SequenceEqual(newBytes);

                    if (!same)
                        differences.Add($"{oldProp.Name}: Changed Image.");
                    continue;
                }

                var oldItems = ToItems(oldRaw);
                var newItems = ToItems(newRaw);

                if (oldItems.Concat(newItems).Any(IsModel)) continue;

                var oldTexts = oldItems.Select(i => i?.ToString() ?? "-").ToList();
                var newTexts = newItems.Select(i => i?.ToString() ?? "-").ToList();

                var added = newTexts.Except(oldTexts).ToList();
                var removed = oldTexts.Except(newTexts).ToList();

                if (added.Count > 0 || removed.Count > 0)
                    differences.Add($"{oldProp.Name}: Added: [{string.Join(", ", added)}]. Removed: [{string.Join(", ", removed)}]");

                continue;
            }

            var oldValue = oldRaw?.ToString() ?? "-";
            var newValue = newRaw?.ToString() ?? "-";

            if ((oldValue == newValue) || oldProp.Name.Contains("Password"))
                continue;

            differences.Add($"{oldProp.Name}: New: {newValue}. Old: {oldValue}");
        }

        return differences;
    }

    public static bool IsEnumerable(this PropertyInfo property)
    {
        Type type = property.PropertyType;
        if (type == typeof(string))
            return false;

        return typeof(IEnumerable).IsAssignableFrom(type);
    }

    private static List<object?> ToItems(object? value) =>
        value is IEnumerable items ? items.Cast<object?>().ToList() : new List<object?>();

    private static bool IsModel(object? item) =>
        item is not null && IsModelType(item.GetType());

    private static bool IsModelType(Type type)
    {
        if (type.IsEnum) return false;
        var ns = type.Namespace ?? "";
        return ns.StartsWith("StockFlow.Domain.Entities") || ns.StartsWith("StockFlow.Application.DTOs");
    }

    private static readonly Lazy<HashSet<string>> NavigationPropertyNames = new(() =>
        typeof(StockFlow.Domain.Entities.Stock.Product).Assembly.GetTypes()
            .Where(t => t.Namespace?.StartsWith("StockFlow.Domain.Entities") == true)
            .SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            .Where(p => IsModelType(p.PropertyType))
            .Select(p => p.Name)
            .ToHashSet());

    public static List<string> WithoutNavigationChanges(this IEnumerable<string> messages) =>
        messages
            .Where(m =>
            {
                var separator = m.IndexOf(": ", StringComparison.Ordinal);
                return separator < 0 || !NavigationPropertyNames.Value.Contains(m[..separator]);
            })
            .ToList();
}