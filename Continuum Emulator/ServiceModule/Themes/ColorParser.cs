using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace Continuum93.ServiceModule.Themes
{
    public static class ColorParser
    {
        public static Color Parse(string value)
        {
            value = value.Trim();

            // #RRGGBB or #RRGGBBAA
            if (value.StartsWith("#"))
            {
                return ParseHex(value);
            }

            // Named color: "White", "Black", etc.
            var named = typeof(Color).GetProperty(value);
            if (named != null)
            {
                return (Color)named.GetValue(null);
            }

            // Comma format: "R,G,B" or "R,G,B,A"
            if (value.Contains(","))
            {
                return ParseRGBA(value);
            }

            throw new Exception($"Unknown color format: {value}");
        }

        private static Color ParseHex(string hex)
        {
            hex = hex.TrimStart('#');

            if (hex.Length == 6) // RRGGBB
                return new Color(
                    byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber)
                );

            if (hex.Length == 8) // RRGGBBAA
                return new Color(
                    byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber)
                );

            throw new Exception("Invalid hex color format.");
        }

        private static Color ParseRGBA(string rgba)
        {
            var parts = rgba.Split(',').Select(p => byte.Parse(p)).ToArray();

            if (parts.Length == 3)
                return new Color(parts[0], parts[1], parts[2]);

            if (parts.Length == 4)
                return new Color(parts[0], parts[1], parts[2], parts[3]);

            throw new Exception("Invalid RGBA format.");
        }

        /// <summary>
        ///     Formats a Color object back into a string representation.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string Format(Color color)
        {
            // Try to match named colors first
            var colorType = typeof(Color);
            var properties = colorType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            
            foreach (var prop in properties)
            {
                if (prop.PropertyType == typeof(Color))
                {
                    var namedColor = (Color)prop.GetValue(null);
                    if (namedColor == color)
                    {
                        return prop.Name;
                    }
                }
            }

            // If not a named color, use RGBA format
            if (color.A == 255)
            {
                return $"{color.R},{color.G},{color.B}";
            }
            else
            {
                return $"{color.R},{color.G},{color.B},{color.A}";
            }
        }
    }

}
