using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP_Algorithms.Classes;

namespace TSP_Algorithms
{
    public class CircleMethod
    {
        public const string Name = "Circle Method";

        public static (List<Point> points, float distance, Point homePoint) RunCircleMethodAlgo(List<Point> points)
        {
            if (points.Count <= 1)
                return (points, 0, points[0]);

            var workingPoints = new List<Point>(points);

            // Find the index of the point with the lowest Y value
            int indexOfLowestY = workingPoints
                .Select((point, index) => new { Point = point, Index = index })
                .Aggregate((p1, p2) => p1.Point.Y > p2.Point.Y ? p1 : p2)
                .Index;

            // Swap the point with the lowest Y value with the first point in the list
            Point temp = workingPoints[0];
            workingPoints[0] = workingPoints[indexOfLowestY];
            workingPoints[indexOfLowestY] = temp;


            // Calculate centroid
            float centroidX = 0;
            float centroidY = 0;
            foreach (var point in workingPoints)
            {
                centroidX += (float)point.X;
                centroidY += (float)point.Y;
            }
            centroidX /= workingPoints.Count;
            centroidY /= workingPoints.Count;

            Point centroid = new Point(centroidX, centroidY);


            // Create pointsWithDistances (point, distance to centroid)
            List<PointWithDistance> pointsWithDistances = new List<PointWithDistance>();
            foreach (var point in workingPoints)
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
            while (pointsOnBestPath.Count != workingPoints.Count)
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
                        float distScore = (float)Math.Pow(dist + (float)(currentPoint.DistanceToCentroid - point.DistanceToCentroid), 2);
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
            pointsOnBestPath.Add(workingPoints[0]);
            pointsOnBestPath = Algorithms.TwoOpt(pointsOnBestPath);
            var totalDistance = (float)Algorithms.CalculatePathDistance(pointsOnBestPath);
            return (pointsOnBestPath, totalDistance, points[0]);
        }
    }
}
