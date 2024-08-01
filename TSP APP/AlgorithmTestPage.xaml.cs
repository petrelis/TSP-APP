using System.Diagnostics;
using TSP_Algorithms;

namespace TSP_APP;

public partial class AlgorithmTestPage : ContentPage
{
    public AlgorithmTestPage()
    {
        InitializeComponent();
    }

    private void TestAllAlgosBtnClicked(object sender, EventArgs e)
    {
        int numOfCycles = 999;
        List<Point> points = new List<Point>();
        int numOfPoints = 7;
        long[] times = new long[3];
        double[] distances = new double[3];

        for (int i = 0; i < numOfCycles; i++)
        {
            //Generate points
            points.Clear();

            for (int j = 0; j < numOfPoints; j++)
            {
                points.Add(Algorithms.GenerateRandomPoint(5));
            }

            //Generate paths and calculate times and distances
            (List<Point> points, float distance) path;

            Stopwatch[] watches = new Stopwatch[3];

            watches[0] = new Stopwatch();
            watches[0].Start();
            path = Algorithms.BruteForce(points);
            watches[0].Stop();
            times[0] += watches[0].ElapsedTicks;
            distances[0] += path.distance;

            watches[1] = new Stopwatch();
            watches[1].Start();
            path = Algorithms.NearestNeighbour(points);
            watches[1].Stop();
            times[1] += watches[1].ElapsedTicks;
            distances[1] += path.distance;

            watches[2] = new Stopwatch();
            watches[2].Start();
            path = Algorithms.CircleMethod(points);
            watches[2].Stop();
            times[2] += watches[2].ElapsedTicks;
            distances[2] += path.distance;

            //Calculate efficiencies
            //distances[1] = distances[0]/distances[1];
            //distances[2] = distances[0]/distances[2];
            //times[1] /= times[0];
            //times[2] /= times[0];
        }

        NNDistanceEfficiencyLbl.Text = $"NNDistance Efficiency: {(1- distances[1] / distances[0])*100:0.0000}%";
        NNTimeEfficiencyLbl.Text = $"NNTime Efficiency: {times[1]}";
        CMDistanceEfficiencyLbl.Text = $"CMDistance Efficiency: {(1 - distances[2] / distances[0]) * 100:0.0000}%";
        CMTimeEfficiencyLbl.Text = $"CMTime Efficiency: {times[2]}";
    }
}