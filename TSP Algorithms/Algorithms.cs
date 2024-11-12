using Microsoft.Maui.Controls.Shapes;
using System.Linq;
using TSP_Algorithms.Classes;

namespace TSP_Algorithms
{
    public class Algorithms
    {
        public static Point GenerateRandomPoint(int mult)
        {
            Random rnd = new Random();

            float x = mult - rnd.NextSingle() * mult * 2;
            float y = mult - rnd.NextSingle() * mult * 2;

            return new Point(x, y);
        }
    }
}
