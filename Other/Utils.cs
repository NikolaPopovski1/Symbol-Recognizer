using System.Windows;

namespace SymbolRecogniser.Other
{
    public static class Utils
    {
        public static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
        public static Point CenterVector(Point p1, Point p2)
        {
            return new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
        }
        // random number between min and max
        public static double RandomWeight(int min, int max)
        {
            Random random = new Random();
            return random.NextDouble() * (max - min) + min;
        }
        // random double between min and max
        public static double GetRandomDouble(double min, double max)
        {
            Random random = new Random();
            return random.NextDouble() * (max - min) + min;
        }
    }
}
