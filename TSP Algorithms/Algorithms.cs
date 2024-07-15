namespace TSP_Algorithms
{
    // All the code in this file is included in all platforms.
    public class Algorithms
    {
        public static (List<Point> points, float distance) BruteForce(List<Point> Points)
        {
            float minDist = float.MaxValue;
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

            return (pointsOnBestPath, minDist);
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
                    perm.Insert(0, p);
                    permutations.Add(perm);
                }
            }
            return permutations;
        }

        public static (List<Point> points, float distance) NearestNeighbour(List<Point> points)
        {
            if (points.Count <= 1)
                return (points, 0);

            var startPoint = points[0];
            var currentPoint = startPoint;
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
            totaldistance += (float)currentPoint.Distance(startPoint);
            pointsOnBestPath.Add(startPoint);
            return (pointsOnBestPath, totaldistance);
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
