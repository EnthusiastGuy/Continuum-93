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
        private static readonly byte[] POSITIONS = new byte[] { 103, 136, 192, 0 };

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

            // Draw registers
            for (int i = 0; i < 26; i++)
            {
                int y = startY + i * lineHeight;
                if (y + lineHeight > contentRect.Bottom)
                    break;

                // Draw register name and values (1-4 bit)
                for (int j = 0; j < 4; j++)
                {
                    int x = contentRect.X + Padding + POSITIONS[j];
                    string regName = RegistryUtils.GetNBitRegisterName(i, 1 + j);
                    string hexValue = RegistryUtils.GetHexValueForNBitRegister(i, 1 + j, regPage0);
                    string oldHexValue = RegistryUtils.GetHexValueForNBitRegister(i, 1 + j, regPageOld);
                    Color regColor = RegistryUtils.GetRegisterStateColor(i);

                    // Register name
                    ServiceGraphics.DrawText(
                        Fonts.ModernDOS_12x18_thin,
                        $"{regName}:",
                        x,
                        y,
                        contentRect.Width - Padding * 2,
                        regColor,
                        Color.Black,
                        (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline),
                        0xFF
                    );

                    // Hex value with change detection
                    int hexX = x + (regName.Length + 1) * charWidth;
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
                            (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline),
                            0xFF
                        );
                    }
                }

                // Draw separator
                ServiceGraphics.DrawText(
                    Fonts.ModernDOS_12x18_thin,
                    ">",
                    contentRect.X + Padding + 272,
                    startY + i * lineHeight,
                    contentRect.Width - Padding * 2,
                    Color.White,
                    Color.Black,
                    (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline),
                    0xFF
                );

                // Draw memory data pointed by register
                for (int j = 0; j < 6; j++)
                {
                    if (i * 16 + j < regMemoryData.Length)
                    {
                        int memX = contentRect.X + Padding + 281 + j * 17;
                        byte memValue = regMemoryData[i * 16 + j];
                        string hexMem = memValue.ToString("X2");

                        ServiceGraphics.DrawText(
                            Fonts.ModernDOS_12x18_thin,
                            hexMem,
                            memX,
                            startY + i * lineHeight,
                            contentRect.Width - Padding * 2,
                            new Color(40, 80, 160), // Memory byte color
                            Color.Black,
                            (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline),
                            0xFF
                        );
                    }
                }
            }
        }
    }
}

