using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Algorithms
{
    public class GeneticAlgorithm
    {
        public const string Name = "Genetic Algorithm";

        public static (List<Point> points, float distance, Point homePoint) RunAlgo(List<Point> points)
        {
            var population = GenerateRandomPopulation(points);

            int generationCount = 100;

            for (int i = 0; i < generationCount; i++)
            {
                population = EliminateHalfPopulation(population);
                population = ProduceNewGeneration(population);
                population = MutatePopulation(population);
            }

            double minDist = double.MaxValue;
            List<Point> shortestPath = new List<Point>();
            foreach (var path in population)
            {
                var dist = Algorithms.CalculatePathDistance(path);
                if (dist < minDist)
                {
                    minDist = dist;
                    shortestPath = path;
                }
            }

            return (shortestPath, (float)minDist, points[0]);
        }

        private static List<List<Point>> GenerateRandomPopulation(List<Point> points)
        {
            Random rnd = new Random();
            int randomPathCount = 5000;

            List<List<Point>> randomPaths = new List<List<Point>>();
            for (int i = 0; i < randomPathCount; i++)
            {
                var randomPathArr = points.ToArray();
                rnd.Shuffle(new Span<Point>(randomPathArr, 1, randomPathArr.Length - 1));
                List<Point> randomPath = randomPathArr.ToList();
                randomPath.Add(points[0]);
                randomPaths.Add(randomPath);
            }

            return randomPaths;
        }

        private static List<List<Point>> EliminateHalfPopulation(List<List<Point>> population)
        {
            List<List<Point>> survivors = new List<List<Point>>();

            int midway = (population.Count - 1) / 2;
            for (int i = 0; i <= midway; i++)
            {
                if (Algorithms.CalculatePathDistance(population[i]) < Algorithms.CalculatePathDistance(population[midway + i]))
                    survivors.Add(population[i]);
                else
                    survivors.Add(population[midway + i]);
            }

            return survivors;
        }

        private static List<List<Point>> ProduceNewGeneration(List<List<Point>> population)
        {
            List<List<Point>> offsprings = new List<List<Point>>();
            int midway = (population.Count - 1) / 2;

            for (int i = 0; i <= midway; i++)
            {
                List<Point> Parent1 = population[i];
                List<Point> Parent2 = population[midway + i];

                for (int j = 0; j < 2; j++)
                {
                    offsprings.Add(CreateOffspring(Parent1, Parent2));
                    offsprings.Add(CreateOffspring(Parent2, Parent1));
                }
            }

            return offsprings;
        }

        private static List<Point> CreateOffspring(List<Point> Parent1, List<Point> Parent2)
        {
            Random rnd = new Random();

            var offspring = new List<Point>();

            int genStart = rnd.Next(2, Parent1.Count - 2);
            int genEnd = rnd.Next(genStart, Parent2.Count - 1);

            List<Point> gen1 = Parent1.GetRange(genStart - 1, genEnd - genStart);
            List<Point> gen2 = Parent2.Except(gen1).ToList();
            gen2.Add(Parent2[0]);

            for (int i = 0; i < Parent1.Count; i++)
            {
                if (i >= genStart && i < genEnd)
                {
                    offspring.Add(gen1[0]);
                    gen1.RemoveAt(0);
                }
                else
                {
                    offspring.Add(gen2[0]);
                    gen2.RemoveAt(0);
                }
            }

            return offspring;
        }

        private static List<List<Point>> MutatePopulation(List<List<Point>> population)
        {
            Random rnd = new Random();

            List<List<Point>> mutatedPopulation = new List<List<Point>>();

            float chanceOfMutation = 0.01f;
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
