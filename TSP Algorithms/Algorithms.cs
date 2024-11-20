using Microsoft.Maui.Controls.Shapes;
using System.Linq;
using TSP_Algorithms.Classes;

namespace TSP_Algorithms
{
    public class Algorithms
    {
        public static Point GenerateRandomPoint(int mult)
        {
            Random rnd = new Random();

            float x = mult - rnd.NextSingle() * mult * 2;
            float y = mult - rnd.NextSingle() * mult * 2;

            return new Point(x, y);
        }

        public static List<Point> TwoOpt(List<Point> path)
        {
            bool improvement = true;

            while (improvement)
            {
                improvement = false;

                for (int i = 1; i < path.Count - 2; i++) // Avoid the first and last city
                {
                    for (int j = i + 1; j < path.Count - 1; j++) // Avoid overlapping segments
                    {
                        // Calculate distances for current and swapped segments
                        double currentDistance = path[i - 1].Distance(path[i]) + 
                                                 path[j].Distance(path[j + 1]);
                        double newDistance = path[i - 1].Distance(path[j]) +
                                             path[i].Distance(path[j + 1]);

                        if (newDistance < currentDistance)
                        {
                            // Reverse the segment between i and j
                            path.Reverse(i, j - i + 1);
                            improvement = true;
                        }
                    }
                }
            }

            return path;
        }

        public static double CalculatePathDistance(List<Point> path)
        {
            double totalDistance = 0;

            for (int i = 0; i < path.Count - 1; i++)
            {
                totalDistance += path[i].Distance(path[i + 1]); 
            }

            return totalDistance;
        }

    }
}
