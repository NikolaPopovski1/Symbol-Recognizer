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
        private List<SymbolCharNDrawings> _listOfSymbolCharNDrawings = new List<SymbolCharNDrawings>();

        public List<SymbolCharNDrawings> ListOfSymbolCharNDrawings
        {
            get { return _listOfSymbolCharNDrawings; }
            set { _listOfSymbolCharNDrawings = value; }
        }



        public MainWindow()
        {
            InitializeComponent();
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
                SymbolCharNDrawings symbolCharNDrawings = new SymbolCharNDrawings(TextBoxSymbol.Text[0]);
                System.Collections.Generic.List<Point> oneStroke = new List<Point>();
                foreach (var stroke in Strokes) oneStroke.AddRange(stroke);

                if (!symbolCharNDrawings.AddDrawing(oneStroke))
                {
                    MessageBox.Show("Drawing is too small.");
                    return;
                }
                else
                {
                    ListOfSymbolCharNDrawings.Add(symbolCharNDrawings);
                    MessageBox.Show($"Symbol '{TextBoxSymbol.Text[0]}' saved successfully.");
                }
                MyCanvas.Children.Clear();
                Strokes.Clear();
            }
        }
    }
}
