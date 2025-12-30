using Continuum93.CodeAnalysis;
using Continuum93.ServiceModule;
using Continuum93.ServiceModule.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Continuum93.ServiceModule.UI
{
    public class HistoryHoverPopup : Window
    {
        private int _scrollOffset = 0;
        private int _maxScrollOffset = 0;
        private MouseState _prevMouseState;
        private const int PopupWidth = 600;
        private const int PopupHeight = 500;
        private int? _currentInstructionAddress;
        private bool _wasVisible = false;
        private bool _shouldScrollToBottom = false;

        public HistoryHoverPopup(int x, int y, int? currentInstructionAddress)
            : base("Instruction History", x, y, PopupWidth, PopupHeight, 0f, false, false)
        {
            _currentInstructionAddress = currentInstructionAddress;
            IsOnTop = true;
            Height = PopupHeight;
            _prevMouseState = Mouse.GetState();
            _shouldScrollToBottom = true; // Start scrolled to bottom when first created
        }

        public override void Update(GameTime gameTime)
        {
            // Reset scroll to bottom when popup first becomes visible
            if (Visible && !_wasVisible)
            {
                _shouldScrollToBottom = true;
            }
            _wasVisible = Visible;
            
            if (!Visible) return;
            UpdateContent(gameTime);
        }

        protected override void UpdateContent(GameTime gameTime)
        {
            // Handle scrolling
            var mouse = Mouse.GetState();
            
            if (ContentRect.Contains(new Point(mouse.X, mouse.Y)))
            {
                int scrollDelta = mouse.ScrollWheelValue - _prevMouseState.ScrollWheelValue;
                if (scrollDelta != 0)
                {
                    int scrollStep = Math.Abs(scrollDelta) / 120;
                    if (scrollDelta > 0)
                    {
                        _scrollOffset = Math.Max(0, _scrollOffset - scrollStep);
                    }
                    else
                    {
                        _scrollOffset = Math.Min(_maxScrollOffset, _scrollOffset + scrollStep);
                    }
                }
            }
            
            _prevMouseState = mouse;
        }

        public void UpdateCurrentInstruction(int? currentInstructionAddress)
        {
            _currentInstructionAddress = currentInstructionAddress;
        }

        public void ResetScrollToBottom()
        {
            _scrollOffset = int.MaxValue; // Will be set to actual max in DrawContent
            _shouldScrollToBottom = true;
        }

        public override bool HandleInput(MouseState mouse, MouseState prevMouse)
        {
            // Allow scrolling but don't capture other input
            return false;
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            var theme = ServiceGraphics.Theme;
            var font = Fonts.ModernDOS_12x18;
            byte fontFlags = (byte)ServiceFontFlags.DrawOutline;
            
            int fontHeight = font.GlyphCellHeight;
            const byte characterSpacing = 1;
            int lineHeight = fontHeight + characterSpacing;
            
            // Get all history (oldest to newest)
            var allHistory = StepHistory.GetAllHistory();
            
            // Get current instruction to check if it's the last one in history
            DissLine current = null;
            if (_currentInstructionAddress.HasValue)
            {
                current = Disassembled.GetCurentInstruction();
            }
            
            // Build display list: history (oldest to newest)
            // The last item is the current instruction (in progress) - mark it as such
            List<(DissLine item, bool isCurrent)> displayList = new List<(DissLine, bool)>();
            for (int i = 0; i < allHistory.Count; i++)
            {
                bool isCurrentItem = false;
                // Check if this is the last item and matches current instruction
                if (i == allHistory.Count - 1 && current != null && allHistory[i].Address == current.Address)
                {
                    isCurrentItem = true;
                }
                displayList.Add((allHistory[i], isCurrentItem));
            }
            
            // If current instruction is not in history, add it at the end
            if (current != null && (allHistory.Count == 0 || allHistory[allHistory.Count - 1].Address != current.Address))
            {
                displayList.Add((current, true));
            }
            
            int totalItems = displayList.Count;
            
            // Calculate how many lines fit
            int visibleHeight = contentRect.Height - Padding * 2;
            int linesPerPage = Math.Max(1, visibleHeight / lineHeight);
            
            // Update max scroll offset (scroll from top, showing newest at bottom by default)
            _maxScrollOffset = Math.Max(0, totalItems - linesPerPage);
            
            // Scroll to bottom when popup first becomes visible
            // This ensures the latest instruction (at bottom) is visible when popup first appears
            if (_shouldScrollToBottom)
            {
                _scrollOffset = _maxScrollOffset;
                _shouldScrollToBottom = false;
            }
            
            _scrollOffset = MathHelper.Clamp(_scrollOffset, 0, _maxScrollOffset);
            
            // Calculate which items to show (start from scroll offset)
            int startIndex = _scrollOffset;
            int endIndex = Math.Min(startIndex + linesPerPage, totalItems);
            
            int y = contentRect.Y + Padding;
            
            // Draw items (oldest to newest, with current at bottom in orange)
            for (int i = startIndex; i < endIndex; i++)
            {
                var (item, isCurrentItem) = displayList[i];
                string instruction = item.Instruction ?? "";
                // Last instruction (currently in progress) is orange, all others are red
                Color itemColor = isCurrentItem ? theme.TextDarkOrange : theme.VideoPaletteNumber;
                
                ServiceGraphics.DrawText(
                    font,
                    instruction,
                    contentRect.X + Padding,
                    y,
                    contentRect.Width - Padding * 2,
                    itemColor,
                    theme.TextOutline,
                    fontFlags,
                    0xFF
                );
                
                y += lineHeight;
            }
        }
    }
}

