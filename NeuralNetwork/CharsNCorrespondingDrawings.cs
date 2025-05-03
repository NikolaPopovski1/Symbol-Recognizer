using System.Windows;

namespace SymbolRecogniser.NeuralNetwork
{
    class CharsNCorrespondingDrawings
    {
        private List<SymbolCharNDrawings> _listOfSymbolCharNDrawings;

        public CharsNCorrespondingDrawings()
        {
            _listOfSymbolCharNDrawings = new List<SymbolCharNDrawings>();
        }

        public List<SymbolCharNDrawings> List
        {
            get { return _listOfSymbolCharNDrawings; }
            set { _listOfSymbolCharNDrawings = value; }
        }



        public int SymbolCount
        {
            get { return _listOfSymbolCharNDrawings.Count; }
        }
        public int DrawingsCount
        {
            get
            {
                int count = 0;
                foreach (SymbolCharNDrawings symbolCharNDrawings in _listOfSymbolCharNDrawings)
                {
                    count += symbolCharNDrawings.DrawingsCount;
                }
                return count;
            }
        }
        public bool Add(List<Point> drawing, char symbol)
        {
            if (Contains(symbol))
            {
                foreach (SymbolCharNDrawings symbolCharNDrawingsInner in _listOfSymbolCharNDrawings)
                {
                    if (symbolCharNDrawingsInner.Symbol == symbol)
                    {
                        return symbolCharNDrawingsInner.AddDrawing(drawing);
                    }
                }
            }
            else
            {
                SymbolCharNDrawings symbolCharNOneDrawing = new SymbolCharNDrawings(symbol);
                if (!symbolCharNOneDrawing.AddDrawing(drawing))
                {
                    return false;
                }
                _listOfSymbolCharNDrawings.Add(symbolCharNOneDrawing);
                return true;
            }
            return false;
        }
        private bool Contains(char symbol)
        {
            foreach (SymbolCharNDrawings symbolCharNDrawings in _listOfSymbolCharNDrawings)
            {
                if (symbolCharNDrawings.Symbol == symbol)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
