using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TSP_Algorithms.Classes;
using TSP_Algorithms.ConvexHull;

namespace TSP_Algorithms
{
    public class BenchmarkResult
    {
        public String AlgoName {  get; set; }
        public int NumberOfPoints { get; set; }
        public double ExecutionTimeMs { get; set; }
        public double TotalDistance { get; set; }
    }

    public class AlgoBenchmark
    {
        private static Random rnd = new Random(32);
        private readonly string outputDirectory;

        public AlgoBenchmark(string outputDir)
        {
            outputDirectory = outputDir;
            Directory.CreateDirectory(outputDir);
        }

        public void RunBenchmark(int[] pointCounts, int repetitionsPerCount)
        {
            foreach (int pointCount in pointCounts)
            {
                var benchmarkResults = new List<BenchmarkResult>();
                List<Point> points = new List<Point>();
                for (int i = 0; i < pointCount; i++)
                {
                    points.Add(GenerateRandomPoint(1000));
                }

                for (int rep = 1; rep <= repetitionsPerCount; rep++)
                {
                    var stopwatch = Stopwatch.StartNew();
                    var result = new ShortestPath(GeneticAlgorithm.RunAlgo(points));
                    stopwatch.Stop();

                    var benchmarkResult = new BenchmarkResult
                    {
                        AlgoName = "GeneticAlgorithm",
                        NumberOfPoints = pointCount,
                        ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                        TotalDistance = result.Distance,
                    };

                    benchmarkResults.Add(benchmarkResult);
                }

                string fileName = Path.Combine(outputDirectory, $"GA_benchmark_results_{pointCount}points1.json");
                string jsonString = JsonSerializer.Serialize(benchmarkResults, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(fileName, jsonString);
            }
        }
        private static Point GenerateRandomPoint(int mult)
        {
            float x = mult - rnd.NextSingle() * mult * 2;
            float y = mult - rnd.NextSingle() * mult * 2;

            return new Point(x, y);
        }

    }
}
