using System.Reflection;

namespace StockFlow.Shared.Common
{
    public static class AppVersion
    {
        public const string DevVersion = "0.0.0-dev";

        public static string Of(Assembly assembly)
        {
            string version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? assembly.GetName().Version?.ToString(3)
                ?? DevVersion;

            int metadata = version.IndexOf('+');
            return metadata >= 0 ? version[..metadata] : version;
        }

        public static bool IsDev(string? version) =>
            string.IsNullOrWhiteSpace(version) || version.EndsWith("-dev", StringComparison.OrdinalIgnoreCase);

        public static string Display(string? version) =>
            IsDev(version) ? "dev build" : $"v{version!.TrimStart('v', 'V')}";

        public static bool IsNewer(string? candidate, string? current)
        {
            if (IsDev(current)) return false;

            var candidateCore = Core(candidate);
            var currentCore = Core(current);
            return candidateCore is not null && currentCore is not null && candidateCore > currentCore;
        }

        public static bool AreSame(string? first, string? second)
        {
            var firstCore = Core(first);
            return firstCore is not null && firstCore == Core(second)
                && string.Equals(Suffix(first), Suffix(second), StringComparison.OrdinalIgnoreCase);
        }

        private static Version? Core(string? version)
        {
            if (string.IsNullOrWhiteSpace(version)) return null;

            string value = version.Trim().TrimStart('v', 'V');
            int suffix = value.IndexOfAny(['-', '+']);
            if (suffix >= 0) value = value[..suffix];

            if (!Version.TryParse(value, out var parsed)) return null;
            return new Version(parsed.Major, parsed.Minor, Math.Max(parsed.Build, 0));
        }

        private static string Suffix(string? version)
        {
            if (string.IsNullOrWhiteSpace(version)) return "";
            int dash = version.IndexOf('-');
            return dash >= 0 ? version[dash..] : "";
        }
    }
}
