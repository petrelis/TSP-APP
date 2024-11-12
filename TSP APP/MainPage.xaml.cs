using TSP_Algorithms;
using TSP_Algorithms.Classes;
using TSP_Algorithms.Drawables;

namespace TSP_APP
{
    public partial class MainPage : ContentPage
    {
        List<Point> Points = new List<Point>();
        int NumOfPoints = 0;
        CoordinateAxisDrawable drawable;
        List<string> AlgorithmNames = [BruteForce.Name, NearestNeighbour.Name, CircleMethod.Name, ConvexHull.Name];

        public MainPage()
        {
            InitializeComponent();

            drawable = new CoordinateAxisDrawable(Points);
            graphicsView.Drawable = drawable;
            AlgoPicker.ItemsSource = AlgorithmNames;
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
            UpdateLabels(0, Points.Count + 1, 0, 0);
        }

        void DrawPathBtnClicked(object sender, EventArgs args)
        {
            var selectedAlgoIndex = AlgoPicker.SelectedIndex;

            ShortestPath path = null;
            bool convexhull = false;

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            switch(selectedAlgoIndex)
            {
                case 0:
                    path = new ShortestPath(BruteForce.RunBruteForceAlgo(Points));
                    break;
                case 1:
                    path = new ShortestPath(NearestNeighbour.RunNearestNeighbourAlgo(Points));
                    break;
                case 2:
                    path = new ShortestPath(CircleMethod.RunCircleMethodAlgo(Points));
                    break;
                case 3:
                    path = new ShortestPath(ConvexHull.RunConvexHullAlgo(Points));
                    convexhull = true;
                    break;
                default:
                    path = new ShortestPath(([new Point(0, 0)], 0, new Point(0, 0)));
                    break;
            }

            watch.Stop();
            var timeElapsed = watch.ElapsedMilliseconds;
            var ticksElapsed = watch.ElapsedTicks;

            drawable.UpdatePoints(path.Points, true, path.HomePoint, path.IsConvexHull, path.AllConvexHulls);
            graphicsView.Invalidate();
            UpdateLabels(path.Distance, path.Points.Count, timeElapsed, ticksElapsed);

        }

        void ClearPointsBtnClicked(object sender, EventArgs args)
        {
            DrawPathBtn.IsEnabled = false;
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

        private void AlgoPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawPathBtn.IsEnabled = true;
            if (AlgoPicker.SelectedIndex == 3)
            {
                DisplayHullsBtn.IsVisible = true;
            }
            else
            {
                DisplayHullsBtn.IsVisible = false;
            }
        }

        private async void NavigateBtnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AlgorithmTestPage());
        }

        private void DisplayHullsBtnClicked(object sender, EventArgs e)
        {
            
        }

    }

}
