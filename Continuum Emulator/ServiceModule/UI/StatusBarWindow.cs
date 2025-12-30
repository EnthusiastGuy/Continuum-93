using Continuum93.CodeAnalysis;
using Continuum93.Emulator;
using Continuum93.ServiceModule.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Continuum93.ServiceModule.UI
{
    public class StatusBarWindow : Window
    {
        private HistoryHoverPopup _hoverPopup;
        private float _hoverTimer = 0f;
        private float _closeTimer = 0f;
        private const float HoverDelay = 0.3f; // 300ms
        private const float CloseDelay = 0.5f; // 500ms delay before closing
        private Rectangle _historyArea;

        public HistoryHoverPopup HoverPopup => _hoverPopup;

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
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var mouse = Mouse.GetState();
            Point mousePos = new Point(mouse.X, mouse.Y);

            // Update hover popup
            UpdateHoverPopup(dt, mousePos);
            
            // Update popup directly if it exists
            if (_hoverPopup != null && _hoverPopup.Visible)
            {
                _hoverPopup.Update(gameTime);
            }
        }

        private void UpdateHoverPopup(float dt, Point mousePos)
        {
            // Hide pop-up if StatusBarWindow is not visible
            if (!Visible)
            {
                if (_hoverPopup != null && _hoverPopup.Visible)
                {
                    HideHoverPopup();
                }
                _hoverTimer = 0f;
                return;
            }

            // Check if mouse is over the pop-up (keep it open)
            bool mouseOverPopup = _hoverPopup != null && _hoverPopup.Visible && _hoverPopup.Bounds.Contains(mousePos);
            
            // Check if mouse is over history area
            bool mouseOverHistory = _historyArea.Contains(mousePos);
            
            // If hovering over history area or popup, reset close timer and increment hover timer
            if (mouseOverHistory || mouseOverPopup)
            {
                _closeTimer = 0f; // Reset close timer when mouse is over history or popup
                
                // If hovering over history area and not over pop-up, increment hover timer
                if (mouseOverHistory && !mouseOverPopup)
                {
                    _hoverTimer += dt;
                    
                    // Show pop-up after delay
                    if (_hoverTimer >= HoverDelay)
                    {
                        if (_hoverPopup == null || !_hoverPopup.Visible)
                        {
                            ShowHoverPopup(mousePos);
                        }
                        else
                        {
                            // Update pop-up data
                            UpdateHoverPopupData();
                        }
                    }
                }
                else if (mouseOverPopup)
                {
                    // Mouse is over popup, keep it open and reset hover timer
                    _hoverTimer = 0f;
                }
            }
            else
            {
                // Not hovering over history area or popup
                _hoverTimer = 0f; // Reset hover timer
                
                // Increment close timer
                if (_hoverPopup != null && _hoverPopup.Visible)
                {
                    _closeTimer += dt;
                    
                    // Hide pop-up after delay
                    if (_closeTimer >= CloseDelay)
                    {
                        HideHoverPopup();
                        _closeTimer = 0f;
                    }
                }
            }
        }

        private void ShowHoverPopup(Point mousePos)
        {
            // Get current instruction
            DissLine current = Disassembled.GetCurentInstruction();
            int? currentAddress = current?.Address;
            
            // Position pop-up above the history area, centered horizontally
            int popupWidth = 600;
            int popupHeight = 500;
            int popupX = _historyArea.X + (_historyArea.Width / 2) - (popupWidth / 2);
            int popupY = _historyArea.Y - popupHeight - 10; // Above the history area
            
            // Ensure pop-up stays on screen
            var device = Renderer.GetGraphicsDevice();
            if (popupX < 0)
                popupX = 10;
            if (popupX + popupWidth > device.Viewport.Width)
                popupX = device.Viewport.Width - popupWidth - 10;
            if (popupY < 0)
                popupY = _historyArea.Bottom + 10; // Show below if no room above
            
            if (_hoverPopup == null)
            {
                _hoverPopup = new HistoryHoverPopup(popupX, popupY, currentAddress);
            }
            else
            {
                _hoverPopup.X = popupX;
                _hoverPopup.Y = popupY;
                _hoverPopup.UpdateCurrentInstruction(currentAddress);
                _hoverPopup.ResetScrollToBottom(); // Reset scroll to bottom each time popup opens
                _hoverPopup.Visible = true;
            }
        }

        private void UpdateHoverPopupData()
        {
            if (_hoverPopup == null)
                return;
                
            DissLine current = Disassembled.GetCurentInstruction();
            int? currentAddress = current?.Address;
            _hoverPopup.UpdateCurrentInstruction(currentAddress);
        }

        private void HideHoverPopup()
        {
            if (_hoverPopup != null)
            {
                _hoverPopup.Visible = false;
            }
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            var theme = ServiceGraphics.Theme;
            var font = Fonts.ModernDOS_12x18;
            byte fontFlags = (byte)ServiceFontFlags.DrawOutline;

            // Get font metrics for proper alignment
            int charWidth = font.MeasureText("M", 0, fontFlags).width;
            int arrowWidth = font.MeasureText(">", 0, fontFlags).width;

            // Step-by-step mode status
            bool isConnected = Machine.COMPUTER != null;
            bool stepByStepMode = DebugState.StepByStep;
            Color stepByStepColor = stepByStepMode ? theme.TextGreenYellow : theme.VideoPaletteNumber;
            string stepByStepStatus = isConnected ? (stepByStepMode ? "CPU debugging" : "CPU running") : "CPU unknown";

            // Measure status label width for proper spacing
            int stepByStepStatusWidth = font.MeasureText(stepByStepStatus, 0, fontFlags).width;
            int statusLabelsWidth = stepByStepStatusWidth;

            // History label
            int historyLabelWidth = font.MeasureText("HISTORY", 0, fontFlags).width;
            int currentX = contentRect.X + Padding + historyLabelWidth + 10;
            int historyY = contentRect.Y + Padding;
            int historyAreaStartX = currentX;

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
            
            // Track history area for hover detection
            int fontHeight = font.GlyphCellHeight;
            const byte characterSpacing = 1;
            int lineHeight = fontHeight + characterSpacing;
            _historyArea = new Rectangle(
                historyAreaStartX,
                historyY,
                availableWidth,
                lineHeight
            );

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

            // Status indicator (right side) - CPU debugging/running status
            int statusX = contentRect.Right - Padding - stepByStepStatusWidth;
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



