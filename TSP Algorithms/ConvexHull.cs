using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP_Algorithms.Classes;
using TSP_Algorithms.Classes.ConvexHullClasses;

namespace TSP_Algorithms
{
    public class ConvexHull
    {
        public const string Name = "Convex Hull";

        private class DistanceCache
        {
            //Key of two points and value of distance
            private readonly Dictionary<(Point, Point), double> _cache = new();

            public double GetDistance(Point p1, Point p2)
            {
                var key = (p1, p2);
                //if distance between two points is not in the dictionary
                if (!_cache.ContainsKey(key))
                {
                    //add point and distance kvp
                    _cache[key] = p1.Distance(p2);
                }
                //return stored distance between the two points
                return _cache[key];
            }
        }


        public static (List<Point> points, float distance, Point homePoint, List<List<Point>>? allHulls) RunConvexHullAlgo(List<Point> points)
        {
            if (points == null || points.Count < 3)
                return (points, 0, new Point(0, 0), null);

            var workingPoints = new List<Point>(points);

            //(List<List<Point>> allConvexHulls, List<Point> rp) = FindAllConvexHulls(workingPoints);

            //List<Point> pointsOnBestPath = ConnectAllConvexHulls(allConvexHulls, restPoints);

            (List<Point> convexHull, List<Point> restPoints) = FindConvexHull(workingPoints);

            var allConvexHulls = new List<List<Point>> { convexHull };

            var pointsOnBestPath = ConnectTwoConvexHulls(convexHull, restPoints);

            pointsOnBestPath = Algorithms.TwoOpt(pointsOnBestPath);

            var totalDistance = (float)Algorithms.CalculatePathDistance(pointsOnBestPath);

            return (pointsOnBestPath, totalDistance, points[0], allConvexHulls);
        }

        public static (List<List<Point>> allConvexHulls, List<Point> restPoints) FindAllConvexHulls(List<Point> points)
        {
            List<List<Point>> allConvexHulls = new List<List<Point>>();
            var workingPoints = new List<Point>(points);
            var restPoints = new List<Point>(points);

            while (restPoints.Count > 3)
            {
                (List<Point> convexHull, List<Point> rp) = FindConvexHull(workingPoints);
                allConvexHulls.Add(convexHull);
                restPoints.RemoveAll(convexHull.Contains);
            }

            return (allConvexHulls, restPoints);
        }

        static (List<Point> convexHull, List<Point> restPoints) FindConvexHull(List<Point> points) //Graham's Scan
        {
            if (points.Count <= 3)
                return (points, new List<Point>());

            var homePoint = points[0];

            var workingPoints = new List<Point>(points);

            // Find the index of the point with the lowest Y value
            int indexOfLowestY = workingPoints
                .Select((point, index) => new { Point = point, Index = index })
                .Aggregate((p1, p2) => p1.Point.Y > p2.Point.Y ? p1 : p2)
                .Index;
            //Set lowest Y value point as PPoint
            Point PPoint = workingPoints[indexOfLowestY];

            //Calculate angles PPoint and all other points make with the positive X-axis
            List<PointWithAngle> pointsWithAngles = new List<PointWithAngle>();
            foreach (var point in workingPoints)
            {
                double angle = 180 + Math.Atan2(point.Y - PPoint.Y, point.X - PPoint.X) * 180 / Math.PI;
                pointsWithAngles.Add(new PointWithAngle(point, angle));
            }
            //Order pointsWithAngles according to their angle with PPoint and positive X-axis
            pointsWithAngles = pointsWithAngles.OrderBy(pwa => pwa.Angle).Reverse().ToList();


            //Stack to hold the points of the convex hull
            Stack<PointWithAngle> hullStack = new Stack<PointWithAngle>();

            // Push the first three points onto the stack
            hullStack.Push(pointsWithAngles[0]);
            hullStack.Push(pointsWithAngles[1]);
            hullStack.Push(pointsWithAngles[2]);

            //Process the remaining points
            for (int i = 3; i < pointsWithAngles.Count; i++)
            {
                //While the top of the stack and the next point make a right turn, pop the top
                while (hullStack.Count >= 2 && Orientation(NextToTop(hullStack), hullStack.Peek(), pointsWithAngles[i]) != 1)
                {
                    hullStack.Pop();
                }
                // Push the current point onto the stack
                hullStack.Push(pointsWithAngles[i]);
            }
            hullStack.Push(pointsWithAngles[0]);
            //The stack now contains the points in the convex hull

            List<Point> hullPoints = new List<Point>();
            List<Point> restPoints = new List<Point>(points);
            foreach (var pointWithAngle in hullStack)
            {
                hullPoints.Add(pointWithAngle.Point);
                restPoints.Remove(pointWithAngle.Point);
            }
            return (hullPoints, restPoints);
        }

        static int Orientation(PointWithAngle A, PointWithAngle B, PointWithAngle C)
        {
            //Calculate the cross product of vector AB and BC
            double value = (B.Point.Y - A.Point.Y) * (C.Point.X - B.Point.X) - (B.Point.X - A.Point.X) * (C.Point.Y - B.Point.Y);

            if (value == 0) return 0;  // Collinear
            return value > 0 ? 1 : -1;  // 1: Clockwise (right turn), -1: Counterclockwise (left turn)
        }

        static PointWithAngle NextToTop(Stack<PointWithAngle> stack)
        {
            PointWithAngle top = stack.Pop();
            PointWithAngle next = stack.Peek();
            stack.Push(top);
            return next;
        }

        private static List<Point> ConnectAllConvexHulls(List<List<Point>> allConvexHulls, List<Point> restPoints)
        {
            if (allConvexHulls == null || allConvexHulls.Count == 0)
                return new List<Point>();

            //Create a working copy to avoid modifying the input
            var workingHulls = allConvexHulls.Select(hull => new List<Point>(hull)).ToList();
            var currentPath = workingHulls[0];

            //Iteratively connect hulls
            for (int i = 1; i < workingHulls.Count; i++)
            {
                currentPath = ConnectTwoConvexHulls(currentPath, workingHulls[i]);
            }

            //Connect remaining points
            if (restPoints.Count > 0)
            {
                currentPath = ConnectTwoConvexHulls(currentPath, restPoints);
            }

            return currentPath;
        }

        private static List<Point> ConnectTwoConvexHulls(List<Point> outerHull, List<Point> innerPoints)
        {
            if (outerHull == null || outerHull.Count < 3)
                return new List<Point>();
            if (innerPoints == null || innerPoints.Count < 1)
                return outerHull;

            var distanceCache = new DistanceCache();
            var workingHull = new List<Point>(outerHull);
            var remainingPoints = new List<Point>(innerPoints);

            while (remainingPoints.Count > 0)
            {
                var (insertPoint, insertIndex) = FindBestInsertion(workingHull, remainingPoints, distanceCache);
                if (insertIndex == -1)
                    throw new InvalidOperationException("Failed to find valid insertion point");

                workingHull.Insert(insertIndex, insertPoint);
                remainingPoints.Remove(insertPoint);
            }

            return workingHull;
        }

        private static (Point point, int index) FindBestInsertion(List<Point> hull, List<Point> candidates, DistanceCache distanceCache)
        {
            double bestCost = double.MaxValue;
            Point bestPoint = new();
            int bestIndex = -1;

            foreach (var candidate in candidates)
            {
                for (int i = 0; i < hull.Count; i++)
                {
                    var next = (i + 1) % hull.Count;
                    var currentEdgeLength = distanceCache.GetDistance(hull[i], hull[next]);

                    // Calculate insertion cost
                    var insertionCost = distanceCache.GetDistance(hull[i], candidate) +
                                      distanceCache.GetDistance(candidate, hull[next]) -
                                      currentEdgeLength;

                    if (insertionCost < bestCost)
                    {
                        bestCost = insertionCost;
                        bestPoint = candidate;
                        bestIndex = next;
                    }
                }
            }

            return (bestPoint, bestIndex);
        }
    }
}
