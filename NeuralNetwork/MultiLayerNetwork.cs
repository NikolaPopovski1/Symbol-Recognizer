using System.Drawing;

namespace SymbolRecogniser.NeuralNetwork
{
    class MultiLayerNetwork
    {
        private int _outputLayerCount;
        private Layer[] _layers;

        public MultiLayerNetwork(int outputLayerCount, CharsNCorrespondingDrawings listOfCharsNDrawings)
        {
            /*
             * neuronsInLayers[0] = input layer
             * neuronsInLayers[1] = hidden layer
             * neuronsInLayers[2] = hidden layer
             * neuronsInLayers[...] = hidden layer
             * neuronsInLayers[n] = output layer
             */

            _outputLayerCount = outputLayerCount;
            _layers = new Layer[Parameters.NUM_OF_LAYERS];

            _layers[0] = new Layer(Parameters.INPUT_LAYER_SIZE, Parameters.FIRST_HIDDEN_LAYER_SIZE, null, true);
            _layers[1] = new Layer(Parameters.FIRST_HIDDEN_LAYER_SIZE, Parameters.SECOND_HIDDEN_LAYER_SIZE);
            _layers[2] = new Layer(Parameters.SECOND_HIDDEN_LAYER_SIZE, outputLayerCount);
            _layers[3] = new Layer(listOfCharsNDrawings.SymbolCount, 0, null, false, true);

            _layers[0].SetNextLayer(_layers[1]);
            _layers[1].SetNextLayer(_layers[2]);
            _layers[2].SetNextLayer(_layers[3]);

            // for loop kjer uporabin FeedForwardInputLayer in ostale + backpropagation + ...
        }



        public void FeedForwardInputLayer(List<Point> vectors)
        {
            if (vectors.Count != Parameters.NUM_OF_POINTS)
            {
                throw new Exception("Number of vectors does not match number of neurons in layer.");
            }
            for (int i = 0; i < Parameters.INPUT_LAYER_SIZE; i += 2)
            {
                _layers[0].Neurons[i].Output = vectors[i].X;
                _layers[0].Neurons[i + 1].Output = vectors[i].Y;
            }
        }
    }
}
