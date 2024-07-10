using Microsoft.Maui.Graphics;

namespace TSP_APP
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();

            var drawable = new CoordinateAxisDrawable();
            graphicsView.Drawable = drawable;
        }

        public class CoordinateAxisDrawable : IDrawable
        {
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

                double axisXMidPoint = dirtyRect.Width / 2;
                double axisYMidPoint = dirtyRect.Height / 2;
                float PointOffsetXMultiplier = dirtyRect.Width / numberOfTicks;
                float PointOffsetYMultiplier = dirtyRect.Height / numberOfTicks;

                List<Point> points = new List<Point>();
                List<Point> initPoints = [new Point(-3, -4), new Point(1, 2), new Point(-2, -1)];
                foreach (var point in initPoints)
                {
                    Point pointToDraw = new Point(point.X * PointOffsetXMultiplier + axisXMidPoint, -point.Y * PointOffsetYMultiplier + axisYMidPoint);
                    canvas.DrawCircle(pointToDraw, 3.0);
                    points.Add(pointToDraw);
                }

                for (int i = 0; i < points.Count-1; i++)
                {
                    canvas.DrawLine(points[i], points[i+1]);
                }
            }
        }

    }

}
