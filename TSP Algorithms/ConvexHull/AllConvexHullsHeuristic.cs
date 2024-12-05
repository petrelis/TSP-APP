using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Algorithms.ConvexHull
{
    public class AllConvexHullsHeuristic : ConvexHullGeneral
    {
        public const string Name = "All Convex Hull Heuristic";

        public static (List<Point> points, float distance, Point homePoint, List<List<Point>>? allHulls) RunAlgo(List<Point> points)
        {
            if (points == null || points.Count < 3)
                return (points, 0, new Point(0, 0), null);

            var workingPoints = new List<Point>(points);

            (List<List<Point>> allConvexHulls, List<Point> restPoints) = FindAllConvexHulls(workingPoints);

            List<Point> pointsOnBestPath = ConnectAllConvexHulls(allConvexHulls, restPoints);

            pointsOnBestPath = Algorithms.TwoOpt(pointsOnBestPath);

            var totalDistance = (float)Algorithms.CalculatePathDistance(pointsOnBestPath);

            return (pointsOnBestPath, totalDistance, points[0], allConvexHulls);
        }
    }
}
