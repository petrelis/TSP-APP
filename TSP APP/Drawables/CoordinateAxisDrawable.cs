﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Algorithms.Drawables
{
    public class CoordinateAxisDrawable : IDrawable
    {
        private List<Point> _points;
        private bool _drawLines;
        private Point _homePoint;

        public CoordinateAxisDrawable(List<Point> points)
        {
            _points = points;
        }

        public void UpdatePoints(List<Point> points, bool drawLines, Point homePoint)
        {
            _points = points;
            _drawLines = drawLines;
            _homePoint = homePoint;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {

            canvas.StrokeColor = Colors.White;
            canvas.StrokeSize = 2;

            // Draw horizontal axis
            canvas.DrawLine(0, dirtyRect.Height / 2, dirtyRect.Width, dirtyRect.Height / 2);

            // Draw vertical axis
            canvas.DrawLine(dirtyRect.Width / 2, 0, dirtyRect.Width / 2, dirtyRect.Height);

            // Draw labels and ticks
            const int numberOfTicks = 10;
            const float tickLength = 10;

            for (int i = 0; i <= numberOfTicks; i++)
            {
                float x = (i / (float)numberOfTicks) * dirtyRect.Width;
                float y = (i / (float)numberOfTicks) * dirtyRect.Height;

                // Horizontal ticks and labels
                canvas.DrawLine(x, dirtyRect.Height / 2 - tickLength, x, dirtyRect.Height / 2 + tickLength);
                canvas.DrawString($"{i - numberOfTicks / 2}", x, dirtyRect.Height / 2 + tickLength + 5, HorizontalAlignment.Center);

                // Vertical ticks and labels
                canvas.DrawLine(dirtyRect.Width / 2 - tickLength, y, dirtyRect.Width / 2 + tickLength, y);
                canvas.DrawString($"{numberOfTicks / 2 - i}", dirtyRect.Width / 2 + tickLength + 5, y, HorizontalAlignment.Left);
            }

            float axisXMidPoint = dirtyRect.Width / 2;
            float axisYMidPoint = dirtyRect.Height / 2;
            float PointOffsetXMultiplier = dirtyRect.Width / numberOfTicks;
            float PointOffsetYMultiplier = dirtyRect.Height / numberOfTicks;

            List<Point> pointsDrawn = new List<Point>();
            foreach (var point in _points)
            {
                Point pointToDraw = new Point(point.X * PointOffsetXMultiplier + axisXMidPoint, point.Y * PointOffsetYMultiplier + axisYMidPoint);
                canvas.DrawCircle(pointToDraw, 3.0);
                pointsDrawn.Add(pointToDraw);
            }

            if (_points.Count > 0)
            {
                canvas.StrokeColor = Colors.Red;
                Point homePointToDraw = new Point(_homePoint.X * PointOffsetXMultiplier + axisXMidPoint, _homePoint.Y * PointOffsetYMultiplier + axisYMidPoint);
                canvas.DrawCircle(homePointToDraw, 3.0);
            }

            if (_drawLines)
            {
                canvas.StrokeColor = Colors.Blue;

                for (int i = 0; i < pointsDrawn.Count - 1; i++)
                {
                    canvas.DrawLine(pointsDrawn[i], pointsDrawn[i + 1]);
                }
            }
        }
    }
}
