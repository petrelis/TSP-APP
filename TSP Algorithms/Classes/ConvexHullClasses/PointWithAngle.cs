using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Algorithms.Classes.ConvexHullClasses
{
    public class PointWithAngle
    {
        public PointWithAngle(Point point, double angle)
        {
            Point = point;
            Angle = angle;
        }

        public Point Point { get; set; }
        public double Angle { get; set; }
    }
}
