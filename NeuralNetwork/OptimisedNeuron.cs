using SymbolRecogniser.Other;

namespace SymbolRecogniser.NeuralNetwork
{
    class OptimisedNeuron
    {
        private List<double> _weights;
        private double _bias;
        private double _value;
        private double _output;

        public OptimisedNeuron(int neuronsInNextLayer)
        {
            _bias = Utils.GetRandomDouble(Parameters.MIN_RANDOM_BIAS, Parameters.MAX_RANDOM_BIAS);_weights = new List<double>();
            for (int i = 0; i < neuronsInNextLayer; i++)
            {
                _weights.Add(Utils.GetRandomDouble(Parameters.MIN_RANDOM_WEIGHT, Parameters.MAX_RANDOM_WEIGHT));
            }
        }

        public List<double> Weights
        {
            get { return _weights; }
        }
        public double Bias
        {
            get { return _bias; }
        }
        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }
        public double Output
        {
            get { return _output; }
            set { _output = value; }
        }
    }
}