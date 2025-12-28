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
            var theme = ServiceGraphics.Theme;

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
                theme.TextTitle,
                theme.TextOutline,
                (byte)ServiceFontFlags.DrawOutline,
                0xFF
            );

            int startY = contentRect.Y + Padding + 24;
            byte fontFlags = (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline);

            // width of "FF" with the active font rules (monospace + spacing + outline pad)
            int byteCellWidth = theme.PrimaryFont.MeasureText("FF", 0, fontFlags).width;

            // extra gap between bytes so it doesn�t look cramped
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
                    int colPosition = (j + 3) % 4;  // Starting with the 32 bit register column, then 8, 16 and finally
                    // 24 bit, so we can represent them as address:value pairs more naturally

                    string regName = RegistryUtils.GetNBitRegisterName(i, 1 + colPosition);
                    string hexValue = RegistryUtils.GetHexValueForNBitRegister(i, 1 + colPosition, regPage0);
                    string oldHexValue = RegistryUtils.GetHexValueForNBitRegister(i, 1 + colPosition, regPageOld);
                    Color regColor = RegistryUtils.GetRegisterStateColor(i);

                    // Measure the full column text (register name + ":" + hex value")
                    string fullColumnText = $"{regName}:{hexValue}";
                    int columnWidth = theme.PrimaryFont.MeasureText(
                        fullColumnText,
                        contentRect.Width - Padding * 2,
                        fontFlags
                    ).width;

                    // Register name
                    ServiceGraphics.DrawText(
                        theme.PrimaryFont,
                        $"{regName}:",
                        currentX,
                        y,
                        contentRect.Width + Padding * 9,
                        regColor,
                        theme.TextOutline,
                        fontFlags,
                        0xFF
                    );

                    // Hex value with change detection
                    string regNameWithColon = $"{regName}:";
                    int regNameWidth = theme.PrimaryFont.MeasureText(
                        regNameWithColon,
                        contentRect.Width - Padding * 2,
                        fontFlags
                    ).width;
                    int hexX = currentX + regNameWidth;
                    bool valueChanged = hexValue != oldHexValue;
                    Color hexColor = valueChanged ? theme.RegisterValueChangedColor : theme.RegisterValueUnchangedColor;

                    // Draw hex value with leading zero transparency
                    bool nonZero = false;
                    for (int k = 0; k < hexValue.Length; k++)
                    {
                        if (hexValue[k] != '0')
                            nonZero = true;

                        Color charColor = hexColor;
                        if (!nonZero && hexValue[k] == '0')
                            charColor = theme.NumberLeadingZeroes;

                        if (k < oldHexValue.Length && hexValue[k] != oldHexValue[k])
                            charColor = theme.RegisterValueChangedColor;

                        ServiceGraphics.DrawText(
                            theme.PrimaryFont,
                            hexValue[k].ToString(),
                            hexX + k * charWidth,
                            y,
                            contentRect.Width - Padding * 2,
                            charColor,
                            theme.TextOutline,
                            fontFlags,
                            0xFF
                        );
                    }

                    // Update X position for next column: current position + column width + spacing
                    currentX += columnWidth + ColumnSpacing;
                }

                // Draw separator
                ServiceGraphics.DrawText(
                    theme.PrimaryFont,
                    ">",
                    contentRect.X + Padding + 530,
                    startY + i * lineHeight,
                    contentRect.Width - Padding * 2,
                    theme.TextPrimary,
                    theme.TextOutline,
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
                        theme.PrimaryFont,
                        hexMem,
                        memX,
                        startY + i * lineHeight,
                        contentRect.Width - Padding * 2,
                        theme.RegisterMemoryDataColor,
                        theme.TextOutline,
                        fontFlags,
                        0xFF
                    );
                }
            }
        }
    }
}

