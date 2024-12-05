using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Algorithms.Classes.ConvexHullClasses
{
    public class DistanceCache
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
}
