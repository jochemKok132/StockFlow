using System.Globalization;
using System.Text;

namespace StockFlow.Shared.Common
{
    public static class Code128
    {
        private const int StartB = 104;
        private const int StartC = 105;
        private const int SwitchToB = 100;
        private const string Stop = "1100011101011";
        private const int QuietZone = 10;

        private static readonly string[] Patterns =
        [
            "11011001100", "11001101100", "11001100110", "10010011000", "10010001100", "10001001100", "10011001000", "10011000100",
            "10001100100", "11001001000", "11001000100", "11000100100", "10110011100", "10011011100", "10011001110", "10111001100",
            "10011101100", "10011100110", "11001110010", "11001011100", "11001001110", "11011100100", "11001110100", "11101101110",
            "11101001100", "11100101100", "11100100110", "11101100100", "11100110100", "11100110010", "11011011000", "11011000110",
            "11000110110", "10100011000", "10001011000", "10001000110", "10110001000", "10001101000", "10001100010", "11010001000",
            "11000101000", "11000100010", "10110111000", "10110001110", "10001101110", "10111011000", "10111000110", "10001110110",
            "11101110110", "11010001110", "11000101110", "11011101000", "11011100010", "11011101110", "11101011000", "11101000110",
            "11100010110", "11101101000", "11101100010", "11100011010", "11101111010", "11001000010", "11110001010", "10100110000",
            "10100001100", "10010110000", "10010000110", "10000101100", "10000100110", "10110010000", "10110000100", "10011010000",
            "10011000010", "10000110100", "10000110010", "11000010010", "11001010000", "11110111010", "11000010100", "10001111010",
            "10100111100", "10010111100", "10010011110", "10111100100", "10011110100", "10011110010", "11110100100", "11110010100",
            "11110010010", "11011011110", "11011110110", "11110110110", "10101111000", "10100011110", "10001011110", "10111101000",
            "10111100010", "11110101000", "11110100010", "10111011110", "10111101110", "11101011110", "11110101110", "11010000100",
            "11010010000", "11010011100",
        ];

        public static IReadOnlyList<int> Codes(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("A barcode needs a value.", nameof(value));

            var codes = new List<int>();
            bool digits = value.All(char.IsAsciiDigit);

            if (digits && value.Length >= 2)
            {
                codes.Add(StartC);
                int pairs = value.Length / 2 * 2;
                for (int i = 0; i < pairs; i += 2)
                    codes.Add(int.Parse(value.AsSpan(i, 2), CultureInfo.InvariantCulture));

                if (pairs < value.Length)
                {
                    codes.Add(SwitchToB);
                    codes.Add(value[^1] - 32);
                }
            }
            else
            {
                codes.Add(StartB);
                foreach (char character in value)
                {
                    if (character < 32 || character > 126)
                        throw new ArgumentException("A barcode can only contain printable ASCII characters.", nameof(value));
                    codes.Add(character - 32);
                }
            }

            int checksum = codes[0];
            for (int i = 1; i < codes.Count; i++)
                checksum += codes[i] * i;
            codes.Add(checksum % 103);

            return codes;
        }

        public static string Modules(string value)
        {
            var builder = new StringBuilder();
            foreach (int code in Codes(value))
                builder.Append(Patterns[code]);
            builder.Append(Stop);
            return builder.ToString();
        }

        public static int SvgWidth(string value, int moduleWidth = 2) =>
            (Modules(value).Length + QuietZone * 2) * moduleWidth;

        public static string ToSvg(string value, int moduleWidth = 2, int height = 60, string color = "#111111", string background = "#ffffff")
        {
            string modules = Modules(value);
            int width = (modules.Length + QuietZone * 2) * moduleWidth;

            var svg = new StringBuilder();
            svg.Append(CultureInfo.InvariantCulture, $"<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 {width} {height}\" width=\"{width}\" height=\"{height}\" shape-rendering=\"crispEdges\" role=\"img\" aria-label=\"Barcode {value}\">");
            svg.Append(CultureInfo.InvariantCulture, $"<rect width=\"{width}\" height=\"{height}\" fill=\"{background}\"/>");

            int i = 0;
            while (i < modules.Length)
            {
                if (modules[i] != '1') { i++; continue; }

                int start = i;
                while (i < modules.Length && modules[i] == '1') i++;

                svg.Append(CultureInfo.InvariantCulture, $"<rect x=\"{(start + QuietZone) * moduleWidth}\" width=\"{(i - start) * moduleWidth}\" height=\"{height}\" fill=\"{color}\"/>");
            }

            svg.Append("</svg>");
            return svg.ToString();
        }
    }
}
