using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Algorithms.ConvexHull
{
    public class SingleConvexHullHeuristic : ConvexHullGeneral
    {
        public const string Name = "Single Convex Hull Heuristic";

        public static (List<Point> points, float distance, Point homePoint, List<List<Point>>? allHulls) RunAlgo(List<Point> points)
        {
            if (points == null || points.Count < 3)
                return (points, 0, new Point(0, 0), null);

            var workingPoints = new List<Point>(points);

            (List<Point> convexHull, List<Point> restPoints) = FindConvexHull(workingPoints);

            var allConvexHulls = new List<List<Point>> { convexHull };

            var pointsOnBestPath = ConnectTwoConvexHulls(convexHull, restPoints);

            pointsOnBestPath = Algorithms.TwoOpt(pointsOnBestPath);

            var totalDistance = (float)Algorithms.CalculatePathDistance(pointsOnBestPath);

            return (pointsOnBestPath, totalDistance, points[0], allConvexHulls);
        }
    }
}
