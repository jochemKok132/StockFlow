namespace StockFlow.Shared.Common
{
    public static class ImageFormats
    {
        public const string Accept = "image/png,image/jpeg,image/gif,image/webp";

        private static readonly string[] ContentTypes = ["image/png", "image/jpeg", "image/gif", "image/webp"];

        public static bool IsAllowedContentType(string? contentType) =>
            contentType is not null && ContentTypes.Contains(contentType.ToLowerInvariant());

        public static bool IsImage(ReadOnlySpan<byte> bytes) => DetectContentType(bytes) is not null;

        public static string? DetectContentType(ReadOnlySpan<byte> bytes)
        {
            if (bytes.StartsWith(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))
                return "image/png";

            if (bytes.StartsWith(new byte[] { 0xFF, 0xD8, 0xFF }))
                return "image/jpeg";

            if (bytes.StartsWith("GIF87a"u8) || bytes.StartsWith("GIF89a"u8))
                return "image/gif";

            if (bytes.Length >= 12 && bytes[..4].SequenceEqual("RIFF"u8) && bytes[8..12].SequenceEqual("WEBP"u8))
                return "image/webp";

            return null;
        }
    }
}
