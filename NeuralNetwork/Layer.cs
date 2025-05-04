using SymbolRecogniser.Other;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;

namespace SymbolRecogniser.NeuralNetwork
{
    class Layer
    {
        [NotNull] private int _neuronsInLayer;
        [NotNull] private int _neuronsInNextLayer;
        [NotNull] private int _neuronsInPreviousLayer;
        [NotNull] private Neuron[] _neurons;
        [NotNull] private bool _isInputLayer;
        [NotNull] private bool _isOutputLayer;
        private Layer _nextLayer;
        private Layer _previousLayer;

        public Layer(int neuronsInLayer, int neuronsInNextLayer, int previousInNextLayer, Layer nextLayer = null, Layer previousLayer = null, bool isInputLayer = false, bool isOutputLayer = false)
        {
            _neuronsInNextLayer = neuronsInNextLayer;
            _neuronsInPreviousLayer = previousInNextLayer;
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
            _previousLayer = previousLayer;
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
        public void SetPreviousLayer(Layer previousLayer)
        {
            _previousLayer = previousLayer;
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
        public void BackPropagate(double[] expectedOutput = null)
        {
            if (!_isOutputLayer)
            {
                for (int i = 0; i < _neuronsInLayer; i++)
                {
                    Neuron neuron = _neurons[i];
                    for (int j = 0; j < neuron.Weights.Length; j++)
                    {
                        // update weights going from this neuron to the next layer
                        neuron.Weights[j] += Parameters.LEARNING_RATE * _nextLayer._neurons[j].Delta * neuron.Output;
                    }
                    neuron.Bias += Parameters.LEARNING_RATE * neuron.Delta;
                }
            }
            else
            {
                for (int i = 0; i < _neuronsInLayer; i++)
                {
                    Neuron neuron = _neurons[i];
                    double error = 0;

                    for (int j = 0; j < _nextLayer._neurons.Length; j++)
                    {
                        if (i < _nextLayer._neurons[j].Weights.Length)
                        {
                            error += _nextLayer._neurons[j].Weights[i] * _nextLayer._neurons[j].Delta;
                        }
                    }

                    neuron.Delta = error * Utils.SigmoidDerivative(neuron.Output);
                }
            }

            if (!_isInputLayer)
            {
                for (int i = 0; i < _neuronsInLayer; i++)
                {
                    Neuron neuron = _neurons[i];
                    for (int j = 0; j < _previousLayer._neurons.Length; j++)
                    {
                        Neuron prevNeuron = _previousLayer._neurons[j];
                        prevNeuron.Weights[i] += Parameters.LEARNING_RATE * _neurons[i].Delta * prevNeuron.Output;
                    }
                    neuron.Bias += Parameters.LEARNING_RATE * neuron.Delta;
                }
            }
        }
    }
}