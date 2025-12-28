using Continuum93.Emulator;
using Continuum93.ServiceModule.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Continuum93.ServiceModule.UI
{
    public class StatusBarWindow : Window
    {
        public StatusBarWindow(
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
            // Status bar updates automatically
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            var theme = ServiceGraphics.Theme;

            // Connection status
            bool isConnected = Machine.COMPUTER != null;
            Color connectionColor = isConnected ? theme.TextGreenYellow : theme.TextIndianRed;
            string connectedStatus = isConnected ? "Connected" : "No signal";

            // Step-by-step mode status (TODO: implement step-by-step mode tracking)
            bool stepByStepMode = false; // TODO: get from actual state
            Color stepByStepColor = stepByStepMode ? theme.TextGreenYellow : theme.VideoPaletteNumber;
            string stepByStepStatus = isConnected ? (stepByStepMode ? "CPU debugging" : "CPU running") : "CPU unknown";

            // History
            ServiceGraphics.DrawText(
                Fonts.ModernDOS_12x18,
                "HISTORY",
                contentRect.X + Padding,
                contentRect.Y + Padding,
                contentRect.Width - Padding * 2,
                theme.TextTitle,
                theme.TextOutline,
                (byte)ServiceFontFlags.DrawOutline,
                0xFF
            );

            int currentX = contentRect.X + Padding + 64;
            int historyY = contentRect.Y + Padding;

            // Show History
            List<DissLine> latestHistory = StepHistory.GetAtMost(6);
            for (int i = 0; i < latestHistory.Count; i++)
            {
                string instruction = latestHistory[i].Instruction ?? "";
                int width = instruction.Length * 13 + 4; // Approximate width
                ServiceGraphics.DrawText(
                    Fonts.ModernDOS_12x18,
                    instruction,
                    currentX,
                    historyY,
                    contentRect.Width - Padding * 2,
                    theme.VideoPaletteNumber,
                    theme.TextOutline,
                    (byte)ServiceFontFlags.DrawOutline,
                    0xFF
                );
                currentX += width + 2;
                ServiceGraphics.DrawText(
                    Fonts.ModernDOS_12x18,
                    ">",
                    currentX,
                    historyY,
                    contentRect.Width - Padding * 2,
                    theme.VideoPaletteNumber,
                    theme.TextOutline,
                    (byte)ServiceFontFlags.DrawOutline,
                    0xFF
                );
                currentX += 8;
            }

            // Current instruction
            DissLine current = Disassembled.GetCurentInstruction();
            if (current != null)
            {
                string instruction = current.Instruction ?? "";
                ServiceGraphics.DrawText(
                    Fonts.ModernDOS_12x18,
                    instruction,
                    currentX,
                    historyY,
                    contentRect.Width - Padding * 2,
                    theme.TextDarkOrange,
                    theme.TextOutline,
                    (byte)ServiceFontFlags.DrawOutline,
                    0xFF
                );
            }

            // Status indicators (right side)
            int statusX = contentRect.Right - Padding - 200;
            ServiceGraphics.DrawText(
                Fonts.ModernDOS_12x18,
                stepByStepStatus,
                statusX,
                historyY,
                contentRect.Width - Padding * 2,
                stepByStepColor,
                theme.TextOutline,
                (byte)ServiceFontFlags.DrawOutline,
                0xFF
            );

            statusX = contentRect.Right - Padding - 100;
            ServiceGraphics.DrawText(
                Fonts.ModernDOS_12x18,
                connectedStatus,
                statusX,
                historyY,
                contentRect.Width - Padding * 2,
                connectionColor,
                theme.TextOutline,
                (byte)ServiceFontFlags.DrawOutline,
                0xFF
            );
        }
    }
}



