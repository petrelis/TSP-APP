using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Algorithms.Classes
{
    public class ShortestPath
    {
        public List<Point> Points { get; set; }
        public float Distance { get; set; }
        public Point HomePoint { get; set; }
        public bool IsConvexHull { get; set; }
        public List<List<Point>>? AllConvexHulls { get; set; }

        public ShortestPath((List<Point> points, float distance, Point homePoint) data) 
        {
            Points = data.points;
            Distance = data.distance;
            HomePoint = data.homePoint;
            IsConvexHull = false;
            AllConvexHulls = new List<List<Point>>();
        }

        public ShortestPath((List<Point> points, float distance, Point homePoint, List<List<Point>>? allConvexHulls) data)
        {
            Points = data.points;
            Distance = data.distance;
            HomePoint = data.homePoint;
            IsConvexHull = true;
            AllConvexHulls = data.allConvexHulls;
        }
    }
}
