using Microsoft.Maui.Controls.Shapes;
using System.Linq;
using TSP_Algorithms.Classes;

namespace TSP_Algorithms
{
    public class BruteForce
    {
        public const string Name = "Brute Force";

        public static (List<Point> points, float distance, Point homePoint) RunAlgo(List<Point> Points)
        {
            float minDist = float.MaxValue;
            var homePoint = Points[0];
            var permutations = GenerateAllPermutations(Points);
            List<Point> pointsOnBestPath = new List<Point>();
            foreach (var p in permutations)
            {
                float dist = 0;

                for (int i = 0; i < p.Count - 1; i++)
                {
                    dist += (float)p[i].Distance(p[i + 1]);
                }

                dist += (float)p[p.Count - 1].Distance(p[0]);
                p.Add(p[0]);

                if (dist < minDist)
                {
                    minDist = dist;
                    pointsOnBestPath = p;
                }
            }

            return (pointsOnBestPath, minDist, homePoint);
        }

        private static List<List<Point>> GenerateAllPermutations(List<Point> Points)
        {
            if (Points.Count <= 1)
                return [Points];

            List<List<Point>> permutations = new List<List<Point>>();
            for (int i = 0; i < Points.Count; i++)
            {
                Point p = Points[i];
                List<Point> rest = new List<Point>();
                for (int i0 = 0; i0 < i; i0++)
                {
                    rest.Add(Points[i0]);
                }
                for (int iend = i + 1; iend < Points.Count; iend++)
                {
                    rest.Add(Points[iend]);
                }

                List<List<Point>> restPermutations = GenerateAllPermutations(rest);

                foreach (var perm in restPermutations)
                {
                    perm.Add(p);
                    permutations.Add(perm);
                }
            }
            return permutations;
        }
    }
}