using System.Drawing;

namespace SymbolRecogniser.NeuralNetwork
{
    class MultiLayerNetwork
    {
        private int _layersCount;
        private Layer[] _layers;

        public MultiLayerNetwork(int[] neuronsInLayers)
        {
            /*
             * neuronsInLayers[0] = input layer
             * neuronsInLayers[1] = hidden layer
             * neuronsInLayers[2] = hidden layer
             * neuronsInLayers[...] = hidden layer
             * neuronsInLayers[n] = output layer
             */

            _layersCount = neuronsInLayers.Length;
            _layers = new Layer[_layersCount];
            for (int i = 0; i < _layersCount; i++)
            {
                if (_layersCount - 1 == i)
                {
                    _layers[i] = new Layer(neuronsInLayers[i], 0, _layers[i - 1]);
                }
                else
                {
                    if (i == 0)
                    {
                        _layers[i] = new Layer(neuronsInLayers[i], neuronsInLayers[i + 1], null, true);
                    }
                    else
                    {
                        _layers[i] = new Layer(neuronsInLayers[i], neuronsInLayers[i + 1], _layers[i - 1]);
                    }
                }
            }
        }
    }
}
