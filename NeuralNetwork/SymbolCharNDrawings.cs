using SymbolRecogniser.Other;
using System.Windows;

namespace SymbolRecogniser.NeuralNetwork
{
    public class SymbolCharNDrawings
    {
        private char _symbol = ' ';
        private List<List<Point>> _drawing = new List<List<Point>>(); // lahko bi naredil z arrayem, da bi se znebil overheada ampak mi je zmanjkalo časa

        public SymbolCharNDrawings(char symbol)
        {
            _symbol = symbol;
        }
        public char Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }
        public List<List<Point>> Drawings
        {
            get { return _drawing; }
            set { _drawing = value; }
        }
        public int DrawingsCount
        {
            get { return _drawing.Count; }
        }



        public bool AddDrawing(List<Point> drawing)
        {
            if (drawing.Count > Parameters.NUM_OF_POINTS)
            {
                drawing = Normalize(ReduceSize(drawing));
            }
            else if (drawing.Count < Parameters.NUM_OF_POINTS)
            {
                return false;
            }
            _drawing.Add(drawing);
            return true;
        }
        /*
        public List<Point> ReduceSize(List<Point> drawings)
        {
            List<Point> result = new List<Point>(drawings);
            List<KeyValuePair<double, int>> distances = new List<KeyValuePair<double, int>>();

            for (int i = 1; i < drawings.Count - 2; i++)
            {
                distances.Add(new KeyValuePair<double, int>(Utils.Distance(drawings[i], drawings[i + 1]), i));
            }
            // sort distances from smallest to largest key
            distances.Sort((x, y) => x.Key.CompareTo(y.Key));

            int numToCombine = result.Count - Parameters.NUM_OF_POINTS - 3;
            for (int i = 0; i < numToCombine; i++)
            {
                int index = distances[i].Value;

                Point newPoint = Utils.CenterVector(drawings[index], drawings[index + 1]);
                
                result.Insert(index, newPoint);
            }

            return result;
        }
        */
        public List<Point> ReduceSize(List<Point> drawings)
        {
            List<Point> result = new List<Point>(drawings);
            List<KeyValuePair<double, int>> distances = new List<KeyValuePair<double, int>>();
            while (result.Count != Parameters.NUM_OF_POINTS + 1)
            {
                for (int i = 1; i < result.Count - 2; i++)
                {
                    distances.Add(new KeyValuePair<double, int>(Utils.Distance(result[i], result[i + 1]), i));
                }
                distances.Sort((x, y) => x.Key.CompareTo(y.Key));

                int index = distances[0].Value;

                Point newPoint = Utils.CenterVector(result[index], result[index + 1]);
                result.RemoveAt(index);
                result.RemoveAt(index + 1);
                result.Insert(index, newPoint);

                distances.Clear();
            }

            return result;
        }
        public List<Point> Normalize(List<Point> drawings)
        {
            List<Point> vectorInputs = new List<Point>();
            for (int i = 0; i < Parameters.NUM_OF_POINTS; i++)
            {
                Vector v = drawings[i + 1] - drawings[i];
                double length = v.Length;

                double normX = length > 0 ? v.X / length : 0;
                double normY = length > 0 ? v.Y / length : 0;

                vectorInputs.Add(new Point(normX, normY));
            }
            return vectorInputs;
        }
    }
}
