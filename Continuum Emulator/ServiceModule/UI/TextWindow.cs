using Continuum93.Emulator;          // Fonts, ServiceFontFlags
using Continuum93.ServiceModule;      // ServiceGraphics
using Microsoft.Xna.Framework;        // Rectangle, Color
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Continuum93.ServiceModule.UI
{
    public class TextWindow : Window
    {
        private Func<string> _textSource;

        public TextWindow(string title, Func<string> textSource, int x, int y, int width, int height, float spawnDelaySeconds, bool canResize, bool canClose)
            : base(title, x, y, width, height, spawnDelaySeconds, canResize, canClose)
        {
            //_text = text ?? string.Empty;
            _textSource = textSource;
        }

        /// <summary>
        /// Draws the window's content inside the content rectangle.
        /// Assumes the SpriteBatch is already in a Begin/End block
        /// (opened by Window.Draw).
        /// </summary>
        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            string text = _textSource?.Invoke() ?? string.Empty;
            var theme = ServiceGraphics.Theme;

            ServiceGraphics.DrawText(
                theme.PrimaryFont,
                text,
                contentRect.X + Padding,
                contentRect.Y + Padding,
                contentRect.Width - Padding * 2,
                theme.TextTitle,
                theme.TextOutline,
                (byte)(ServiceFontFlags.Wrap |
                       ServiceFontFlags.DrawOutline |
                       ServiceFontFlags.MonospaceCentering),
                0xFF
            );
        }

        // Optional: override if you want per-frame behavior (scrolling, cursor, etc.)
        // protected override void UpdateContent(GameTime gameTime) { }
    }
}
