using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace NuPlot
{
    /// <summary>
    /// Control responsible for rendering an axis on a plot.
    /// </summary>
    internal class AxisView : PlotCanvas
    {
        private const double _largeTickSizeDiu = 10;
        private const double _spacing = 5;

        private Size _currentSize;
        private AxisPosition _position;
        private AxisBase _logicalAxis;
        private double _min;
        private double _max;
        private Typeface _typeface = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        private double _emSize = 10;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AxisView()
            : base(true)
        {
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            UpdateSize();
        }

        /// <summary>
        /// Position of the axis relative to the plot.
        /// </summary>
        public AxisPosition Position
        {
            get { return _position; }
            set
            {
                if (value != _position)
                {
                    _position = value;
                    UpdateSize();
                    UpdateVisuals();
                }
            }
        }

        /// <summary>
        /// Configure the view: the logical axis to bind to and the viewport in normalized coordinates.
        /// </summary>
        public void SetView(AxisBase logicalAxis, double min, double max)
        {
            if (_logicalAxis != null)
            {
                _logicalAxis.AppearanceChanged -= Axis_AppearanceChanged;
            }
            _logicalAxis = logicalAxis;
            if (_logicalAxis != null)
            {
                _logicalAxis.AppearanceChanged += Axis_AppearanceChanged;
            }
            _min = min;
            _max = max;
            UpdateSize();
            UpdateVisuals();
        }

        private void UpdateSize()
        {
            string text = null;
            if (_logicalAxis != null)
            {
                const double typicalSizeDiu = 500;
                var format = _logicalAxis.GetLargeTickLabelFormat(typicalSizeDiu);
                foreach (var tick in _logicalAxis.PlaceLargeTicks(typicalSizeDiu))
                {
                    var t = _logicalAxis.FormatValue(tick, format, CultureInfo.InvariantCulture);
                    if (text == null || t.Length > text.Length)
                    {
                        text = t;
                    }
                }
            }
            var formattedText = new FormattedText(text ?? "1.00", CultureInfo.InvariantCulture, FlowDirection.LeftToRight, _typeface, _emSize, null);

            switch (_position)
            {
                case AxisPosition.Left:
                    Width = formattedText.Width + _spacing;
                    ClearValue(HeightProperty);
                    break;

                case AxisPosition.Bottom:
                    ClearValue(WidthProperty);
                    Height = formattedText.Height + _spacing;
                    break;
            }
        }

        private void UpdateVisuals()
        {
            ClearVisuals();

            if (_currentSize.Width > 0 && _currentSize.Height > 0 && _logicalAxis != null && _min < _max)
            {
                var tickPen = new Pen(Brushes.Gray, 1);
                var provider = CultureInfo.InvariantCulture;

                double sizeDiu = (Position == AxisPosition.Left) ? _currentSize.Height : _currentSize.Width;

                var visual = new DrawingVisual();
                using (var context = visual.RenderOpen())
                {
                    var largeTicks = _logicalAxis.PlaceLargeTicks(sizeDiu);
                    var labelFormat = _logicalAxis.GetLargeTickLabelFormat(sizeDiu);
                    switch (_position)
                    {
                        case AxisPosition.Left:
                            {
                                var x1 = _currentSize.Width;
                                var x2 = _currentSize.Width + _largeTickSizeDiu;
                                foreach (var tick in largeTicks)
                                {
                                    var y = NormalizedToCanvas(_logicalAxis.WorldToNormalized(tick), _currentSize.Height);
                                    context.DrawLine(tickPen, new Point(x1, y), new Point(x2, y));
                                    var text = new FormattedText(_logicalAxis.FormatValue(tick, labelFormat, provider), provider, FlowDirection.LeftToRight, _typeface, _emSize, Brushes.Black);
                                    context.DrawText(text, new Point(x1 - text.Width - _spacing, y - text.Height / 2));
                                }
                            }
                            break;

                        case AxisPosition.Bottom:
                            {
                                var y1 = 0;
                                var y2 = -_largeTickSizeDiu;
                                foreach (var tick in largeTicks)
                                {
                                    var x = NormalizedToCanvas(_logicalAxis.WorldToNormalized(tick), _currentSize.Width);
                                    context.DrawLine(tickPen, new Point(x, y1), new Point(x, y2));
                                    var text = new FormattedText(_logicalAxis.FormatValue(tick, labelFormat, provider), provider, FlowDirection.LeftToRight, _typeface, _emSize, Brushes.Black);
                                    context.DrawText(text, new Point(x - text.Width / 2, _spacing));
                                }
                            }
                            break;
                    }
                }
                AddVisual(visual);
            }
        }

        private double NormalizedToCanvas(double value, double canvasSize)
        {
            if (_min < _max)
            {
                if (_position == AxisPosition.Bottom)
                {
                    return canvasSize * (value - _min) / (_max - _min);
                }
                else
                {
                    return canvasSize * (1 - (value - _min) / (_max - _min));
                }
            }
            else
            {
                return 0;
            }
        }

        protected override void OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == FrameworkElement.ActualWidthProperty || e.Property == FrameworkElement.ActualHeightProperty)
            {
                OnActualSizeChanged();
            }
        }

        private void OnActualSizeChanged()
        {
            var size = new Size(ActualWidth, ActualHeight);
            if (size != _currentSize)
            {
                //Trace.WriteLine(string.Format("AxisView: size changed to {0}.", size));
                _currentSize = size;
                UpdateVisuals();
            }
        }

        private void Axis_AppearanceChanged(object sender, EventArgs e)
        {
            UpdateVisuals();
        }
    }

    internal enum AxisPosition
    {
        Left,
        Bottom
    }
}
