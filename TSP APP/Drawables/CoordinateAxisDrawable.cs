using Microsoft.Maui.Graphics;
using System;
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
        private bool _drawConvexHulls;
        private List<List<Point>> _convexHulls;

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

        public void UpdatePoints(List<Point> points, bool drawLines, Point homePoint, bool drawConvexHulls, List<List<Point>>? convexHulls)
        {
            _points = points;
            _drawLines = drawLines;
            _homePoint = homePoint;
            _drawConvexHulls = drawConvexHulls;
            _convexHulls = convexHulls;
        }


        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.White;
            canvas.FillRectangle(dirtyRect);

            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 2;

            //Draw horizontal axis
            canvas.DrawLine(0, dirtyRect.Height / 2, dirtyRect.Width, dirtyRect.Height / 2);

            //Draw vertical axis
            canvas.DrawLine(dirtyRect.Width / 2, 0, dirtyRect.Width / 2, dirtyRect.Height);


            if (_drawConvexHulls)
            {
                canvas.StrokeColor = Colors.Black;
                if (_convexHulls != null || _convexHulls.Count < 1)
                {
                    List<Point> chPointstoDraw = new List<Point>();
                    foreach (var ch in _convexHulls)
                    {
                        foreach (var pt in ch)
                        {
                            chPointstoDraw.Add(pt);
                        }
                    }
                    chPointstoDraw = ScalePointsToFitRectangle(chPointstoDraw, dirtyRect);

                    int j = 0;
                    int i = 0;
                    foreach (var ch in _convexHulls)
                    {
                        canvas.StrokeColor = Colors.Black;
                        foreach (var chpt in ch)
                        {
                            canvas.DrawCircle(chPointstoDraw[j], 3.0);
                            j++;
                        }

                        canvas.StrokeColor = Colors.Black;
                        for (int ii = i; ii < i + ch.Count - 1; ii++)
                        {
                            var p1 = chPointstoDraw[ii];
                            var p2 = chPointstoDraw[ii + 1];
                            canvas.DrawLine(p1, p2);
                        }
                        i += ch.Count;
                    }
                }
                return;
            }

            List<Point> pointsToDraw = ScalePointsToFitRectangle(_points, dirtyRect);
            foreach (var ptd in pointsToDraw)
            {
                canvas.DrawCircle(ptd, 3.0);
            }

            if (_drawLines)
            {
                canvas.StrokeColor = Colors.Black;

                for (int i = 0; i < pointsToDraw.Count - 1; i++)
                {
                    canvas.DrawLine(pointsToDraw[i], pointsToDraw[i + 1]);
                }
            }
        }

        private List<Point> ScalePointsToFitRectangle(List<Point> points, RectF dirtyRect)
        {
            if (points == null || points.Count == 0)
                return new List<Point>();

            double minX = points.Min(p => p.X);
            double maxX = points.Max(p => p.X);
            double minY = points.Min(p => p.Y);
            double maxY = points.Max(p => p.Y);

            double originalWidth = maxX - minX;
            double originalHeight = maxY - minY;

            if (originalWidth == 0 && originalHeight == 0)
            {
                return new List<Point>
        {
            new Point(
                dirtyRect.Left + dirtyRect.Width / 2,
                dirtyRect.Top + dirtyRect.Height / 2
            )
        };
            }

            double scaleX = dirtyRect.Width / originalWidth;
            double scaleY = dirtyRect.Height / originalHeight;

            double scale = Math.Min(scaleX, scaleY);

            double centeredOffsetX = dirtyRect.Left + (dirtyRect.Width - (originalWidth * scale)) / 2;
            double centeredOffsetY = dirtyRect.Top + (dirtyRect.Height - (originalHeight * scale)) / 2;

            return points.Select(p => new Point(
                centeredOffsetX + (p.X - minX) * scale,
                centeredOffsetY + (originalHeight * scale) - (p.Y - minY) * scale
            )).ToList();
        }
    }
}
