using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Continuum93.ServiceModule.UI
{
    /// <summary>
    /// Window shell that hosts the emulator's main view. The actual view is drawn
    /// by ServiceGraphics so this window only provides chrome, sizing, and
    /// a target rectangle for the zoom animation.
    /// </summary>
    public class EmulatorWindow : Window
    {
        public const int BaseViewportWidth = 480;
        public const int BaseViewportHeight = 270;
        private const int ViewPadding = 6;

        public EmulatorWindow(int x, int y)
            : base(
                  "Emulator",
                  x,
                  y,
                  BaseViewportWidth + BorderWidth * 2,
                  BaseViewportHeight + TitleBarHeight + BorderWidth,
                  spawnDelaySeconds: 0f,
                  canResize: true,
                  canClose: false)
        {
        }

        // Let the emulator feed show through – only draw chrome/title/borders.
        protected override bool ShouldDrawBackground => false;

        /// <summary>
        /// Computes the destination rectangle for the emulator texture inside this window,
        /// preserving aspect ratio and applying a small padding.
        /// </summary>
        public Rectangle GetViewportRect(int sourceWidth, int sourceHeight)
        {
            var targetArea = GetViewportBounds();
            if (targetArea.Width <= 0 || targetArea.Height <= 0)
            {
                return Rectangle.Empty;
            }

            float scale = Math.Min(
                (float)targetArea.Width / sourceWidth,
                (float)targetArea.Height / sourceHeight);

            int destWidth = (int)Math.Round(sourceWidth * scale);
            int destHeight = (int)Math.Round(sourceHeight * scale);

            int destX = targetArea.X + (targetArea.Width - destWidth) / 2;
            int destY = targetArea.Y + (targetArea.Height - destHeight) / 2;

            return new Rectangle(destX, destY, destWidth, destHeight);
        }

        private Rectangle GetViewportBounds()
        {
            var rect = ContentRect;
            rect.Inflate(-ViewPadding, -ViewPadding);

            // Ensure non-negative dimensions
            if (rect.Width < 1) rect.Width = 1;
            if (rect.Height < 1) rect.Height = 1;

            return rect;
        }

        protected override void OnResized()
        {
            base.OnResized();
            SnapToAspectRatio();
        }

        private void SnapToAspectRatio()
        {
            const float aspect = (float)BaseViewportWidth / BaseViewportHeight;

            int contentWidth = Math.Max(1, Width - BorderWidth * 2);
            int contentHeight = Math.Max(1, Height - TitleBarHeight - BorderWidth);

            int targetHeightFromWidth = (int)Math.Round(contentWidth / aspect);
            int targetWidthFromHeight = (int)Math.Round(contentHeight * aspect);

            // Pick the adjustment that requires the smaller change
            int adjustedContentWidth = contentWidth;
            int adjustedContentHeight = contentHeight;

            if (Math.Abs(targetWidthFromHeight - contentWidth) < Math.Abs(targetHeightFromWidth - contentHeight))
            {
                adjustedContentWidth = targetWidthFromHeight;
            }
            else
            {
                adjustedContentHeight = targetHeightFromWidth;
            }

            // Clamp to a reasonable minimum while keeping 16:9
            int minContentWidth = BaseViewportWidth / 2;
            int minContentHeight = BaseViewportHeight / 2;

            if (adjustedContentWidth < minContentWidth)
            {
                adjustedContentWidth = minContentWidth;
                adjustedContentHeight = (int)Math.Round(adjustedContentWidth / aspect);
            }

            if (adjustedContentHeight < minContentHeight)
            {
                adjustedContentHeight = minContentHeight;
                adjustedContentWidth = (int)Math.Round(adjustedContentHeight * aspect);
            }

            Width = adjustedContentWidth + BorderWidth * 2;
            Height = adjustedContentHeight + TitleBarHeight + BorderWidth;
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            // The emulator view is rendered in ServiceGraphics with the zoom animation;
            // nothing else to draw here.
        }
    }
}




