using System.Collections.Generic;
using System;
using Continuum93.Emulator;

namespace Continuum93.Emulator.Interrupts
{
    public struct PointF
    {
        public float X;
        public float Y;

        public PointF(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    public class BezierSegment
    {
        public float x1, y1; // Start point
        public float cx, cy; // Control point
        public float x2, y2; // End point
    }

    public class PathPoint
    {
        public float x, y;
    }

    public static class InterruptsVideoDrawBezierPath
    {
        public static void DrawBezierPath(byte regId, Computer computer)
        {
            // Read parameters from registers
            uint bezierPathAddress = computer.CPU.REGS.Get24BitRegister(computer.CPU.REGS.GetNextRegister(regId, 1));
            byte videoPage = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 4));
            byte lineBrushWidth = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 5));
            byte fillSegmentSize = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 6));
            byte emptySegmentSize = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 7));
            byte color = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 8));
            byte outlineColor = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 9));
            byte startPercent = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 10));
            byte endPercent = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 11));

            // Ensure startPercent and endPercent are within 0-100
            startPercent = Math.Min((byte)100, startPercent);
            endPercent = Math.Min((byte)100, endPercent);

            // Swap if startPercent > endPercent
            if (startPercent > endPercent)
            {
                byte temp = startPercent;
                startPercent = endPercent;
                endPercent = temp;
            }

            // Read the bezier path data from memory
            List<BezierSegment> bezierSegments = new();

            uint address = bezierPathAddress;

            // Read the initial start point
            ushort x1 = computer.MEMC.Get16bitFromRAM(address);
            address += 2;
            if (x1 == 0xFFFF) return; // Empty path
            ushort y1 = computer.MEMC.Get16bitFromRAM(address);
            address += 2;
            if (y1 == 0xFFFF) return; // Empty path

            while (true)
            {
                // Read control point
                ushort cx = computer.MEMC.Get16bitFromRAM(address);
                address += 2;
                if (cx == 0xFFFF) break;
                ushort cy = computer.MEMC.Get16bitFromRAM(address);
                address += 2;
                if (cy == 0xFFFF) break;

                // Read end point
                ushort x2 = computer.MEMC.Get16bitFromRAM(address);
                address += 2;
                if (x2 == 0xFFFF) break;
                ushort y2 = computer.MEMC.Get16bitFromRAM(address);
                address += 2;
                if (y2 == 0xFFFF) break;

                // Create the segment
                BezierSegment segment = new()
                {
                    x1 = x1,
                    y1 = y1,
                    cx = cx,
                    cy = cy,
                    x2 = x2,
                    y2 = y2
                };

                bezierSegments.Add(segment);

                // Update x1, y1 for the next segment
                x1 = x2;
                y1 = y2;
            }

            if (bezierSegments.Count == 0)
                return; // No segments to draw

            // Get video memory boundaries
            uint vStart = computer.GRAPHICS.GetVideoPageAddress(videoPage);

            // Calculate total length of the path
            float totalLength = 0;
            List<PathPoint> pathPoints = new();

            foreach (var segment in bezierSegments)
            {
                // Sample points along the segment
                int numSamples = 50; // Adjust for desired smoothness
                float lastX = segment.x1;
                float lastY = segment.y1;

                for (int i = 1; i <= numSamples; i++)
                {
                    float t = (float)i / numSamples;
                    float x = (1 - t) * (1 - t) * segment.x1 + 2 * (1 - t) * t * segment.cx + t * t * segment.x2;
                    float y = (1 - t) * (1 - t) * segment.y1 + 2 * (1 - t) * t * segment.cy + t * t * segment.y2;

                    float dx = x - lastX;
                    float dy = y - lastY;
                    float segmentLength = (float)Math.Sqrt(dx * dx + dy * dy);
                    totalLength += segmentLength;

                    pathPoints.Add(new PathPoint() { x = x, y = y });

                    lastX = x;
                    lastY = y;
                }
            }

            if (totalLength == 0)
                return; // Path has zero length

            // Determine the indices for start and end percentages
            float accumulatedLength = 0;
            int startIndex = 0;
            int endIndex = pathPoints.Count - 1;

            for (int i = 1; i < pathPoints.Count; i++)
            {
                float dx = pathPoints[i].x - pathPoints[i - 1].x;
                float dy = pathPoints[i].y - pathPoints[i - 1].y;
                float segmentLength = (float)Math.Sqrt(dx * dx + dy * dy);
                accumulatedLength += segmentLength;

                float percent = (accumulatedLength / totalLength) * 100;

                if (startIndex == 0 && percent >= startPercent)
                    startIndex = i - 1;

                if (percent >= endPercent)
                {
                    endIndex = i;
                    break;
                }
            }

            // Apply filled and empty segment pattern
            bool drawing = true;
            float currentSegmentLength = 0;
            float fillLength = fillSegmentSize;
            float emptyLength = emptySegmentSize;

            List<PointF> leftSidePoints = null;
            List<PointF> rightSidePoints = null;

            PointF leftEndPoint = new PointF();
            PointF rightEndPoint = new PointF();

            // Draw the path
            for (int i = startIndex + 1; i <= endIndex; i++)
            {
                var p0 = pathPoints[i - 1];
                var p1 = pathPoints[i];

                float dx = p1.x - p0.x;
                float dy = p1.y - p0.y;
                float segmentLength = (float)Math.Sqrt(dx * dx + dy * dy);

                if (drawing)
                {
                    // Start of a filled segment
                    if (currentSegmentLength == 0 || leftSidePoints == null)
                    {
                        leftSidePoints = new List<PointF>();
                        rightSidePoints = new List<PointF>();
                    }

                    // Draw the brush stroke between p0 and p1, collecting outline points and end points
                    DrawBrushStroke(p0, p1, lineBrushWidth, color, vStart, computer, leftSidePoints, rightSidePoints, out leftEndPoint, out rightEndPoint);

                    currentSegmentLength += segmentLength;

                    // Handle filled segments
                    if (currentSegmentLength >= fillLength)
                    {
                        drawing = false;
                        currentSegmentLength = 0;

                        // Add the end points to the outline
                        leftSidePoints.Add(leftEndPoint);
                        rightSidePoints.Add(rightEndPoint);

                        // At the end of a filled segment, draw the outline if outlineColor is not zero
                        if (outlineColor != 0)
                        {
                            // Combine left and right side points to form the outline
                            List<PointF> outlinePoints = new();
                            outlinePoints.AddRange(leftSidePoints);

                            // Add the right side points in reverse order
                            rightSidePoints.Reverse();
                            outlinePoints.AddRange(rightSidePoints);

                            // Draw the outline
                            DrawPolygon(outlinePoints, outlineColor, vStart, computer);
                        }

                        // Reset the point lists
                        leftSidePoints = null;
                        rightSidePoints = null;
                    }
                }
                else
                {
                    currentSegmentLength += segmentLength;

                    // Handle empty segments
                    if (currentSegmentLength >= emptyLength)
                    {
                        drawing = true;
                        currentSegmentLength = 0;
                    }
                }
            }

            // Finalize any remaining outline if still drawing
            if (drawing && outlineColor != 0 && leftSidePoints != null && rightSidePoints != null)
            {
                // Add the end points from the last brush stroke
                leftSidePoints.Add(leftEndPoint);
                rightSidePoints.Add(rightEndPoint);

                // Combine left and right side points
                List<PointF> outlinePoints = new();
                outlinePoints.AddRange(leftSidePoints);
                rightSidePoints.Reverse();
                outlinePoints.AddRange(rightSidePoints);

                // Draw the outline
                DrawPolygon(outlinePoints, outlineColor, vStart, computer);
            }
        }

        // Method to draw the brush stroke between two points
        private static void DrawBrushStroke(
            PathPoint start,
            PathPoint end,
            byte brushWidth,
            byte color,
            uint vStart,
            Computer computer,
            List<PointF> leftSidePoints,
            List<PointF> rightSidePoints,
            out PointF leftEndPoint,
            out PointF rightEndPoint)
        {
            // Calculate the direction vector
            float dx = end.x - start.x;
            float dy = end.y - start.y;
            float length = (float)Math.Sqrt(dx * dx + dy * dy);

            if (length == 0)
            {
                leftEndPoint = new PointF(0, 0);
                rightEndPoint = new PointF(0, 0);
                return;
            }

            // Normalize direction vector
            float nx = dx / length;
            float ny = dy / length;

            // Calculate the perpendicular vector
            float px = -ny;
            float py = nx;

            // Calculate the four corners of the brush stroke rectangle
            float halfWidth = brushWidth / 2.0f;
            float offsetX = px * halfWidth;
            float offsetY = py * halfWidth;

            // Points of the rectangle
            float x1 = start.x + offsetX;
            float y1 = start.y + offsetY;

            float x2 = start.x - offsetX;
            float y2 = start.y - offsetY;

            float x3 = end.x - offsetX;
            float y3 = end.y - offsetY;

            float x4 = end.x + offsetX;
            float y4 = end.y + offsetY;

            // Draw the filled quad (brush stroke)
            DrawFilledQuad(x1, y1, x2, y2, x3, y3, x4, y4, color, vStart, computer);

            // Collect the outline points
            if (leftSidePoints != null && rightSidePoints != null)
            {
                // Add the starting points
                leftSidePoints.Add(new PointF(x1, y1)); // Left side start point
                rightSidePoints.Add(new PointF(x2, y2)); // Right side start point
            }

            // Return the end points
            leftEndPoint = new PointF(x4, y4); // Left side end point
            rightEndPoint = new PointF(x3, y3); // Right side end point
        }

        // Method to draw a filled quadrilateral (convex polygon)
        private static void DrawFilledQuad(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, byte color, uint vStart, Computer computer)
        {
            // For simplicity, we'll split the quad into two triangles and draw them
            DrawFilledTriangle(x1, y1, x2, y2, x3, y3, color, vStart, computer);
            DrawFilledTriangle(x1, y1, x3, y3, x4, y4, color, vStart, computer);
        }

        // Method to draw a filled triangle using the scanline fill algorithm
        private static void DrawFilledTriangle(float x1, float y1, float x2, float y2, float x3, float y3, byte color, uint vStart, Computer computer)
        {
            // Convert points to integers
            int[] x = { (int)Math.Round(x1), (int)Math.Round(x2), (int)Math.Round(x3) };
            int[] y = { (int)Math.Round(y1), (int)Math.Round(y2), (int)Math.Round(y3) };

            // Sort the vertices by y-coordinate ascending (y0 <= y1 <= y2)
            if (y[0] > y[1])
            {
                Swap(ref y[0], ref y[1]);
                Swap(ref x[0], ref x[1]);
            }
            if (y[1] > y[2])
            {
                Swap(ref y[1], ref y[2]);
                Swap(ref x[1], ref x[2]);
            }
            if (y[0] > y[1])
            {
                Swap(ref y[0], ref y[1]);
                Swap(ref x[0], ref x[1]);
            }

            // Compute inverse slopes
            float dx1 = y[1] - y[0] > 0 ? (float)(x[1] - x[0]) / (y[1] - y[0]) : 0;
            float dx2 = y[2] - y[0] > 0 ? (float)(x[2] - x[0]) / (y[2] - y[0]) : 0;
            float dx3 = y[2] - y[1] > 0 ? (float)(x[2] - x[1]) / (y[2] - y[1]) : 0;

            float xs = x[0];
            float xe = x[0];

            // Draw upper part of the triangle
            if (y[1] != y[0])
            {
                for (int yPos = y[0]; yPos < y[1]; yPos++)
                {
                    DrawScanLine((int)xs, (int)xe, yPos, color, vStart, computer);
                    xs += dx1;
                    xe += dx2;
                }
            }

            // Draw lower part of the triangle
            if (y[2] != y[1])
            {
                xs = x[1];
                if (y[2] - y[1] > 0)
                    dx1 = (float)(x[2] - x[1]) / (y[2] - y[1]);
                else
                    dx1 = 0;

                for (int yPos = y[1]; yPos <= y[2]; yPos++)
                {
                    DrawScanLine((int)xs, (int)xe, yPos, color, vStart, computer);
                    xs += dx1;
                    xe += dx2;
                }
            }
        }

        // Helper method to swap two integers
        private static void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }

        // Helper method to draw a horizontal line (scanline)
        private static void DrawScanLine(int xStart, int xEnd, int yPos, byte color, uint vStart, Computer computer)
        {
            if (yPos < 0 || yPos >= Constants.V_HEIGHT)
                return;

            if (xStart > xEnd)
            {
                int temp = xStart;
                xStart = xEnd;
                xEnd = temp;
            }

            xStart = Math.Max(0, xStart);
            xEnd = Math.Min((int)Constants.V_WIDTH - 1, xEnd);

            for (int xPos = xStart; xPos <= xEnd; xPos++)
            {
                uint pixelAddress = vStart + (uint)(yPos * Constants.V_WIDTH + xPos);
                computer.MEMC.Set8bitToRAM(pixelAddress, color);
            }
        }

        // Method to draw a polygon by connecting points
        private static void DrawPolygon(List<PointF> points, byte color, uint vStart, Computer computer)
        {
            if (points == null || points.Count < 2)
                return;

            // Draw lines between consecutive points
            for (int i = 0; i < points.Count; i++)
            {
                PointF p0 = points[i];
                PointF p1 = points[(i + 1) % points.Count]; // Wrap around to the first point

                DrawLine(p0.X, p0.Y, p1.X, p1.Y, color, vStart, computer);
            }
        }

        // Method to draw a line using Bresenham's algorithm
        private static void DrawLine(float x0, float y0, float x1, float y1, byte color, uint vStart, Computer computer)
        {
            int xStart = (int)Math.Round(x0);
            int yStart = (int)Math.Round(y0);
            int xEnd = (int)Math.Round(x1);
            int yEnd = (int)Math.Round(y1);

            int dx = Math.Abs(xEnd - xStart);
            int dy = -Math.Abs(yEnd - yStart);
            int sx = xStart < xEnd ? 1 : -1;
            int sy = yStart < yEnd ? 1 : -1;
            int err = dx + dy;

            while (true)
            {
                if (xStart >= 0 && xStart < Constants.V_WIDTH && yStart >= 0 && yStart < Constants.V_HEIGHT)
                {
                    uint pixelAddress = vStart + (uint)(yStart * Constants.V_WIDTH + xStart);
                    computer.MEMC.Set8bitToRAM(pixelAddress, color);
                }

                if (xStart == xEnd && yStart == yEnd)
                    break;

                int e2 = 2 * err;
                if (e2 >= dy)
                {
                    err += dy;
                    xStart += sx;
                }
                if (e2 <= dx)
                {
                    err += dx;
                    yStart += sy;
                }
            }
        }

    }
}
