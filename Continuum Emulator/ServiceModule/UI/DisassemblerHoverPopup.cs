using Continuum93.Emulator.AutoDocs.MetaInfo;
using Continuum93.ServiceModule;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Continuum93.ServiceModule.UI
{
    public class DisassemblerHoverPopup : Window
    {
        private string _instructionName;
        private string _fullInstruction;
        private string _opcodes;
        private ASMMeta _meta;
        private int _scrollOffset = 0;
        private int _maxScrollOffset = 0;
        private const int PopupWidth = 500;
        private const int PopupHeight = 400;

        public DisassemblerHoverPopup(int x, int y, string instructionName, string fullInstruction, string opcodes)
            : base(instructionName, x, y, PopupWidth, PopupHeight, 0f, false, false)
        {
            _instructionName = instructionName;
            _fullInstruction = fullInstruction;
            _opcodes = opcodes;
            _meta = ASMMetaInfo.GetMeta(instructionName);
            IsOnTop = true;
            // Set height immediately to full size (no spawn animation)
            Height = PopupHeight;
            _prevMouseState = Mouse.GetState(); // Initialize previous mouse state
        }
        
        // Override Update to skip spawn animation - pop-up should appear immediately
        public override void Update(GameTime gameTime)
        {
            if (!Visible) return;
            // Skip spawn animation - just update content
            UpdateContent(gameTime);
        }

        protected override void UpdateContent(GameTime gameTime)
        {
            // Update previous mouse state for scroll handling
            _prevMouseState = Mouse.GetState();
        }
        
        private MouseState _prevMouseState;

        public void UpdateData(string instructionName, string fullInstruction, string opcodes)
        {
            _instructionName = instructionName;
            _fullInstruction = fullInstruction;
            _opcodes = opcodes;
            _meta = ASMMetaInfo.GetMeta(instructionName);
            Title = instructionName;
            _scrollOffset = 0;
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            var theme = ServiceGraphics.Theme;
            var font = Fonts.ModernDOS_9x15;
            byte monoFlags = (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline);
            byte normalFlags = (byte)(ServiceFontFlags.DrawOutline);
            
            int lineHeight = 18;
            int sectionSpacing = 10;
            int contentWidth = contentRect.Width - Padding * 2;
            
            // Calculate content height first (without scroll offset) to determine max scroll
            int tempY = contentRect.Y + Padding;
            int startY = tempY;

            // First pass: Calculate total content height (without drawing)
            void MeasureSectionHeader()
            {
                tempY += lineHeight;
            }

            void MeasureMonoText(string text)
            {
                tempY += lineHeight;
            }

            void MeasureWrappedText(string text)
            {
                if (string.IsNullOrEmpty(text))
                {
                    tempY += lineHeight;
                    return;
                }

                // Simple word wrapping measurement
                string[] words = text.Split(' ');
                string currentLine = "";
                
                foreach (string word in words)
                {
                    string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                    var size = font.MeasureText(testLine, contentWidth, normalFlags);
                    
                    if (size.width > contentWidth && !string.IsNullOrEmpty(currentLine))
                    {
                        tempY += lineHeight;
                        currentLine = word;
                    }
                    else
                    {
                        currentLine = testLine;
                    }
                }
                
                if (!string.IsNullOrEmpty(currentLine))
                {
                    tempY += lineHeight;
                }
            }

            // Measure all content
            MeasureSectionHeader(); // "Instruction:"
            MeasureMonoText(_fullInstruction);
            tempY += sectionSpacing;
            MeasureSectionHeader(); // "Opcodes:"
            MeasureMonoText(_opcodes);
            tempY += sectionSpacing;

            if (_meta != null)
            {
                MeasureSectionHeader(); // "Description:"
                MeasureWrappedText(_meta.Description);
                tempY += sectionSpacing;
                MeasureSectionHeader(); // "Application:"
                MeasureWrappedText(_meta.Application);
                tempY += sectionSpacing;
                MeasureSectionHeader(); // "Operators:"
                MeasureWrappedText(_meta.Format);
            }

            // Calculate max scroll offset - stop when last line is fully visible at bottom
            int totalContentHeight = tempY - startY;
            int visibleHeight = contentRect.Height;
            // Add extra padding (one line height) to ensure last line is fully visible
            _maxScrollOffset = Math.Max(0, totalContentHeight - visibleHeight + lineHeight);
            
            // Clamp scroll offset to valid range now that we know the max
            _scrollOffset = Math.Max(0, Math.Min(_scrollOffset, _maxScrollOffset));

            // Second pass: Draw content with scroll offset applied
            int y = contentRect.Y + Padding - _scrollOffset;

            // Helper to draw a section header
            void DrawSectionHeader(string header)
            {
                if (y >= contentRect.Y - lineHeight && y < contentRect.Bottom)
                {
                    ServiceGraphics.DrawText(
                        font,
                        header,
                        contentRect.X + Padding,
                        y,
                        contentWidth,
                        theme.TextPrimary,
                        theme.TextOutline,
                        monoFlags,
                        0xFF
                    );
                }
                y += lineHeight;
            }

            // Helper to draw monospaced text
            void DrawMonoText(string text, Color color)
            {
                if (y >= contentRect.Y - lineHeight && y < contentRect.Bottom)
                {
                    ServiceGraphics.DrawText(
                        font,
                        text,
                        contentRect.X + Padding,
                        y,
                        contentWidth,
                        color,
                        theme.TextOutline,
                        monoFlags,
                        0xFF
                    );
                }
                y += lineHeight;
            }

            // Helper to draw wrapped non-monospaced text
            void DrawWrappedText(string text, Color color)
            {
                if (string.IsNullOrEmpty(text))
                {
                    y += lineHeight;
                    return;
                }

                // Simple word wrapping
                string[] words = text.Split(' ');
                string currentLine = "";
                
                foreach (string word in words)
                {
                    string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                    var size = font.MeasureText(testLine, contentWidth, normalFlags);
                    
                    if (size.width > contentWidth && !string.IsNullOrEmpty(currentLine))
                    {
                        // Draw current line
                        if (y >= contentRect.Y - lineHeight && y < contentRect.Bottom)
                        {
                            ServiceGraphics.DrawText(
                                font,
                                currentLine,
                                contentRect.X + Padding,
                                y,
                                contentWidth,
                                color,
                                theme.TextOutline,
                                normalFlags,
                                0xFF
                            );
                        }
                        y += lineHeight;
                        currentLine = word;
                    }
                    else
                    {
                        currentLine = testLine;
                    }
                }
                
                // Draw remaining text
                if (!string.IsNullOrEmpty(currentLine))
                {
                    if (y >= contentRect.Y - lineHeight && y < contentRect.Bottom)
                    {
                        ServiceGraphics.DrawText(
                            font,
                            currentLine,
                            contentRect.X + Padding,
                            y,
                            contentWidth,
                            color,
                            theme.TextOutline,
                            normalFlags,
                            0xFF
                        );
                    }
                    y += lineHeight;
                }
            }

            // Draw instruction sections
            DrawSectionHeader("Instruction:");
            DrawMonoText(_fullInstruction, theme.DisassemblerInstructionColor);
            y += sectionSpacing;

            DrawSectionHeader("Opcodes:");
            DrawMonoText(_opcodes, theme.DisassemblerOpcodeColor);
            y += sectionSpacing;

            if (_meta != null)
            {
                DrawSectionHeader("Description:");
                DrawWrappedText(_meta.Description, theme.TextSecondary);
                y += sectionSpacing;

                DrawSectionHeader("Application:");
                DrawWrappedText(_meta.Application, theme.TextSecondary);
                y += sectionSpacing;

                DrawSectionHeader("Operators:");
                DrawWrappedText(_meta.Format, theme.TextSecondary);
            }

            // Draw overflow indicator (+) at bottom if content is scrollable
            if (_maxScrollOffset > 0 && _scrollOffset < _maxScrollOffset)
            {
                string overflowIndicator = "+";
                int indicatorY = contentRect.Bottom - lineHeight - Padding;
                ServiceGraphics.DrawText(
                    font,
                    overflowIndicator,
                    contentRect.X + contentRect.Width / 2 - 5,
                    indicatorY,
                    contentWidth,
                    theme.TextPrimary,
                    theme.TextOutline,
                    monoFlags,
                    0xFF
                );
            }
        }

        // Override to prevent input capture - pop-up should not be interactive except for scrolling
        public override bool HandleInput(MouseState mouse, MouseState prevMouse)
        {
            // Only handle scroll wheel input if mouse is over the popup
            if (Bounds.Contains(mouse.Position))
            {
                int scrollDelta = mouse.ScrollWheelValue - prevMouse.ScrollWheelValue;
                if (scrollDelta != 0)
                {
                    // Scroll speed: each notch (120 units) moves 20 pixels
                    // Positive scrollDelta (scrolling down) should decrease scrollOffset (scroll content down)
                    _scrollOffset -= scrollDelta / 120 * 20;
                    // Clamp scroll offset to valid range (use a large temporary max if not calculated yet)
                    int maxScroll = _maxScrollOffset > 0 ? _maxScrollOffset : 10000;
                    _scrollOffset = Math.Max(0, Math.Min(_scrollOffset, maxScroll));
                    return true; // Consume scroll input
                }
            }
            return false; // Don't capture other input
        }
    }
}

