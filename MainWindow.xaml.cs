using SymbolRecogniser.NeuralNetwork;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SymbolRecogniser
{
    public partial class MainWindow : Window
    {
        private bool _isDrawing = false;
        [NotNull] private Point _lastPoint;
        public List<List<Point>> Strokes { get; private set; } = new List<List<Point>>();
        [NotNull] private List<Point> _currentStroke;
        private CharsNCorrespondingDrawings listOfSymbolCharNDrawings = new CharsNCorrespondingDrawings();

        private MultiLayerNetwork network;
        private CancellationTokenSource _cts;
        private SymbolCharNDrawings _predictionSymbolCharNDrawings;

        public MainWindow()
        {
            InitializeComponent();
            network = new MultiLayerNetwork(listOfSymbolCharNDrawings.SymbolCount, listOfSymbolCharNDrawings, LogTextBlock, LogScrollViewer);
            _predictionSymbolCharNDrawings = new SymbolCharNDrawings(' ');
        }




        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _isDrawing = true;
                _lastPoint = e.GetPosition(MyCanvas);

                _currentStroke = new List<Point>();
                _currentStroke.Add(_lastPoint);
                Strokes.Add(_currentStroke);
            }
        }
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing)
            {
                Point currentPoint = e.GetPosition(MyCanvas);

                var line = new Line
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    X1 = _lastPoint.X,
                    Y1 = _lastPoint.Y,
                    X2 = currentPoint.X,
                    Y2 = currentPoint.Y
                };

                MyCanvas.Children.Add(line);
                _lastPoint = currentPoint;

                _currentStroke.Add(currentPoint);
            }
        }
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = false;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextBoxSymbol.Text.Length > 1)
            {
                MessageBox.Show("Please enter only one character.");
                TextBoxSymbol.Text = TextBoxSymbol.Text.Substring(0, 1);
                TextBoxSymbol.CaretIndex = TextBoxSymbol.Text.Length;
            }
        }

        private void ButtonClearCanvas_Click(object sender, RoutedEventArgs e)
        {
            MyCanvas.Children.Clear();
            Strokes.Clear();
        }

        private void ButtonSaveSymbol_Click(object sender, RoutedEventArgs e)
        {
            if (MyCanvas.Children.Count == 0)
            {
                MessageBox.Show("Please draw something before saving.");
                return;
            }
            else if (string.IsNullOrEmpty(TextBoxSymbol.Text))
            {
                MessageBox.Show("Please enter a symbol.");
                return;
            }
            else
            {
                System.Collections.Generic.List<Point> oneStroke = new List<Point>();
                foreach (var stroke in Strokes) oneStroke.AddRange(stroke);

                if (listOfSymbolCharNDrawings.Add(oneStroke, TextBoxSymbol.Text[0]))
                {
                    MessageBox.Show($"Symbol '{TextBoxSymbol.Text[0]}' saved successfully.");
                    MyCanvas.Children.Clear();
                    Strokes.Clear();
                }
                else
                {
                    MessageBox.Show("Drawing is too small.");
                }
            }
        }

        private async void ButtonStartLearning_Click(object sender, RoutedEventArgs e)
        {
            if (listOfSymbolCharNDrawings.SymbolCount < Parameters.MIN_OUTPUT_LAYER_COUNT)
            {
                MessageBox.Show("Please save at least six different symbols before starting learning.");
                return;
            }
            else if (listOfSymbolCharNDrawings.SymbolCount > Parameters.MAX_OUTPUT_LAYER_COUNT)
            {
                MessageBox.Show($"Please save less than {Parameters.MAX_OUTPUT_LAYER_COUNT + 1} different symbols before starting learning.");
                return;
            }
            else if (_cts != null && !_cts.IsCancellationRequested)
            {
                MessageBox.Show("Learning process is already running.");
                return;
            }
            else
            {
                _cts = new CancellationTokenSource();
                CancellationToken token = _cts.Token;

                LogTextBlock.Text = "Learning process started...\n";
                LogScrollViewer.ScrollToEnd();

                network.PrepareForTraining(listOfSymbolCharNDrawings);
                try
                {
                    await Task.Run(() => network.TrainNetwork(token), token);
                }
                catch (OperationCanceledException)
                {
                    LogTextBlock.Text += "Training cancelled.\n";
                    LogScrollViewer.ScrollToEnd();
                }

                LogTextBlock.Text += "Learning process finished.\n";
                LogScrollViewer.ScrollToEnd();
                return;
            }
        }

        private void ButtonStopLearning_Click(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
        }

        private void ButtonRecogniseSymbol_Click(object sender, RoutedEventArgs e)
        {
            if (MyCanvas.Children.Count == 0)
            {
                MessageBox.Show("Please draw something before recognising.");
                return;
            }
            else
            {
                System.Collections.Generic.List<Point> oneStroke = new List<Point>();
                foreach (var stroke in Strokes) oneStroke.AddRange(stroke);

                if (_predictionSymbolCharNDrawings.AddDrawing(oneStroke))
                {
                    char result = network.RecogniseSymbol(_predictionSymbolCharNDrawings.Drawings[0]);
                    if (result == ' ')
                    {
                        _predictionSymbolCharNDrawings = new SymbolCharNDrawings(' ');
                        return;
                    }

                    MessageBox.Show($"Perdicted symbol is: '{result}'");
                    MyCanvas.Children.Clear();
                    Strokes.Clear();
                    _predictionSymbolCharNDrawings = new SymbolCharNDrawings(' ');
                }
                else
                {
                    MessageBox.Show("Drawing is too small.");
                }
            }
        }
    }
}
