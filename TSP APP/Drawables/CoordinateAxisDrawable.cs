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
            canvas.StrokeColor = Colors.White;
            canvas.StrokeSize = 2;

            // Draw horizontal axis
            canvas.DrawLine(0, dirtyRect.Height / 2, dirtyRect.Width, dirtyRect.Height / 2);

            // Draw vertical axis
            canvas.DrawLine(dirtyRect.Width / 2, 0, dirtyRect.Width / 2, dirtyRect.Height);


            if (_drawConvexHulls)
            {
                canvas.StrokeColor = Colors.White;
                if (_convexHulls != null || _convexHulls.Count < 1)
                {
                    List<Point> chPointstoDraw = new List<Point>();
                    foreach(var ch in _convexHulls)
                    {
                        foreach(var pt in ch)
                        {
                            chPointstoDraw.Add(pt);
                        }
                    }
                    chPointstoDraw = ScalePointsToFitRectangle(chPointstoDraw, dirtyRect);

                    int j = 0;
                    foreach (var ch in _convexHulls)
                    {
                        foreach (var chpt in ch)
                        {
                            canvas.DrawCircle(chPointstoDraw[j], 3.0);
                            j++;
                        }

                        canvas.StrokeColor = Colors.Pink;
                        for (int i = 0; i < chPointstoDraw.Count - 1; i++)
                        {
                            canvas.DrawLine(chPointstoDraw[i], chPointstoDraw[i + 1]);
                        }
                    }

                }

                return;
            }

            List<Point> pointsToDraw = ScalePointsToFitRectangle(_points, dirtyRect);
            foreach(var ptd in pointsToDraw)
            {
                canvas.DrawCircle(ptd, 3.0);
            }

            //if (_points.Count > 0)
            //{
            //    canvas.StrokeColor = Colors.Red;
            //    Point homePointToDraw = CreatePointToDraw(_homePoint, virtualLength, minX, minY, dirtyRect);
            //    canvas.DrawCircle(homePointToDraw, 3.0);
            //}

            if (_drawLines)
            {
                canvas.StrokeColor = Colors.Yellow;

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

            // Find the bounding box of the original points
            double minX = points.Min(p => p.X);
            double maxX = points.Max(p => p.X);
            double minY = points.Min(p => p.Y);
            double maxY = points.Max(p => p.Y);

            // Calculate the original width and height of the points
            double originalWidth = maxX - minX;
            double originalHeight = maxY - minY;

            // If all points are at the same location, center a single point
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

            // Calculate scale factors
            double scaleX = dirtyRect.Width / originalWidth;
            double scaleY = dirtyRect.Height / originalHeight;

            // Use the smaller scale to maintain aspect ratio
            double scale = Math.Min(scaleX, scaleY);

            // Calculate offsets to center the scaled points
            double centeredOffsetX = dirtyRect.Left + (dirtyRect.Width - (originalWidth * scale)) / 2;
            double centeredOffsetY = dirtyRect.Top + (dirtyRect.Height - (originalHeight * scale)) / 2;

            // Transform points
            return points.Select(p => new Point(
                centeredOffsetX + (p.X - minX) * scale,
                centeredOffsetY + (originalHeight * scale) - (p.Y - minY) * scale
            )).ToList();
        }
    }
}
