using System.Security.Claims;
using System.Text.Json;

namespace StockFlow.CashRegister.Authentication
{
    public static class ParseJWT
    {
        public static List<Claim> GetClaims(string token)
        {
            List<Claim> claims = new List<Claim>();

            Dictionary<string, JsonElement>? dictionary = token
                .GetPayload()
                .ToBase64()
                .ToValidPayload()
                .ToBytesOfJSON()
                .ToDictionaryOfJSON();

            if (dictionary == null)
                return claims;

            foreach (KeyValuePair<string, JsonElement> pair in dictionary)
                claims.AddPair(pair);

            return claims;
        }

        private static string GetPayload(this string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return string.Empty;

            string[] jwt = token.Split('.');
            if (jwt.Length != 3)
                return string.Empty;

            return jwt[1];
        }

        private static string ToBase64(this string payload)
        {
            if (string.IsNullOrEmpty(payload))
                return string.Empty;

            return payload.Replace('-', '+').Replace('_', '/');
        }

        private static string ToValidPayload(this string payload)
        {
            if (string.IsNullOrEmpty(payload))
                return string.Empty;

            int mod = payload.Length % 4;
            if (mod == 0)
                return payload;

            return payload + new string('=', 4 - mod);
        }

        private static byte[] ToBytesOfJSON(this string base64)
        {
            if (string.IsNullOrEmpty(base64))
                return [];

            try
            {
                return Convert.FromBase64String(base64);
            }
            catch
            {
                return [];
            }
        }

        private static Dictionary<string, JsonElement>? ToDictionaryOfJSON(this byte[] bytes)
        {
            if (bytes.Length == 0)
                return null;

            return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(bytes);
        }

        private static List<Claim> AddPair(this List<Claim> claims, KeyValuePair<string, JsonElement> pair)
        {
            if (pair.Value.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in pair.Value.EnumerateArray())
                {
                    claims.Add(new Claim(pair.Key, item.GetValue()));
                }
            }
            else if (pair.Value.ValueKind != JsonValueKind.Null)
            {
                claims.Add(new Claim(pair.Key, pair.Value.GetValue()));
            }

            return claims;
        }

        private static string GetValue(this JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.String)
                return element.GetString() ?? string.Empty;

            return element.ToString();
        }
    }
}