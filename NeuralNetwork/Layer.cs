using SymbolRecogniser.Other;
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

        public Layer(int neuronsInLayer, int neuronsInNextLayer, Layer nextLayer = null, bool isInputLayer = false, bool isOutputLayer = false)
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



        public void SetNextLayer(Layer nextLayer)
        {
            _nextLayer = nextLayer;
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
                result += _nextLayer.Neurons[i].Bias;
                result = Utils.Sigmoid(result);

                _nextLayer.Neurons[i].Output = result;
            }
        }
    }
}