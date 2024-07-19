using TSP_Algorithms;
using TSP_Algorithms.Drawables;

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
            UpdateLabels(0, Points.Count+1, 0, 0);
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

        private void OnSizeChanged(object sender, EventArgs e)
        {
            graphicsView.HeightRequest = UsableWindow.Height * 0.5;
            graphicsView.WidthRequest = graphicsView.HeightRequest;
        }


    }

}
