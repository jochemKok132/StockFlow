using System.Text.Json;

namespace StockFlow.CashRegister
{
    public class RegisterSettings
    {
        public const string FileName = "appsettings.json";
        public const string DefaultApiBaseUrl = "https://localhost:7037/";

        public string ApiBaseUrl { get; set; } = DefaultApiBaseUrl;

        public static RegisterSettings Load()
        {
            string path = Path.Combine(AppContext.BaseDirectory, FileName);
            if (!File.Exists(path))
                return new RegisterSettings();

            try
            {
                var settings = JsonSerializer.Deserialize<RegisterSettings>(
                    File.ReadAllText(path),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return settings is null || string.IsNullOrWhiteSpace(settings.ApiBaseUrl)
                    ? new RegisterSettings()
                    : settings;
            }
            catch (JsonException)
            {
                return new RegisterSettings();
            }
        }

        public Uri ApiBaseUri => new(ApiBaseUrl.EndsWith('/') ? ApiBaseUrl : ApiBaseUrl + "/");
    }
}
