using SymbolRecogniser.Other;
using System.Diagnostics.CodeAnalysis;

namespace SymbolRecogniser.NeuralNetwork
{
    class Neuron
    {
        [NotNull] private double[] _weights;
        [NotNull] private double _bias;
        //private double _value;
        private double _output;

        public Neuron(int neuronsInNextLayer)
        {
            _bias = Utils.GetRandomDouble(Parameters.MIN_RANDOM_BIAS, Parameters.MAX_RANDOM_BIAS);
            _weights = new double[neuronsInNextLayer];
            for (int i = 0; i < neuronsInNextLayer; i++)
            {
                _weights[i] = Utils.GetRandomDouble(Parameters.MIN_RANDOM_WEIGHT, Parameters.MAX_RANDOM_WEIGHT);
            }
        }

        public double[] Weights
        {
            get { return _weights; }
            set { _weights = value; }
        }
        public double Bias
        {
            get { return _bias; }
        }
        /*public double Value
        {
            get { return _value; }
            set { _value = value; }
        }*/
        public double Output
        {
            get { return _output; }
            set { _output = value; }
        }
    }
}
