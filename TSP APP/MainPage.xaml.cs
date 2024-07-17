using TSP_Algorithms;

namespace TSP_APP
{
    public partial class MainPage : ContentPage
    {
        List<Point> Points = new List<Point>();
        int NumOfPoints = 0;
        CoordinateAxisDrawable drawable;

        public MainPage()
        {
            InitializeComponent();

            drawable = new CoordinateAxisDrawable(Points);
            graphicsView.Drawable = drawable;
        }

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
        void RandomPointBtnClicked(object sender, EventArgs args)
        {
            Points.Clear();

            for (int i = 0; i < NumOfPoints; i++)
            {
                Points.Add(Algorithms.GenerateRandomPoint(5));
            }

            drawable.UpdatePoints(Points, false, Points[0]);
            graphicsView.Invalidate();
            BruteForcePathBtn.IsEnabled = true;
            ClosestNeighbourPathBtn.IsEnabled = true;
            UpdateLabels();
        }

        void BruteForcePathBtnClicked(object sender, EventArgs args)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var bruteForcePath = Algorithms.BruteForce(Points);

            watch.Stop();
            var timeElapsed = watch.ElapsedMilliseconds;
            var ticksElapsed = watch.ElapsedTicks;

            drawable.UpdatePoints(bruteForcePath.points, true, Points[0]);
            graphicsView.Invalidate();
            UpdateLabels(bruteForcePath.distance, bruteForcePath.points.Count, timeElapsed, ticksElapsed);
        }

        void ClosestNeighbourPathBtnClicked(object sender, EventArgs args)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var closesNeighbourPath = Algorithms.NearestNeighbour(Points);

            watch.Stop();
            var timeElapsed = watch.ElapsedMilliseconds;
            var ticksElapsed = watch.ElapsedTicks;

            drawable.UpdatePoints(closesNeighbourPath.points, true, Points[0]);
            graphicsView.Invalidate();
            UpdateLabels(closesNeighbourPath.distance, closesNeighbourPath.points.Count, timeElapsed, ticksElapsed);
        }

        void ClearPointsBtnClicked(object sender, EventArgs args)
        {
            BruteForcePathBtn.IsEnabled = false;
            ClosestNeighbourPathBtn.IsEnabled = false;
            UpdateLabels();
            Points.Clear();
            drawable.UpdatePoints(Points, false, new Point(0, 0));
            graphicsView.Invalidate();

        }

        void UpdateLabels()
        {
            DistanceLbl.Text = "Total Distance: 0";
            PointCountLbl.Text = "Points Drawn: 0";
            TicksElapsedLbl.Text = "Ticks Elapsed: 0";
            TimeElapsedLbl.Text = "Time Elapsed: 0ms";
        }

        void UpdateLabels(float distance, int pointCount, long timeElapsed, long ticksElapsed)
        {
            DistanceLbl.Text = $"Total Distance: {distance:0.00}";
            PointCountLbl.Text = $"Points Drawn: {pointCount - 1}";
            TicksElapsedLbl.Text = $"Ticks Elapsed: {ticksElapsed}";
            TimeElapsedLbl.Text = $"Miliseconds Elapsed: {timeElapsed}";
        }

        void OnNumOfPointsEntryChanged(object sender, EventArgs e)
        {
            int num = 0;
            if (int.TryParse(NumOfPointsEntry.Text, out num))
            {
                NumOfPoints = num;
                if (num > 2)
                {
                    RandomPointBtn.IsEnabled = true;
                }
                else RandomPointBtn.IsEnabled = false;
            }
            else
            {
                RandomPointBtn.IsEnabled = false;
                BruteForcePathBtn.TextColor = Colors.White;
            }
        }

        void OnNumOfPointsEntryCompleted(object sender, EventArgs e)
        {
        }

    }

}
