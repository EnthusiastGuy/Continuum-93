using Continuum93.Emulator;
using Microsoft.Xna.Framework;
using System;

namespace Continuum93.Emulator.GraphicsAccelerators
{
    public class GFXLineDraw
    {
        private readonly byte[] vMem;

        public GFXLineDraw(byte[] vMem)
        {
            this.vMem = vMem;
        }

        private void DrawLineSegment(Point start, int thickness, Point target)
        {
            int dx = Math.Abs(target.X - start.X);
            int dy = Math.Abs(target.Y - start.Y);

            int sx = start.X < target.X ? 1 : -1;
            int sy = start.Y < target.Y ? 1 : -1;

            int err = dx - dy;

            while (true)
            {
                DrawPixel(start.X, start.Y, thickness);

                if (start.X == target.X && start.Y == target.Y) break;

                int e2 = err * 2;

                if (e2 > -dy)
                {
                    err -= dy;
                    start.X += sx;
                }

                if (e2 < dx)
                {
                    err += dx;
                    start.Y += sy;
                }
            }
        }

        private void DrawPixel(int x, int y, int thickness)
        {
            if (x < 0 || x >= Constants.V_WIDTH || y < 0 || y >= Constants.V_HEIGHT) return;

            for (int tY = -thickness / 2; tY <= thickness / 2; tY++)
            {
                for (int tX = -thickness / 2; tX <= thickness / 2; tX++)
                {
                    int tPosX = x + tX;
                    int tPosY = y + tY;

                    if (tPosX >= 0 && tPosX < Constants.V_WIDTH && tPosY >= 0 && tPosY < Constants.V_HEIGHT)
                    {
                        vMem[tPosY * Constants.V_WIDTH + tPosX] = 0xFF;
                    }
                }
            }
        }

        public void DrawContinuousLines(Point start, LineInfo[] lines)
        {
            Point currentPoint = start;

            foreach (var line in lines)
            {
                Point target = line.Target;
                DrawLineSegment(currentPoint, line.Thickness, target);

                currentPoint = target;
            }
        }
    }

    public struct LineInfo
    {
        public int Thickness;
        public Point Target;

        public LineInfo(int thickness, Point target)
        {
            Thickness = thickness;
            Target = target;
        }
    }
}
