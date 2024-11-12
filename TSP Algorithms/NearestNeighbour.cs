using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Algorithms
{
    public class NearestNeighbour
    {
        public const string Name = "Nearest Neighbour";

        public static (List<Point> points, float distance, Point homePoint) RunNearestNeighbourAlgo(List<Point> points)
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
    }
}
