
using System.IO;
using System.Text.Json;

namespace Continuum93.ServiceModule.Themes
{
    public static class ThemeLoader
    {
        public static ColorTheme Load(string path)
        {
            string json = File.ReadAllText(path);

            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = ColorThemeJsonContext.Default
            };

            var data = JsonSerializer.Deserialize<ColorThemeData>(json, options);

            return new ColorTheme
            {
                Background = ColorParser.Parse(data.Background),
                Text = ColorParser.Parse(data.Text),
                Outline = ColorParser.Parse(data.Outline),
                Highlight = ColorParser.Parse(data.Highlight)
            };
        }
    }
}
