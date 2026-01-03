using Continuum93.Emulator;
using Continuum93.ServiceModule.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Continuum93.ServiceModule.UI
{
    public class StacksWindow : Window
    {
        private int _regStackScrollOffset = 0;
        private int _callStackScrollOffset = 0;
        private int _previousRegScrollValue = 0;
        private int _previousCallScrollValue = 0;
        private const int MaxVisibleValues = 40;
        
        // Stack capacities in bytes
        private const uint RegStackCapacityBytes = 4 * 1024 * 1024;  // 4MB
        private const uint CallStackCapacityEntries = 1024 * 1024;   // 1MB entries
        
        // Track maximum occupancy values
        private double _maxRegStackOccupancy = 0.0;
        private double _maxCallStackOccupancy = 0.0;

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
            
            // Calculate occupancy percentages based on actual stack pointer values
            if (Machine.COMPUTER != null)
            {
                var cpu = Machine.COMPUTER.CPU;
                uint spr = cpu.REGS.SPR;  // Register stack pointer in bytes
                uint spc = cpu.REGS.SPC;  // Call stack pointer in entries
                
                // Calculate register stack occupancy (SPR is in bytes)
                double regStackOccupancy = (spr / (double)RegStackCapacityBytes) * 100.0;
                
                // Calculate call stack occupancy (SPC is in entries)
                double callStackOccupancy = (spc / (double)CallStackCapacityEntries) * 100.0;
                
                // Update maximum values
                if (regStackOccupancy > _maxRegStackOccupancy)
                    _maxRegStackOccupancy = regStackOccupancy;
                if (callStackOccupancy > _maxCallStackOccupancy)
                    _maxCallStackOccupancy = callStackOccupancy;
                
                // Update title with occupancy percentages
                Title = $"STACKS: {regStackOccupancy:F3}% (Max: {_maxRegStackOccupancy:F3}%) {callStackOccupancy:F3}% (Max: {_maxCallStackOccupancy:F3}%)";
            }
            else
            {
                Title = "STACKS: 0.000% (Max: 0.000%) 0.000% (Max: 0.000%)";
            }

            // Handle mouse wheel scrolling
            var mouse = Mouse.GetState();
            Point mousePos = new Point(mouse.X, mouse.Y);

            if (ContentRect.Contains(mousePos))
            {
                // Determine which stack area the mouse is in
                var theme = ServiceGraphics.Theme;
                byte fontFlags = (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline);
                int fontHeight = theme.PrimaryFont.GlyphCellHeight;
                const byte characterSpacing = 1;
                int lineHeight = fontHeight + characterSpacing;
                
                int startY = ContentRect.Y + Padding;
                int labelHeight = lineHeight;
                int regStackAreaStart = startY + labelHeight;
                int regStackAreaEnd = regStackAreaStart + MaxVisibleValues * lineHeight;
                
                // Check if mouse is in register stack area
                if (mousePos.Y >= regStackAreaStart && mousePos.Y < regStackAreaEnd)
                {
                    int scrollDelta = mouse.ScrollWheelValue - _previousRegScrollValue;
                    if (scrollDelta != 0)
                    {
                        var regStack = Stacks.RegisterStack;
                        int totalValues = regStack.Count;
                        if (totalValues > MaxVisibleValues)
                        {
                            // Scroll: positive delta = scroll up (show older values), negative = scroll down (show newer values)
                            int scrollStep = Math.Abs(scrollDelta) / 120; // Normalize scroll wheel
                            if (scrollDelta > 0)
                                _regStackScrollOffset = Math.Min(_regStackScrollOffset + scrollStep, totalValues - MaxVisibleValues);
                            else
                                _regStackScrollOffset = Math.Max(0, _regStackScrollOffset - scrollStep);
                        }
                        else
                        {
                            _regStackScrollOffset = 0;
                        }
                    }
                    _previousRegScrollValue = mouse.ScrollWheelValue;
                }
                // Check if mouse is in call stack area
                else
                {
                    int callStackStartY = regStackAreaEnd + lineHeight * 2; // Space between stacks
                    int callStackAreaStart = callStackStartY + labelHeight;
                    int callStackAreaEnd = callStackAreaStart + MaxVisibleValues * lineHeight;
                    
                    if (mousePos.Y >= callStackAreaStart && mousePos.Y < callStackAreaEnd)
                    {
                        int scrollDelta = mouse.ScrollWheelValue - _previousCallScrollValue;
                        if (scrollDelta != 0)
                        {
                            var callStack = Stacks.CallStack;
                            int totalValues = callStack.Count;
                            if (totalValues > MaxVisibleValues)
                            {
                                // Scroll: positive delta = scroll up (show older values), negative = scroll down (show newer values)
                                int scrollStep = Math.Abs(scrollDelta) / 120; // Normalize scroll wheel
                                if (scrollDelta > 0)
                                    _callStackScrollOffset = Math.Min(_callStackScrollOffset + scrollStep, totalValues - MaxVisibleValues);
                                else
                                    _callStackScrollOffset = Math.Max(0, _callStackScrollOffset - scrollStep);
                            }
                            else
                            {
                                _callStackScrollOffset = 0;
                            }
                        }
                        _previousCallScrollValue = mouse.ScrollWheelValue;
                    }
                }
            }
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            var theme = ServiceGraphics.Theme;
            byte fontFlags = (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline);
            
            // Get font metrics for proper alignment
            int fontHeight = theme.PrimaryFont.GlyphCellHeight;
            const byte characterSpacing = 1;
            int lineHeight = fontHeight + characterSpacing;
            
            // Measure character width using font metrics
            int charWidth = theme.PrimaryFont.MeasureText("M", 0, fontFlags).width;
            
            // Calculate how many values fit per row based on available width
            int availableWidth = contentRect.Width - Padding * 2;
            int labelWidth = theme.PrimaryFont.MeasureText("Regs", 0, fontFlags).width;
            int valuesAreaWidth = availableWidth - labelWidth - 10; // 10px spacing after label
            int valuesPerRow = Math.Max(1, valuesAreaWidth / (charWidth * 3)); // Each value is 2 hex chars + space

            int startY = contentRect.Y + Padding;

            // Draw register stack - show only last 40 values with scrolling
            var regStack = Stacks.RegisterStack;
            int regStackCount = regStack.Count;

            // Register stack label
            ServiceGraphics.DrawText(
                theme.PrimaryFont,
                "Regs",
                contentRect.X + Padding,
                startY,
                contentRect.Width - Padding * 2,
                theme.TextPrimary,
                theme.TextOutline,
                fontFlags,
                0xFF
            );
            
            // Calculate start index: show last MaxVisibleValues, scroll offset allows viewing older values
            int maxRegScroll = Math.Max(0, regStackCount - MaxVisibleValues);
            _regStackScrollOffset = MathHelper.Clamp(_regStackScrollOffset, 0, maxRegScroll);
            int regStackStartIndex = Math.Max(0, regStackCount - MaxVisibleValues - _regStackScrollOffset);
            int visibleRegCount = Math.Min(MaxVisibleValues, regStackCount - regStackStartIndex);
            
            int regStackY = startY + lineHeight;
            int maxRegRow = 0;
            for (int i = 0; i < visibleRegCount; i++)
            {
                int stackIndex = regStackStartIndex + i;
                if (stackIndex >= regStack.Count)
                    break;
                    
                int row = i / valuesPerRow;
                int col = i % valuesPerRow;
                int drawX = contentRect.X + Padding + labelWidth + 10 + col * (charWidth * 3);
                int drawY = regStackY + row * lineHeight;

                if (drawY + lineHeight > contentRect.Bottom)
                    break;

                maxRegRow = Math.Max(maxRegRow, row);

                // Last value gets darker blue color
                bool isLastValue = (stackIndex == regStack.Count - 1);
                Color valueColor = isLastValue ? theme.StackLastValueColor : theme.StackValueColor;

                ServiceGraphics.DrawText(
                    theme.PrimaryFont,
                    regStack[stackIndex],
                    drawX,
                    drawY,
                    contentRect.Width - Padding * 2,
                    valueColor,
                    theme.TextOutline,
                    fontFlags,
                    0xFF
                );
            }

            // Draw call stack - show only last 40 values with scrolling
            var callStack = Stacks.CallStack;
            int callStackCount = callStack.Count;

            // Call stack label - position after register stack with spacing
            int callStackLabelY = regStackY + (maxRegRow + 1) * lineHeight + lineHeight;
            ServiceGraphics.DrawText(
                theme.PrimaryFont,
                "Calls",
                contentRect.X + Padding,
                callStackLabelY,
                contentRect.Width - Padding * 2,
                theme.TextPrimary,
                theme.TextOutline,
                fontFlags,
                0xFF
            );
            
            // Calculate start index: show last MaxVisibleValues, scroll offset allows viewing older values
            int maxCallScroll = Math.Max(0, callStackCount - MaxVisibleValues);
            _callStackScrollOffset = MathHelper.Clamp(_callStackScrollOffset, 0, maxCallScroll);
            int callStackStartIndex = Math.Max(0, callStackCount - MaxVisibleValues - _callStackScrollOffset);
            int visibleCallCount = Math.Min(MaxVisibleValues, callStackCount - callStackStartIndex);
            
            // Calculate values per row for call stack (addresses are 6 hex chars)
            int callValuesPerRow = Math.Max(1, valuesAreaWidth / (charWidth * 7)); // 6 hex chars + space
            
            int callStackY = callStackLabelY + lineHeight;
            for (int i = 0; i < visibleCallCount; i++)
            {
                int stackIndex = callStackStartIndex + i;
                if (stackIndex >= callStack.Count)
                    break;
                    
                int row = i / callValuesPerRow;
                int col = i % callValuesPerRow;
                int drawX = contentRect.X + Padding + labelWidth + 10 + col * (charWidth * 7);
                int drawY = callStackY + row * lineHeight;

                if (drawY + lineHeight > contentRect.Bottom)
                    break;

                // Last value gets darker blue color
                bool isLastValue = (stackIndex == callStack.Count - 1);
                Color valueColor = isLastValue ? theme.StackLastValueColor : theme.StackValueColor;

                ServiceGraphics.DrawText(
                    theme.PrimaryFont,
                    callStack[stackIndex],
                    drawX,
                    drawY,
                    contentRect.Width - Padding * 2,
                    valueColor,
                    theme.TextOutline,
                    fontFlags,
                    0xFF
                );
            }
        }
    }
}

