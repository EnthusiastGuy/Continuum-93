using Continuum93.Emulator;
using Continuum93.ServiceModule.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Continuum93.ServiceModule.UI
{
    public class StacksWindow : Window
    {
        public StacksWindow(
            string title,
            int x, int y,
            int width, int height,
            float spawnDelaySeconds = 0,
            bool canResize = true,
            bool canClose = false)
            : base(title, x, y, width, height, spawnDelaySeconds, canResize, canClose)
        {
        }

        protected override void UpdateContent(GameTime gameTime)
        {
            Stacks.Update();
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            const int lineHeight = 18;
            const int charWidth = 13;
            var theme = ServiceGraphics.Theme;

            int startY = contentRect.Y + Padding;

            // Register stack label
            ServiceGraphics.DrawText(
                theme.PrimaryFont,
                "Regs",
                contentRect.X + Padding,
                startY,
                contentRect.Width - Padding * 2,
                theme.TextPrimary,
                theme.TextOutline,
                (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline),
                0xFF
            );

            // Draw register stack
            var regStack = Stacks.RegisterStack;
            for (int i = 0; i < regStack.Count && i < 120; i++)
            {
                int y = i / 20;
                int x = i % 20;
                int drawX = contentRect.X + Padding + 45 + x * 17;
                int drawY = startY + 20 + y * lineHeight;

                if (drawY + lineHeight > contentRect.Bottom)
                    break;

                ServiceGraphics.DrawText(
                    theme.PrimaryFont,
                    regStack[i],
                    drawX,
                    drawY,
                    contentRect.Width - Padding * 2,
                    theme.StackValueColor,
                    theme.TextOutline,
                    (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline),
                    0xFF
                );
            }

            // Call stack label
            int callStackStartY = startY + 92;
            ServiceGraphics.DrawText(
                theme.PrimaryFont,
                "Calls",
                contentRect.X + Padding,
                callStackStartY,
                contentRect.Width - Padding * 2,
                theme.TextPrimary,
                theme.TextOutline,
                (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline),
                0xFF
            );

            // Draw call stack
            var callStack = Stacks.CallStack;
            for (int i = 0; i < callStack.Count && i < 64; i++)
            {
                int y = i / 7;
                int x = i % 7;
                int drawX = contentRect.X + Padding + 45 + x * 49;
                int drawY = callStackStartY + 20 + y * lineHeight;

                if (drawY + lineHeight > contentRect.Bottom)
                    break;

                ServiceGraphics.DrawText(
                    theme.PrimaryFont,
                    callStack[i],
                    drawX,
                    drawY,
                    contentRect.Width - Padding * 2,
                    theme.StackValueColor,
                    theme.TextOutline,
                    (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline),
                    0xFF
                );
            }
        }
    }
}

