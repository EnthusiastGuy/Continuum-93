using Continuum93.Emulator;
using Continuum93.ServiceModule.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Continuum93.ServiceModule.UI
{
    public class RegisterWindow : Window
    {
        private const int ColumnSpacing = 20; // Spacing between columns

        public RegisterWindow(
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
            CPUState.Update();
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            const int lineHeight = 18;
            const int charWidth = 13;

            byte[] regPage0 = CPUState.RegPage0;
            byte[] regPageOld = CPUState.RegPageOld;
            byte regPageIndex = CPUState.RegPageIndex;
            byte[] regMemoryData = CPUState.RegMemoryData;

            // Title
            ServiceGraphics.DrawText(
                Fonts.ModernDOS_12x18,
                $"Register view (bank: {regPageIndex})",
                contentRect.X + Padding,
                contentRect.Y + Padding,
                contentRect.Width - Padding * 2,
                Color.Yellow,
                Color.Black,
                (byte)ServiceFontFlags.DrawOutline,
                0xFF
            );

            int startY = contentRect.Y + Padding + 24;
            byte fontFlags = (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline);

            // width of "FF" with the active font rules (monospace + spacing + outline pad)
            int byteCellWidth = Fonts.ModernDOS_12x18_thin.MeasureText("FF", 0, fontFlags).width;

            // extra gap between bytes so it doesn’t look cramped
            const int byteGap = 4;
            int byteStride = byteCellWidth + byteGap;

            // Draw registers
            for (int i = 0; i < 26; i++)
            {
                int y = startY + i * lineHeight;
                if (y + lineHeight > contentRect.Bottom)
                    break;

                // Draw register name and values (1-4 bit)
                int currentX = contentRect.X + Padding;
                for (int j = 0; j < 4; j++)
                {
                    string regName = RegistryUtils.GetNBitRegisterName(i, 1 + j);
                    string hexValue = RegistryUtils.GetHexValueForNBitRegister(i, 1 + j, regPage0);
                    string oldHexValue = RegistryUtils.GetHexValueForNBitRegister(i, 1 + j, regPageOld);
                    Color regColor = RegistryUtils.GetRegisterStateColor(i);

                    // Measure the full column text (register name + ":" + hex value")
                    string fullColumnText = $"{regName}:{hexValue}";
                    int columnWidth = Fonts.ModernDOS_12x18_thin.MeasureText(
                        fullColumnText,
                        contentRect.Width - Padding * 2,
                        fontFlags
                    ).width;

                    // Register name
                    ServiceGraphics.DrawText(
                        Fonts.ModernDOS_12x18_thin,
                        $"{regName}:",
                        currentX,
                        y,
                        contentRect.Width + Padding * 9,
                        regColor,
                        Color.Black,
                        fontFlags,
                        0xFF
                    );

                    // Hex value with change detection
                    string regNameWithColon = $"{regName}:";
                    int regNameWidth = Fonts.ModernDOS_12x18_thin.MeasureText(
                        regNameWithColon,
                        contentRect.Width - Padding * 2,
                        fontFlags
                    ).width;
                    int hexX = currentX + regNameWidth;
                    bool valueChanged = hexValue != oldHexValue;
                    Color hexColor = valueChanged ? Color.DarkOrange : Color.White;

                    // Draw hex value with leading zero transparency
                    bool nonZero = false;
                    for (int k = 0; k < hexValue.Length; k++)
                    {
                        if (hexValue[k] != '0')
                            nonZero = true;

                        Color charColor = hexColor;
                        if (!nonZero && hexValue[k] == '0')
                            charColor = new Color(80, 160, 255, 22); // transparent blue

                        if (k < oldHexValue.Length && hexValue[k] != oldHexValue[k])
                            charColor = Color.DarkOrange;

                        ServiceGraphics.DrawText(
                            Fonts.ModernDOS_12x18_thin,
                            hexValue[k].ToString(),
                            hexX + k * charWidth,
                            y,
                            contentRect.Width - Padding * 2,
                            charColor,
                            Color.Black,
                            fontFlags,
                            0xFF
                        );
                    }

                    // Update X position for next column: current position + column width + spacing
                    currentX += columnWidth + ColumnSpacing;
                }

                // Draw separator
                ServiceGraphics.DrawText(
                    Fonts.ModernDOS_12x18_thin,
                    ">",
                    contentRect.X + Padding + 530,
                    startY + i * lineHeight,
                    contentRect.Width - Padding * 2,
                    Color.White,
                    Color.Black,
                    (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline),
                    0xFF
                );




                // Draw memory data pointed by register
                int memBaseX = contentRect.X + Padding + 550;

                for (int j = 0; j < 6; j++)
                {
                    int idx = i * 16 + j;
                    if (idx >= regMemoryData.Length)
                        break;

                    int memX = memBaseX + j * byteStride;
                    string hexMem = regMemoryData[idx].ToString("X2");

                    ServiceGraphics.DrawText(
                        Fonts.ModernDOS_12x18_thin,
                        hexMem,
                        memX,
                        startY + i * lineHeight,
                        contentRect.Width - Padding * 2,
                        new Color(40, 80, 160),
                        Color.Black,
                        fontFlags,
                        0xFF
                    );
                }
            }
        }
    }
}

