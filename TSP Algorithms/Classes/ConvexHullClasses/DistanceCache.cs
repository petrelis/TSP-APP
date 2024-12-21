using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Algorithms.Classes.ConvexHullClasses
{
    public class DistanceCache
    {
        private readonly ConcurrentDictionary<(Point, Point), double> _cache = new();

        public double GetDistance(Point p1, Point p2)
        {
            var key = (p1, p2);

            // Try to get the distance from the cache first
            if (!_cache.TryGetValue(key, out double distance))
            {
                // If not found, calculate the distance and add it to the cache
                distance = p1.Distance(p2);
                _cache[key] = distance;
            }

            return distance;
        }
    }
}
