using SymbolRecogniser.Other;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SymbolRecogniser.NeuralNetwork
{
    class MultiLayerNetwork
    {
        private int _outputLayerCount;
        private Layer[] _layers;
        private CharsNCorrespondingDrawings _listOfCharsNDrawings;
        private TextBlock _logTextBlock;
        private ScrollViewer _logScrollViewer;

        public MultiLayerNetwork(int outputLayerCount, CharsNCorrespondingDrawings listOfCharsNDrawings, TextBlock LogTextBlock, ScrollViewer LogScrollViewer)
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
            _listOfCharsNDrawings = listOfCharsNDrawings;
            _logTextBlock = LogTextBlock;
            _logScrollViewer = LogScrollViewer;

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
            int v = 0;
            for (int i = 0; i < Parameters.INPUT_LAYER_SIZE; i += 2)
            {
                v = i / 2;
                _layers[0].Neurons[i].Output = vectors[v].X;
                _layers[0].Neurons[i + 1].Output = vectors[v].Y;
            }
        }
        private bool ChangeOutputLayerCount(int newOutputLayerCount)
        {
            if (newOutputLayerCount < Parameters.MIN_OUTPUT_LAYER_COUNT || newOutputLayerCount > Parameters.MAX_OUTPUT_LAYER_COUNT)
            {
                return false;
            }
            _outputLayerCount = newOutputLayerCount;

            _layers[2] = new Layer(Parameters.SECOND_HIDDEN_LAYER_SIZE, _outputLayerCount);
            _layers[3] = new Layer(_outputLayerCount, 0, null, false, true);

            _layers[1].SetNextLayer(_layers[2]);
            _layers[2].SetNextLayer(_layers[3]);

            return true;
        }
        public void PrepareForTraining(CharsNCorrespondingDrawings listOfCharsNDrawings)
        {
            _listOfCharsNDrawings = listOfCharsNDrawings;
            ChangeOutputLayerCount(_listOfCharsNDrawings.SymbolCount);
        }
        public void TrainNetwork(CancellationToken token)
        {
            int[] drawingsPassedTrhough = new int[_listOfCharsNDrawings.SymbolCount];
            bool[] symbolsPassedTrhough = new bool[_listOfCharsNDrawings.SymbolCount];
            int allSymbolDrawingsPassed;

            char[] chars = new char[_listOfCharsNDrawings.SymbolCount];
            double[] expectedValues = new double[_listOfCharsNDrawings.SymbolCount];
            double[] outputValues = new double[_listOfCharsNDrawings.SymbolCount];

            double averageError;

            // training loop
            for (int i = 0; i < Parameters.EPOCHS; i++)
            {
                averageError = 0;
                if (token.IsCancellationRequested)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _logTextBlock.Text += "Training cancelled.\n";
                        _logScrollViewer.ScrollToEnd();
                    });
                    return;
                }

                // init or reset all variables
                for (int j = 0; j < _listOfCharsNDrawings.SymbolCount; j++)
                {
                    drawingsPassedTrhough[j] = 0;
                    symbolsPassedTrhough[j] = false;

                    chars[j] = _listOfCharsNDrawings.List[j].Symbol;
                    expectedValues[j] = 0;
                    outputValues[j] = 0;
                }
                allSymbolDrawingsPassed = 0;

                bool allDrawingsPassed = false;
                // randomly go through the drawings
                while (allSymbolDrawingsPassed < _listOfCharsNDrawings.SymbolCount)
                {
                    // randomly select a symbol and its last drawing that has not been passed through
                    int j;
                    do
                    {
                        j = Utils.RandomNumber(0, _listOfCharsNDrawings.SymbolCount - 1);
                        if (allSymbolDrawingsPassed > _listOfCharsNDrawings.SymbolCount)
                        {
                            allDrawingsPassed = true;
                            break;
                        }
                        if (drawingsPassedTrhough[j] >= _listOfCharsNDrawings.List[j].DrawingsCount)
                        {
                            if (symbolsPassedTrhough[j]) allSymbolDrawingsPassed++;
                            symbolsPassedTrhough[j] = true;
                        }
                    } while (symbolsPassedTrhough[j]);
                    if (allDrawingsPassed) break;

                    // feed forward (between layers)
                    for (int k = 0; k < _layers.Length; k++)
                    {
                        if (k == 0)
                        {
                            FeedForwardInputLayer(_listOfCharsNDrawings.List[j].Drawings[drawingsPassedTrhough[j]++]);
                        }
                        else
                        {
                            _layers[k].FeedForward();
                        }
                    }

                    // calculate error
                    // set expected values and output values
                    expectedValues[j] = 1;
                    foreach (Neuron neuron in _layers[_layers.Length - 1].Neurons) outputValues[j] = neuron.Output;
                    averageError += Utils.CategoricalCrossEntropy(expectedValues, Utils.Softmax(outputValues));
                    expectedValues[j] = 0;
                    foreach (Neuron neuron in _layers[_layers.Length - 1].Neurons) outputValues[j] = 0;
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _logTextBlock.Text += $"Epoch {i + 1} - Average error: {averageError / _listOfCharsNDrawings.DrawingsCount}\n";
                    _logScrollViewer.ScrollToEnd();
                });
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                _logTextBlock.Text += "Finished learning.\n";
                _logScrollViewer.ScrollToEnd();
            });
        }
        public char RecogniseSymbol(List<Point> vectors)
        {
            if (_outputLayerCount < Parameters.MIN_OUTPUT_LAYER_COUNT || _outputLayerCount > Parameters.MAX_OUTPUT_LAYER_COUNT)
            {
                _logTextBlock.Text += "Not enough or too many symbols.\n";
                return ' ';
            }
            if (vectors.Count != Parameters.NUM_OF_POINTS)
            {
                throw new Exception("Number of vectors does not match number of neurons in layer.");
            }

            FeedForwardInputLayer(vectors);
            for (int i = 1; i < _layers.Length; i++)
            {
                _layers[i].FeedForward();
            }
            char[] chars = new char[_listOfCharsNDrawings.SymbolCount];
            for (int j = 0; j < _listOfCharsNDrawings.SymbolCount; j++)
            {
                chars[j] = _listOfCharsNDrawings.List[j].Symbol;
            }
            double[] outputValues = new double[_listOfCharsNDrawings.SymbolCount];
            for (int i = 0; i < _listOfCharsNDrawings.SymbolCount; i++)
            {
                outputValues[i] = _layers[_layers.Length - 1].Neurons[i].Output;
            }
            return Utils.GetCharFromArray(outputValues, chars);
        }
        public void LoadNetwork(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                for (int i = 0; i < _layers.Length; i++)
                {
                    _layers[i].LoadLayer(reader);
                }
            }
        }
        public void SaveNetwork(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                for (int i = 0; i < _layers.Length; i++)
                {
                    _layers[i].SaveLayer(writer);
                }
            }
        }
    }
}
