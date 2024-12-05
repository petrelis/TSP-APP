using TSP_Algorithms;
using TSP_Algorithms.Classes;
using TSP_Algorithms.ConvexHull;
using TSP_Algorithms.Drawables;
using TSPLIB;

namespace TSP_APP
{
    public partial class MainPage : ContentPage
    {
        static List<Point> Points = new List<Point>();
        int NumOfPoints = 0;
        CoordinateAxisDrawable drawable;
        List<string> AlgorithmNames = [BruteForce.Name, NearestNeighbour.Name, CircleMethod.Name, SingleConvexHullHeuristic.Name, AllConvexHullsHeuristic.Name, GeneticAlgorithm.Name];
        List<string?> TSPFileNames = TSPParser.GetTspFileNames();
        ShortestPath currentPath = null;
        bool displayHullsBtnClicked = false;
        bool displayHullsBtnEnabled = false;

        public MainPage()
        {
            InitializeComponent();

            drawable = new CoordinateAxisDrawable(Points);
            graphicsView.Drawable = drawable;
            AlgoPicker.ItemsSource = AlgorithmNames;
            TSPFilePicker.ItemsSource = TSPFileNames;
        }
        void RandomPointBtnClicked(object sender, EventArgs args)
        {
            Points.Clear();

            for (int i = 0; i < NumOfPoints; i++)
            {
                Points.Add(Algorithms.GenerateRandomPoint(5));
            }

            drawable.UpdatePoints(Points, false, Points[0], false, null);
            graphicsView.Invalidate();
            UpdateLabels(0, Points.Count, 0, 0);
        }

        void DrawPathBtnClicked(object sender, EventArgs args)
        {
            var selectedAlgoIndex = AlgoPicker.SelectedIndex;

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            switch (selectedAlgoIndex)
            {
                case 0:
                    currentPath = new ShortestPath(BruteForce.RunAlgo(Points));
                    break;
                case 1:
                    currentPath = new ShortestPath(NearestNeighbour.RunAlgo(Points));
                    break;
                case 2:
                    currentPath = new ShortestPath(CircleMethod.RunAlgo(Points));
                    break;
                case 3:
                    currentPath = new ShortestPath(SingleConvexHullHeuristic.RunAlgo(Points));
                    break;
                case 4:
                    currentPath = new ShortestPath(AllConvexHullsHeuristic.RunAlgo(Points));
                    break;
                case 5:
                    currentPath = new ShortestPath(GeneticAlgorithm.RunAlgo(Points));
                    break;
                default:
                    currentPath = new ShortestPath(([new Point(0, 0)], 0, new Point(0, 0)));
                    break;
            }

            watch.Stop();
            var timeElapsed = watch.ElapsedMilliseconds;
            var ticksElapsed = watch.ElapsedTicks;

            if (currentPath.AllConvexHulls != null && currentPath.AllConvexHulls.Count > 0)
                displayHullsBtnEnabled = true;
            else
                displayHullsBtnEnabled = false;
            DisplayHullsBtn.IsEnabled = displayHullsBtnEnabled;
            DisplayHullsBtn.IsVisible = displayHullsBtnEnabled;
            DisplayHullsBtn.Text = "Display All Hulls";
            displayHullsBtnClicked = false;

            drawable.UpdatePoints(currentPath.Points, true, currentPath.HomePoint);
            graphicsView.Invalidate();
            UpdateLabels(currentPath.Distance, Points.Count, timeElapsed, ticksElapsed);

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
            PointCountLbl.Text = $"Points Drawn: {pointCount}";
            TicksElapsedLbl.Text = $"Ticks Elapsed: {ticksElapsed}";
            TimeElapsedLbl.Text = $"Miliseconds Elapsed: {timeElapsed}";
        }

        void OnNumOfPointsEntryChanged(object sender, EventArgs e)
        {
            if (int.TryParse(NumOfPointsEntry.Text, out int num))
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
        }

        private async void NavigateBtnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AlgorithmTestPage());
        }

        private void DisplayHullsBtnClicked(object sender, EventArgs e)
        {
            if (!displayHullsBtnClicked)
                drawable.UpdatePoints(currentPath.Points, false, currentPath.HomePoint, true, currentPath.AllConvexHulls);
            else
                drawable.UpdatePoints(currentPath.Points, true, currentPath.HomePoint, false, null);

            graphicsView.Invalidate();

            displayHullsBtnClicked = displayHullsBtnClicked == true ? false : true;

            if (displayHullsBtnClicked)
            {
                DisplayHullsBtn.Text = "Display Path";
            }
            else
                DisplayHullsBtn.Text = "Display All Hulls";
        }

        private void TSPLIBPointBtnClicked(object sender, EventArgs e)
        {
            var selectedFileItem = TSPFilePicker.SelectedItem;
            if (selectedFileItem == null)
                return;

            string selectedFileName = selectedFileItem.ToString();
            Points.Clear();

            (string dataName, List<Point> pp) = TSPParser.ParseTspFile(selectedFileName);
            Points = pp;

            drawable.UpdatePoints(Points, false, Points[0], false, null);
            graphicsView.Invalidate();
            UpdateLabels(0, Points.Count, 0, 0);

        }
    }

}
