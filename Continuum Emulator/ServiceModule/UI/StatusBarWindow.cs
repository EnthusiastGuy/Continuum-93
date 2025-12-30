using Continuum93.CodeAnalysis;
using Continuum93.Emulator;
using Continuum93.ServiceModule.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
            var font = Fonts.ModernDOS_12x18;
            byte fontFlags = (byte)ServiceFontFlags.DrawOutline;

            // Get font metrics for proper alignment
            int charWidth = font.MeasureText("M", 0, fontFlags).width;
            int arrowWidth = font.MeasureText(">", 0, fontFlags).width;

            // Connection status
            bool isConnected = Machine.COMPUTER != null;
            Color connectionColor = isConnected ? theme.TextGreenYellow : theme.TextIndianRed;
            string connectedStatus = isConnected ? "Connected" : "No signal";

            // Step-by-step mode status
            bool stepByStepMode = DebugState.StepByStep;
            Color stepByStepColor = stepByStepMode ? theme.TextGreenYellow : theme.VideoPaletteNumber;
            string stepByStepStatus = isConnected ? (stepByStepMode ? "CPU debugging" : "CPU running") : "CPU unknown";

            // Measure status labels width for proper spacing
            int stepByStepStatusWidth = font.MeasureText(stepByStepStatus, 0, fontFlags).width;
            int connectedStatusWidth = font.MeasureText(connectedStatus, 0, fontFlags).width;
            int statusLabelsWidth = stepByStepStatusWidth + connectedStatusWidth + 20; // 20px spacing between labels

            // History label
            int historyLabelWidth = font.MeasureText("HISTORY", 0, fontFlags).width;
            int currentX = contentRect.X + Padding + historyLabelWidth + 10;
            int historyY = contentRect.Y + Padding;

            ServiceGraphics.DrawText(
                font,
                "HISTORY",
                contentRect.X + Padding,
                historyY,
                contentRect.Width - Padding * 2,
                theme.TextTitle,
                theme.TextOutline,
                fontFlags,
                0xFF
            );

            // Calculate available width for history (reserve space for status labels on the right)
            int availableWidth = Math.Max(0, contentRect.Width - Padding - currentX - statusLabelsWidth - Padding);

            // Get current instruction
            DissLine current = Disassembled.GetCurentInstruction();
            int? currentAddress = current?.Address;

            // Get history and filter out current instruction if it's already there (using actual font metrics)
            List<DissLine> latestHistory = StepHistory.GetFittingWidth(availableWidth, font, fontFlags, arrowWidth, currentAddress);
            
            // Draw history (red - already executed)
            for (int i = 0; i < latestHistory.Count; i++)
            {
                string instruction = latestHistory[i].Instruction ?? "";
                int instructionWidth = font.MeasureText(instruction, 0, fontFlags).width;
                
                ServiceGraphics.DrawText(
                    font,
                    instruction,
                    currentX,
                    historyY,
                    contentRect.Width - Padding * 2,
                    theme.VideoPaletteNumber,
                    theme.TextOutline,
                    fontFlags,
                    0xFF
                );
                currentX += instructionWidth;
                
                // Draw arrow separator
                ServiceGraphics.DrawText(
                    font,
                    ">",
                    currentX,
                    historyY,
                    contentRect.Width - Padding * 2,
                    theme.VideoPaletteNumber,
                    theme.TextOutline,
                    fontFlags,
                    0xFF
                );
                currentX += arrowWidth + 10; // 10px spacing after arrow
            }

            // Draw current instruction (orange - next to execute) only if it's not already in history
            if (current != null && (latestHistory.Count == 0 || latestHistory[latestHistory.Count - 1].Address != current.Address))
            {
                string instruction = current.Instruction ?? "";
                ServiceGraphics.DrawText(
                    font,
                    instruction,
                    currentX,
                    historyY,
                    contentRect.Width - Padding * 2,
                    theme.TextDarkOrange,
                    theme.TextOutline,
                    fontFlags,
                    0xFF
                );
            }

            // Status indicators (right side) - use actual measured widths
            int statusX = contentRect.Right - Padding - connectedStatusWidth;
            ServiceGraphics.DrawText(
                font,
                connectedStatus,
                statusX,
                historyY,
                contentRect.Width - Padding * 2,
                connectionColor,
                theme.TextOutline,
                fontFlags,
                0xFF
            );

            statusX -= stepByStepStatusWidth + 20; // 20px spacing
            ServiceGraphics.DrawText(
                font,
                stepByStepStatus,
                statusX,
                historyY,
                contentRect.Width - Padding * 2,
                stepByStepColor,
                theme.TextOutline,
                fontFlags,
                0xFF
            );
        }
    }
}



