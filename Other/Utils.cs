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

        public static double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        // random int between min and max
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        // softmax function
        public static double[] Softmax(double[] values)
        {
            double max = values.Max();
            double sum = 0;
            double[] result = new double[values.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Math.Exp(values[i] - max);
                sum += result[i];
            }
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Math.Exp(result[i]) / sum;
            }
            return result;
        }

        // categorical cross entropy loss
        public static double CategoricalCrossEntropy(double[] expectedValues, double[] outputValues)
        {
            for (int i = 0; i < outputValues.Length; i++)
            {
                if (expectedValues[i] == 1)
                {
                    return -Math.Log(outputValues[i]);
                }
            }
            throw new Exception("No expected value is 1");
        }

        public static char GetCharFromArray(double[] outputValues, char[] chars)
        {
            double max = outputValues.Max();
            int index = Array.IndexOf(outputValues, max);
            return chars[index];
        }
    }
}
