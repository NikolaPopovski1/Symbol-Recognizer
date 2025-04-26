using System.Collections.Generic;
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
        private Point _lastPoint;
        public List<List<Point>> Strokes { get; private set; } = new List<List<Point>>();
        private List<Point> _currentStroke;

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
    }
}
