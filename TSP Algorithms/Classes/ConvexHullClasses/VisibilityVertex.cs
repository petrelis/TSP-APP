using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Algorithms.Classes.ConvexHullClasses
{
    public class VisibilityVertex
    {
        public Point BasePoint { get; set; }
        public List<Point> VisiblePoints { get; set; }

        public VisibilityVertex(Point point, List<Point> points)
        {
            BasePoint = point;
            VisiblePoints = points;
        }
    }
}
