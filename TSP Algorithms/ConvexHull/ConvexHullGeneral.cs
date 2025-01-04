using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP_Algorithms.Classes;
using TSP_Algorithms.Classes.ConvexHullClasses;

namespace TSP_Algorithms.ConvexHull
{
    public abstract class ConvexHullGeneral
    {
        protected static (List<List<Point>> allConvexHulls, List<Point> restPoints) FindAllConvexHulls(List<Point> points)
        {
            List<List<Point>> allConvexHulls = new List<List<Point>>();
            var restPoints = new List<Point>(points);

            while (restPoints.Count > 3)
            {
                (List<Point> convexHull, List<Point> rp) = FindConvexHull(restPoints);
                allConvexHulls.Add(convexHull);
                restPoints = rp;
            }

            return (allConvexHulls, restPoints);
        }

        protected static (List<Point> convexHull, List<Point> restPoints) FindConvexHull(List<Point> points) //Graham's Scan
        {
            if (points.Count <= 3)
                return (points, new List<Point>());

            var homePoint = points[0];

            var workingPoints = new List<Point>(points);

            double lowestY = double.MaxValue;
            int indexOfLowestY = 0;

            Parallel.For(1, workingPoints.Count, i =>
            {
                double y = workingPoints[i].Y;
                double x = workingPoints[i].X;

                lock (workingPoints)
                {
                    if (y < lowestY || (y == lowestY && x < workingPoints[indexOfLowestY].X))
                    {
                        lowestY = y;
                        indexOfLowestY = i;
                    }
                }
            });
            //Set lowest Y value point as PPoint
            Point PPoint = workingPoints[indexOfLowestY];

            ConcurrentBag<PointWithAngle> pointsWithAnglesCBag = new ConcurrentBag<PointWithAngle>();

            Parallel.ForEach(workingPoints, point =>
            {
                double angle = 180 + Math.Atan2(point.Y - PPoint.Y, point.X - PPoint.X) * 180 / Math.PI;
                pointsWithAnglesCBag.Add(new PointWithAngle(point, angle));
            });

            var pointsWithAngles = pointsWithAnglesCBag.ToList();

            //Order pointsWithAngles according to their angle with PPoint and positive X-axis
            pointsWithAngles = pointsWithAngles.OrderBy(pwa => pwa.Angle).Reverse().ToList();


            //Stack to hold the points of the convex hull
            Stack<PointWithAngle> hullStack = new Stack<PointWithAngle>();

            //Push the first three points onto the stack
            hullStack.Push(pointsWithAngles[0]);
            hullStack.Push(pointsWithAngles[1]);
            hullStack.Push(pointsWithAngles[2]);

            //Process the remaining points
            for (int i = 2; i < pointsWithAngles.Count; i++)
            {
                //While top point of the stack and the next point make a right turn pop the top point
                while (hullStack.Count >= 2 && Orientation(NextToTop(hullStack), hullStack.Peek(), pointsWithAngles[i]) != 1)
                {
                    hullStack.Pop();
                }
                //Push the current point onto the stack
                hullStack.Push(pointsWithAngles[i]);
            }
            hullStack.Push(pointsWithAngles[0]);

            List<Point> hullPoints = new List<Point>();
            List<Point> restPoints = new List<Point>(points);
            foreach (var pointWithAngle in hullStack)
            {
                hullPoints.Add(pointWithAngle.Point);
                restPoints.Remove(pointWithAngle.Point);
            }
            return (hullPoints, restPoints);
        }

        private static int Orientation(PointWithAngle A, PointWithAngle B, PointWithAngle C)
        {
            // Calculate the cross product of vector AB and BC
            double value = (B.Point.Y - A.Point.Y) * (C.Point.X - B.Point.X) - (B.Point.X - A.Point.X) * (C.Point.Y - B.Point.Y);

            if (value == 0) return 0;  // Collinear
            return value > 0 ? 1 : -1;  // 1: Clockwise (right turn), -1: Counterclockwise (left turn)
        }

        private static PointWithAngle NextToTop(Stack<PointWithAngle> stack)
        {
            PointWithAngle top = stack.Pop();
            PointWithAngle next = stack.Peek();
            stack.Push(top);
            return next;
        }

        protected static List<Point> ConnectAllConvexHulls(List<List<Point>> allConvexHulls, List<Point> restPoints)
        {
            if (allConvexHulls == null || allConvexHulls.Count == 0)
                return new List<Point>();

            var workingHulls = new List<List<Point>>(allConvexHulls);
            var currentPath = workingHulls[0];

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

        protected static List<Point> ConnectTwoConvexHulls(List<Point> outerHull, List<Point> innerPoints)
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
                if (insertIndex != -1)
                {
                    workingHull.Insert(insertIndex, insertPoint);
                    remainingPoints.Remove(insertPoint);
                }
            }

            return workingHull;
        }

        private static (Point point, int index) FindBestInsertion(List<Point> hull, List<Point> candidates, DistanceCache distanceCache)
        {
            object lockObject = new object();
            double bestCost = double.MaxValue;
            Point bestPoint = new();
            int bestIndex = -1;

            Parallel.ForEach(candidates, candidate =>
            {
                for (int i = 0; i < hull.Count; i++)
                {
                    var next = (i + 1) % hull.Count;
                    var currentEdgeLength = distanceCache.GetDistance(hull[i], hull[next]);

                    var insertionCost = distanceCache.GetDistance(hull[i], candidate) +
                                        distanceCache.GetDistance(candidate, hull[next]) -
                                        currentEdgeLength;

                    lock (lockObject)
                    {
                        if (insertionCost < bestCost)
                        {
                            bestCost = insertionCost;
                            bestPoint = candidate;
                            bestIndex = next;
                        }
                    }
                }
            });

            return (bestPoint, bestIndex);
        }
    }
}
