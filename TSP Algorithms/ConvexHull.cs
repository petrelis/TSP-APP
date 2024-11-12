using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP_Algorithms.Classes;
using TSP_Algorithms.Classes.ConvexHullClasses;

namespace TSP_Algorithms
{
    public class ConvexHull //Graham's scan
    {
        public const string Name = "Convex Hull";

        public static (List<Point> points, float distance, Point homePoint, List<List<Point>> allHulls) RunConvexHullAlgo(List<Point> points)
        {
            (List<List<Point>> allConvexHulls, List<Point> restPoints) = FindAllConvexHulls(points);

            List<Point> pointsOnBestPath = new List<Point>();

            for (int i = 0; i < allConvexHulls.Count - 1; i++)
            {
                var pointstoConnect = allConvexHulls[i];
                pointstoConnect.AddRange(allConvexHulls[i + 1]);

                ShortestPath path = new ShortestPath(BruteForce.RunBruteForceAlgo(pointstoConnect));

                pointsOnBestPath.AddRange(path.Points);
            }

            var pointsToConnect = allConvexHulls[^1];
            pointsToConnect.AddRange(restPoints);

            ShortestPath lastPath = new ShortestPath(BruteForce.RunBruteForceAlgo(pointsToConnect));

            pointsOnBestPath.AddRange(lastPath.Points);


            //var allVisibilityVertices = FindVisibilityVertices(allConvexHulls);

            //foreach (var hull in allVisibilityVertices)
            //{
            //    foreach (var visibilityVertex in hull)
            //    {

            //    }
            //}

            //foreach (var convHull in allConvexHulls)
            //{
            //    foreach (var point in convHull)
            //    {
            //        pointsOnBestPath.Add(point);
            //    }
            //}

            //foreach(var restPoint in  restPoints)
            //{
            //    pointsOnBestPath.Add(restPoint);
            //}

            return (pointsOnBestPath, 0, pointsOnBestPath[0], allConvexHulls);
        }

        static (List<List<Point>>, List<Point>) FindAllConvexHulls(List<Point> points)
        {
            List<List<Point>> allConvexHulls = new List<List<Point>>();
            var restPoints = points;

            while (restPoints.Count > 3)
            {
                List<Point> convexHull = FindConvexHull(points);
                allConvexHulls.Add(convexHull);
                restPoints.RemoveAll(convexHull.Contains);
            }

            return (allConvexHulls, restPoints);
        }

        static List<Point> FindConvexHull(List<Point> points)
        {
            if (points.Count <= 3)
                return (points);

            var homePoint = points[0];

            // Find the index of the point with the lowest Y value
            int indexOfLowestY = points
                .Select((point, index) => new { Point = point, Index = index })
                .Aggregate((p1, p2) => p1.Point.Y > p2.Point.Y ? p1 : p2)
                .Index;
            //Set lowest Y value point as PPoint
            Point PPoint = points[indexOfLowestY];

            //Calculate angles PPoint and all other points make with the positive X-axis
            List<PointWithAngle> pointsWithAngles = new List<PointWithAngle>();
            foreach (var point in points)
            {
                double angle = 180 + Math.Atan2(point.Y - PPoint.Y, point.X - PPoint.X) * 180 / Math.PI;
                pointsWithAngles.Add(new PointWithAngle(point, angle));
            }
            //Order pointsWithAngles according to their angle with PPoint and positive X-axis
            pointsWithAngles = pointsWithAngles.OrderBy(pwa => pwa.Angle).Reverse().ToList();


            // Stack to hold the points of the convex hull
            Stack<PointWithAngle> hullStack = new Stack<PointWithAngle>();

            // Push the first three points onto the stack
            hullStack.Push(pointsWithAngles[0]);
            hullStack.Push(pointsWithAngles[1]);
            hullStack.Push(pointsWithAngles[2]);

            // Process the remaining points
            for (int i = 3; i < pointsWithAngles.Count; i++)
            {
                // While the top of the stack and the next point make a right turn, pop the top
                while (hullStack.Count >= 2 && Orientation(NextToTop(hullStack), hullStack.Peek(), pointsWithAngles[i]) != 1)
                {
                    hullStack.Pop();
                }
                // Push the current point onto the stack
                hullStack.Push(pointsWithAngles[i]);
            }
            hullStack.Push(pointsWithAngles[0]);
            // The stack now contains the points in the convex hull

            List<Point> hullPoints = new List<Point>();
            foreach (var pointWithAngle in hullStack)
            {
                hullPoints.Add(pointWithAngle.Point);
            }
            return hullPoints;
        }

        static int Orientation(PointWithAngle A, PointWithAngle B, PointWithAngle C)
        {
            // Calculate the cross product of vector AB and BC
            double value = ((B.Point.Y - A.Point.Y) * (C.Point.X - B.Point.X) - (B.Point.X - A.Point.X) * (C.Point.Y - B.Point.Y));

            if (value == 0) return 0;  // Collinear
            return (value > 0) ? 1 : -1;  // 1: Clockwise (right turn), -1: Counterclockwise (left turn)
        }

        static PointWithAngle NextToTop(Stack<PointWithAngle> stack)
        {
            PointWithAngle top = stack.Pop();
            PointWithAngle next = stack.Peek();
            stack.Push(top);
            return next;
        }

        static List<List<VisibilityVertex>> FindVisibilityVertices(List<List<Point>> allConvexHulls)
        {
            List<List<VisibilityVertex>> allVisibilityVertices = new List<List<VisibilityVertex>>();
            for (int i=0; i<allConvexHulls.Count - 1; i++)
            {
                allVisibilityVertices.Add(new List<VisibilityVertex>());
                //each outer convexhull point
                foreach (var ppoint in allConvexHulls[i])
                {
                    List<PointWithAngle> pointsWithAngles = new List<PointWithAngle>();

                    //Calculate the angle, that the outer convexhull point(ppoint) makes with each inner convex hull point and the positive X-axis
                    foreach(var point in allConvexHulls[i+1])
                    {
                        double angle = 180 + Math.Atan2(point.Y - ppoint.Y, point.X - ppoint.X) * 180 / Math.PI;
                        pointsWithAngles.Add(new PointWithAngle(point, angle));
                    }
                    //Order pointsWithAngles according to their angle with ppoint and positive X-axis
                    pointsWithAngles = pointsWithAngles.OrderBy(pwa => pwa.Angle).ToList();

                    //Create a list of visible points and add the points with the lowest and highest angles, since they will be visible in any point arrangement
                    List<Point> visiblePoints = new List<Point>();
                    visiblePoints.Add(pointsWithAngles[0].Point);
                    visiblePoints.Add(pointsWithAngles[^1].Point);

                    //The line past which points are not visible from base point
                    List<Point> line = new List<Point>();
                    line.Add(pointsWithAngles[0].Point);
                    line.Add(pointsWithAngles[^1].Point);

                    //Add all visible points to visibilityVertex points list
                    foreach (var pwa in pointsWithAngles)
                    {
                        var testPoint = pwa.Point;
                        if(!visiblePoints.Contains(testPoint) && ppoint.Distance(testPoint) > 0) 
                        {
                            if(!IsPointBeyondLine(ppoint, testPoint, line))
                            {
                                visiblePoints.Add(testPoint);
                            }
                        }
                    }
                    allVisibilityVertices[i].Add(new VisibilityVertex(ppoint, visiblePoints));
                }
            }

            return allVisibilityVertices;
        }

        public static bool IsPointBeyondLine(Point basePoint, Point targetPoint, List<Point> line)
        {
            var linePoint1 = line[0];
            var linePoint2 = line[1];

            // Calculate vectors
            double dx1 = linePoint2.X - linePoint1.X;
            double dy1 = linePoint2.Y - linePoint1.Y;

            double dx2 = targetPoint.X - linePoint1.X;
            double dy2 = targetPoint.Y - linePoint1.Y;

            double dx3 = basePoint.X - linePoint1.X;
            double dy3 = basePoint.Y - linePoint1.Y;

            // Calculate cross products
            double crossTarget = dx1 * dy2 - dy1 * dx2;
            double crossBase = dx1 * dy3 - dy1 * dx3;

            // Check if target point is on the opposite side of the line relative to the base point
            return crossTarget * crossBase < 0;
        }
    }
}
