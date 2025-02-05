﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP_Algorithms.ConvexHull;

namespace TSP_Algorithms
{
    public class GeneticAlgorithm : ConvexHullGeneral
    {
        public const string Name = "Genetic Algorithm";

        public static (List<Point> points, float distance, Point homePoint) RunAlgo(List<Point> points)
        {
            var workingPoints = new List<Point>(points);
            var population = GenerateConvexHullPopulation(workingPoints);

            int generationCount = 2000;
            if (points.Count > 1500)
                generationCount = points.Count;

            for (int i = 0; i < generationCount; i++)
            {
                population = EliminateHalfPopulation(population);
                population = ProduceNewGeneration(population);
                population = MutatePopulation(population);

                /*
                double minDistt = double.MaxValue;
                List<Point> shortestPathh = new List<Point>();
                foreach (var path in population)
                {
                    var dist = Algorithms.CalculatePathDistance(path);
                    if (dist < minDistt)
                    {
                        minDistt = dist;
                        shortestPathh = path;
                    }
                }
                */
            }

            double minDist = double.MaxValue;
            List<Point> shortestPath = new List<Point>();
            object lockObject = new object(); // Lock object to handle critical sections
            Parallel.ForEach(population, path =>
            {
                var dist = Algorithms.CalculatePathDistance(path);
                lock (lockObject)
                {
                    if (dist < minDist)
                    {
                        minDist = dist;
                        shortestPath = path;
                    }
                }
            });


            shortestPath = Algorithms.TwoOpt(shortestPath);
            minDist = Algorithms.CalculatePathDistance(shortestPath);

            return (shortestPath, (float)minDist, points[0]);
        }

        private static List<List<Point>> GenerateRandomPopulation(List<Point> points)
        {
            Random rnd = new Random();
            int randomPathCount = 1000;
            if (points.Count > randomPathCount * 10)
            {
                randomPathCount = points.Count / 10;
            }
            var workingPoints = new List<Point>(points);

            List<List<Point>> randomPaths = new List<List<Point>>();
            Parallel.For(0, randomPathCount, i =>
            {
                var randomPathArr = workingPoints.ToArray();
                rnd.Shuffle(new Span<Point>(randomPathArr, 1, randomPathArr.Length - 1));
                List<Point> randomPath = randomPathArr.ToList();
                randomPath.Add(workingPoints[0]);
                randomPaths.Add(randomPath);
            });

            return randomPaths;
        }

        private static List<List<Point>> GenerateConvexHullPopulation(List<Point> points)
        {
            Random rnd = new Random();
            int randomPathCount = 1000;
            if (points.Count > randomPathCount * 10)
            {
                randomPathCount = points.Count / 10;
            }
            var workingPoints = new List<Point>(points);

            (List<Point> convexHull, List<Point> restPoints) = FindConvexHull(workingPoints);

            //Create and fill rp with first and last convexHull points and all restPoints and empty positions for other convexHull points
            var convexHullNull = convexHull.Skip(1).Take(convexHull.Count - 2).ToList();
            for(int i = 0; i  < convexHullNull.Count; i++)
            {
                convexHullNull[i] = new Point(0, 0);
            }
            restPoints.AddRange(convexHullNull);
            var rp = new List<Point>();
            rp.Add(convexHull[0]);
            rp.AddRange(restPoints);
            rp.Add(convexHull[^1]);
            Point checkPoint = new Point(0, 0);

            List<List<Point>> randomPaths = new List<List<Point>>();
            for(int i = 0; i < randomPathCount; i++)
            {
                var randomPathArr = rp.ToArray();
                rnd.Shuffle(new Span<Point>(randomPathArr, 1, randomPathArr.Length - 2));
                int k = 1;
                for (int j = 0; j < randomPathArr.Length; j++)
                {
                    if (randomPathArr[j] == checkPoint)
                    {
                        randomPathArr[j] = convexHull[k];
                        k++;
                    }
                }
                List<Point> randomPath = randomPathArr.ToList();
                randomPaths.Add(randomPath);
            }

            return randomPaths;
        }


        private static List<List<Point>> EliminateHalfPopulation(List<List<Point>> population)
        {
            List<List<Point>> survivors = new List<List<Point>>();
            int midway = (population.Count - 1) / 2;
            object lockObj = new object(); //To synchronize access to the survivors list

            Parallel.For(0, midway + 1, i =>
            {
                if (population[i] == null || population[midway + i] == null)
                    return;

                List<Point> betterPath = Algorithms.CalculatePathDistance(population[i]) <
                                         Algorithms.CalculatePathDistance(population[midway + i])
                    ? population[i]
                    : population[midway + i];

                lock (lockObj)
                {
                    survivors.Add(betterPath);
                }
            });

            return survivors;
        }

        private static List<List<Point>> ProduceNewGeneration(List<List<Point>> population)
        {
            ConcurrentBag<List<Point>> offsprings = new ConcurrentBag<List<Point>>();
            int midway = (population.Count - 1) / 2;

            Parallel.For(0, midway + 1, i =>
            {
                List<Point> Parent1 = population[i];
                List<Point> Parent2 = population[midway + i];

                for (int j = 0; j < 2; j++)
                {
                    offsprings.Add(UniformCrossover(Parent1, Parent2));
                    offsprings.Add(UniformCrossover(Parent2, Parent1));
                }
            });

            return offsprings.ToList();
        }

        //private static List<List<Point>> ProduceNewGeneration(List<List<Point>> population) //non parallel version
        //{
        //    List<List<Point>> offsprings = new List<List<Point>>();
        //    int midway = (population.Count - 1) / 2;

        //    for (int i = 0; i < midway + 1; i++)
        //    {
        //        List<Point> Parent1 = population[i];
        //        List<Point> Parent2 = population[midway + i];

        //        for (int j = 0; j < 2; j++)
        //        {
        //            offsprings.Add(UniformCrossover(Parent1, Parent2));
        //            offsprings.Add(UniformCrossover(Parent2, Parent1));
        //        }
        //    }

        //    return offsprings;
        //}


        private static List<Point> UniformCrossover(List<Point> Parent1, List<Point> Parent2)
        {
            if (Parent1.Count != Parent2.Count)
                throw new ArgumentException("Both parents must have the same number of elements.");

            int length = Parent1.Count;
            Point[] offspring = new Point[length];

            Random rnd = new Random();

            //Generate random binary mask
            bool[] binaryMask = new bool[length];
            for (int i = 1; i < length - 1; i++)
            {
                binaryMask[i] = rnd.Next(2) == 1;
            }

            //Arrayed copies to avoid modifying original data
            var p1 = Parent1.ToArray();
            var p2 = Parent2.ToList();

            //Fill offspring with p1 elements where binaryMask is true
            for (int i = 0; i < length; i++)
            {
                if (binaryMask[i] == true)
                {
                    offspring[i] = p1[i];
                    p2.Remove(p1[i]);
                }
            }

            //Fill remaining empty positions in offsprint with p2 elements
            for (int i = 0; i < length; i++)
            {
                if (offspring[i] == new Point(0, 0))
                {
                    offspring[i] = p2[0];
                    p2.RemoveAt(0);
                }
            }

            return offspring.ToList();
        }

        private static List<List<Point>> MutatePopulation(List<List<Point>> population)
        {
            Random rnd = new Random();

            List<List<Point>> mutatedPopulation = new List<List<Point>>();

            float chanceOfMutation = 0.05f;
            foreach (var path in population)
            {
                if (rnd.NextSingle() < chanceOfMutation)
                {
                    int index1 = rnd.Next(1, path.Count - 1);
                    int index2 = rnd.Next(1, path.Count - 1);

                    Point temp = path[index1];
                    path[index1] = path[index2];
                    path[index2] = temp;
                }
                mutatedPopulation.Add(path);
            }

            return mutatedPopulation;
        }
    }
}
