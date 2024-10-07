using Microsoft.Maui.Controls.Shapes;
using System.Linq;
using TSP_Algorithms.Classes;

namespace TSP_Algorithms
{
    // All the code in this file is included in all platforms.
    public class Algorithms
    {
        public static (List<Point> points, float distance, Point homePoint) BruteForce(List<Point> Points)
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
            //
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

        public static (List<Point> points, float distance, Point homePoint) NearestNeighbour(List<Point> points)
        {
            if (points.Count <= 1)
                return (points, 0, points[0]);

            var homePoint = points[0];
            var currentPoint = homePoint;
            List<Point> pointsOnBestPath = [currentPoint];
            float totaldistance = 0;

            while (pointsOnBestPath.Count != points.Count)
            {
                Point nearestPoint = new Point();
                float minDist = float.MaxValue;

                foreach (var point in points)
                {
                    if (!pointsOnBestPath.Contains(point))
                    {
                        float dist = (float)currentPoint.Distance(point);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            nearestPoint = point;
                        }
                    }
                }
                pointsOnBestPath.Add(nearestPoint);
                totaldistance += minDist;
                currentPoint = nearestPoint;
            }
            totaldistance += (float)currentPoint.Distance(homePoint);
            pointsOnBestPath.Add(homePoint);
            return (pointsOnBestPath, totaldistance, homePoint);
        }

        public static (List<Point> points, float distance, Point homePoint) CircleMethod(List<Point> points)
        {
            if (points.Count <= 1)
                return (points, 0, points[0]);

            var homePoint = points[0];

            // Find the index of the point with the lowest Y value
            int indexOfLowestY = points
                .Select((point, index) => new { Point = point, Index = index })
                .Aggregate((p1, p2) => p1.Point.Y > p2.Point.Y ? p1 : p2)
                .Index;

            // Swap the point with the lowest Y value with the first point in the list
            Point temp = points[0];
            points[0] = points[indexOfLowestY];
            points[indexOfLowestY] = temp;


            // Calculate centroid
            float centroidX = 0;
            float centroidY = 0;
            foreach (var point in points)
            {
                centroidX += (float)point.X;
                centroidY += (float)point.Y;
            }
            centroidX /= points.Count;
            centroidY /= points.Count;

            Point centroid = new Point(centroidX, centroidY);


            // Create pointsWithDistances (point, distance to centroid)
            List<PointWithDistance> pointsWithDistances = new List<PointWithDistance>();
            foreach (var point in points)
            {
                PointWithDistance pointWithDistance = new PointWithDistance
                {
                    Point = point,
                    DistanceToCentroid = point.Distance(centroid)
                };
                pointsWithDistances.Add(pointWithDistance);
            }

            var currentPoint = pointsWithDistances[0];
            List<Point> pointsOnBestPath = [currentPoint.Point];
            float totaldistance = 0;
            // Loop through points until path has all points
            while (pointsOnBestPath.Count != points.Count)
            {
                // init values
                float minDistScore = float.MaxValue;
                float minDist = float.MaxValue;
                PointWithDistance nearestPoint = new PointWithDistance();

                // loop through all points
                foreach (var point in pointsWithDistances)
                {
                    // skip if point is already on path
                    if (!pointsOnBestPath.Contains(point.Point))
                    {
                        // distance from current point to point in this loop
                        float dist = (float)currentPoint.Point.Distance(point.Point);

                        // distance from current point to point in this loop + difference between distances to the two points and the centroid
                        float distScore = (float)Math.Pow(dist+(float)(currentPoint.DistanceToCentroid-point.DistanceToCentroid), 2);
                        // if distScore is less than minDistScore set nearestPoint to point
                        if (distScore < minDistScore)
                        {
                            minDistScore = distScore;
                            nearestPoint = point;
                            minDist = dist;
                        }
                    }
                }
                // Add nearestPoint to bestPath, set is as currentPoint and add distance
                pointsOnBestPath.Add(nearestPoint.Point);
                totaldistance += minDist;
                currentPoint = nearestPoint;
            }
            totaldistance += (float)currentPoint.Point.Distance(points[0]);
            pointsOnBestPath.Add(points[0]);
            return (pointsOnBestPath, totaldistance, homePoint);
        }

        public static (List<Point> points, float distance, Point homePoint) ConvexHull(List<Point> points)
        {
            if (points.Count <= 1)
                return (points, 0, points[0]);

            var homePoint = points[0];

            // Find the index of the point with the lowest Y value
            int indexOfLowestY = points
                .Select((point, index) => new { Point = point, Index = index })
                .Aggregate((p1, p2) => p1.Point.Y > p2.Point.Y ? p1 : p2)
                .Index;

            // Swap the point with the lowest Y value with the first point in the list
            Point temp = points[0];
            points[0] = points[indexOfLowestY];
            points[indexOfLowestY] = temp;

            Point PPoint = points[0];

            List<PointWithAngle> pointsWithAngles = new List<PointWithAngle> ();
            foreach (var point in points)
            {
                bool turnDirectionLeft = false;
                if(point.X < PPoint.X) turnDirectionLeft = true;
                double angle = Math.Asin((point.Y - PPoint.Y) / PPoint.Distance(point));
                //if (!turnDirectionLeft) angle = 180 - angle;
                PointWithAngle pointWithAngle = new PointWithAngle
                {
                    Point = point,
                    Angle = angle
                };
                pointsWithAngles.Add(pointWithAngle);
            }

            pointsWithAngles = pointsWithAngles.OrderBy(pwa => pwa.Angle).ToList();


            List<Point> pointsOnBestPath = new List<Point> ();
            foreach(var pwa in pointsWithAngles)
            {
                pointsOnBestPath.Add(pwa.Point);
            }

            return (pointsOnBestPath, 0, homePoint);
        }

        public static Point GenerateRandomPoint(int mult)
        {
            Random rnd = new Random();

            float x = mult - rnd.NextSingle() * mult * 2;
            float y = mult - rnd.NextSingle() * mult * 2;

            return new Point(x, y);

        }
    }
}
