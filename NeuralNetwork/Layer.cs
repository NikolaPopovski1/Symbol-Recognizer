using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace SymbolRecogniser.NeuralNetwork
{
    class Layer
    {
        [NotNull] private int _neuronsInLayer;
        [NotNull] private int _neuronsInNextLayer;
        [NotNull] private Neuron[] _neurons;
        [NotNull] private bool _isInputLayer;
        [NotNull] private bool _isOutputLayer;
        private Layer _nextLayer;

        public Layer(int neuronsInLayer, int neuronsInNextLayer, Layer nextLayer, bool isInputLayer = false, bool isOutputLayer = false)
        {
            _neuronsInNextLayer = neuronsInNextLayer;

            _neuronsInLayer = neuronsInLayer;
            _neurons = new Neuron[_neuronsInLayer];
            for (int i = 0; i < _neuronsInLayer; i++)
            {
                _neurons[i] = new Neuron(neuronsInNextLayer);
            }

            if (isInputLayer && isOutputLayer)
            {
                throw new Exception("Layer cannot be both input and output layer.");
            }
            _isInputLayer = isInputLayer;
            _isOutputLayer = isOutputLayer;

            _nextLayer = nextLayer;
        }
        public Neuron[] Neurons
        {
            get { return _neurons; }
            set { _neurons = value; }
        }



        public void InitInputLayer(List<Point> vectors)
        {
            if (!_isInputLayer)
            {
                throw new Exception("Layer is not input layer.");
            }
            if (vectors.Count != _neuronsInLayer)
            {
                throw new Exception("Number of vectors does not match number of neurons in layer.");
            }
            if (_neuronsInLayer % 2 != 0)
            {
                throw new Exception("Number of neurons in layer is not even.");
            }
            for (int i = 0; i < _neuronsInLayer; i += 2)
            {
                _neurons[i].Output = vectors[i].X;
                _neurons[i + 1].Output = vectors[i].Y;
            }
        }
        public void FeedForward()
        {
            if (_isOutputLayer) {
                return;
            }
            for (int i = 0; i < _neuronsInNextLayer; i++)
            {
                double result = 0;
                for (int j = 0; j < _neuronsInLayer; j++)
                {
                    result += _neurons[j].Output * _neurons[j].Weights[i];
                }
                _nextLayer.Neurons[i].Output = result + _nextLayer.Neurons[i].Bias;
            }
        }
    }
}