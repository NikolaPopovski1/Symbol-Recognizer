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
        public double learningRate = Parameters.LEARNING_RATE;

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

            _layers[0] = new Layer(Parameters.INPUT_LAYER_SIZE, Parameters.FIRST_HIDDEN_LAYER_SIZE, 0, null, null, true);

            _layers[1] = new Layer(Parameters.FIRST_HIDDEN_LAYER_SIZE, outputLayerCount, listOfCharsNDrawings.SymbolCount);
            _layers[2] = new Layer(listOfCharsNDrawings.SymbolCount, 0, Parameters.FIRST_HIDDEN_LAYER_SIZE, null, null, false, true);

            //if there is a second hidden layer, uncomment the next lines and comment the lines above
            //_layers[1] = new Layer(Parameters.FIRST_HIDDEN_LAYER_SIZE, Parameters.SECOND_HIDDEN_LAYER_SIZE);
            //_layers[2] = new Layer(Parameters.SECOND_HIDDEN_LAYER_SIZE, outputLayerCount);
            //_layers[3] = new Layer(listOfCharsNDrawings.SymbolCount, 0, null, false, true);

            _layers[0].SetNextLayer(_layers[1]);
            _layers[1].SetNextLayer(_layers[2]);
            _layers[1].SetPreviousLayer(_layers[0]);
            _layers[2].SetPreviousLayer(_layers[1]);

            //if there is a second hidden layer, uncomment the next lines and comment the lines above
            //_layers[2].SetNextLayer(_layers[3]);

            // for loop kjer uporabin FeedForwardInputLayer in ostale + backpropagation + ...
        }



        public void FeedForwardInputLayer(List<Point> vectors)
        {
            if (vectors.Count != Parameters.NUM_OF_POINTS)
            {
                throw new Exception("Number of vectors does not match number of neurons in layer.");
            }

            for (int i = 0; i < _layers[0].Neurons.Length; i++)
                _layers[0].Neurons[i].Output = 0;

            int v = 0;
            for (int i = 0; i < Parameters.INPUT_LAYER_SIZE; i += 2)
            {
                v = i / 2;
                _layers[0].Neurons[i].Output = vectors[v].X;
                _layers[0].Neurons[i + 1].Output = vectors[v].Y;
            }
            _layers[0].FeedForward();
        }
        private bool ChangeOutputLayerCount(int newOutputLayerCount)
        {
            if (newOutputLayerCount < Parameters.MIN_OUTPUT_LAYER_COUNT || newOutputLayerCount > Parameters.MAX_OUTPUT_LAYER_COUNT)
            {
                return false;
            }
            _outputLayerCount = newOutputLayerCount;
            
            _layers[1] = new Layer(Parameters.FIRST_HIDDEN_LAYER_SIZE, _outputLayerCount, _listOfCharsNDrawings.SymbolCount);
            _layers[2] = new Layer(_listOfCharsNDrawings.SymbolCount, 0, Parameters.FIRST_HIDDEN_LAYER_SIZE, null, null, false, true);

            //if there is a second hidden layer, uncomment the next lines and comment the lines above
            //_layers[2] = new Layer(Parameters.SECOND_HIDDEN_LAYER_SIZE, _outputLayerCount);
            //_layers[3] = new Layer(_outputLayerCount, 0, null, false, true);

            _layers[0].SetNextLayer(_layers[1]);
            _layers[1].SetNextLayer(_layers[2]);
            _layers[1].SetPreviousLayer(_layers[0]);
            _layers[2].SetPreviousLayer(_layers[1]);
            //if there is a second hidden layer, uncomment the next lines and comment the lines above
            //_layers[2].SetNextLayer(_layers[3]);

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
                        j = Utils.RandomNumber(0, _listOfCharsNDrawings.SymbolCount);
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
                    expectedValues = new double[_outputLayerCount];
                    expectedValues[j] = 1;

                    // get raw outputs
                    double[] rawOutputs = new double[_outputLayerCount];
                    for (int n = 0; n < _outputLayerCount; n++)
                    {
                        rawOutputs[n] = _layers[^1].Neurons[n].Output;
                    }

                    // use softmax in error calculation
                    averageError += Utils.CategoricalCrossEntropy(expectedValues, Utils.Softmax(rawOutputs));

                    // backpropagate
                    for (int k = _layers.Length - 1; k >= 0; k--)
                    {
                        _layers[k].BackPropagate(expectedValues);
                    }
                }
                var err = averageError / _listOfCharsNDrawings.DrawingsCount;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _logTextBlock.Text += $"Epoch {i + 1} - Average error: {err}\n";
                    _logScrollViewer.ScrollToEnd();
                });
                //if (err < 0.35) break;
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

            double[] rawOutputValues = new double[_outputLayerCount];
            for (int i = 0; i < _outputLayerCount; i++)
            {
                rawOutputValues[i] = _layers[_layers.Length - 1].Neurons[i].Output;
            }
            double[] softmaxOutput = Utils.Softmax(rawOutputValues);

            return Utils.GetCharFromArray(softmaxOutput, chars);
        }
    }
}
