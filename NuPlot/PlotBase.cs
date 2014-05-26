using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;

namespace NuPlot
{
    /// <summary>
    /// Base class for plots.
    /// </summary>
    public abstract class PlotBase : PlotCanvas
    {
        #region Dependency properties

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(object), typeof(PlotBase), new UIPropertyMetadata(null));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(PlotBase));

        #endregion

        #region Private data members

        private string _xValuePath = "X";
        private string _yValuePath = "Y";
        private AxisBase _xAxis;
        private AxisBase _yAxis;
        private Viewport _viewport;
        private Size _currentSizeDiu;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="useCustomVisualsHandling">Flag indicating if custom visuals handling should be enabled, overriding the default Canvas behavior.</param>
        protected PlotBase(bool useCustomVisualsHandling)
            : base(useCustomVisualsHandling)
        {
        }

        /// <summary>
        /// Data to be displayed.
        /// A homogeneous array or enumeration of objects.
        /// </summary>
        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Title of the plot for use e.g. in a legend.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Property or field name of the X value on the items bound to the ItemsSource.
        /// The default is "X".
        /// </summary>
        public string XValuePath
        {
            get { return _xValuePath; }
            set
            {
                if (value != _xValuePath)
                {
                    _xValuePath = value;
                    OnDataChanged();
                }
            }
        }

        /// <summary>
        /// Property or field name of the Y value on the items bound to the ItemsSource.
        /// The default is "Y".
        /// </summary>
        public string YValuePath
        {
            get { return _yValuePath; }
            set
            {
                if (value != _yValuePath)
                {
                    _yValuePath = value;
                    OnDataChanged();
                }
            }
        }

        /// <summary>
        /// Event raised when the plot data has changed.
        /// </summary>
        public event EventHandler DataChanged;

        /// <summary>
        /// Event raised when the plot appearance (e.g. marker size) has changed.
        /// </summary>
        public event EventHandler AppearanceChanged;

        /// <summary>
        /// Get the data bound to this plot in world coordinates.
        /// </summary>
        public IEnumerable<WorldPoint> GetPoints()
        {
            return ReflectionUtils.GetPoints(ItemsSource, XValuePath, YValuePath);
        }

        /// <summary>
        /// Get the data bound to this plot in normalized coordinates.
        /// </summary>
        public IEnumerable<Point> GetNormalizedPoints(AxisBase xAxis, AxisBase yAxis)
        {
            if (xAxis == null || yAxis == null) throw new ArgumentNullException();

            foreach (var p in GetPoints())
            {
                yield return new Point(xAxis.WorldToNormalized(p.X), yAxis.WorldToNormalized(p.Y));
            }
        }

        /// <summary>
        /// Configure the view: the logical axes to bind to and the viewport.
        /// </summary>
        public void SetView(AxisBase xAxis, AxisBase yAxis, Viewport viewport)
        {
            _xAxis = xAxis;
            _yAxis = yAxis;
            _viewport = viewport;
            UpdateView(_xAxis, _yAxis, _viewport, _currentSizeDiu);
        }

        /// <summary>
        /// Draw a marker sample for use in a legend.
        /// The sample is to be drawn at (0,0) and is to be sized as specified.
        /// </summary>
        public abstract void DrawMarkerSample(DrawingContext context, Size sizeDiu);

        /// <summary>
        /// Update the view. 
        /// This is an abstract method which will be implemented in different ways for different plots.
        /// </summary>
        /// <param name="xAxis">Logical X axis.</param>
        /// <param name="yAxis">Logical Y axis.</param>
        /// <param name="viewport">Viewport.</param>
        /// <param name="sizeDiu">Size in diu.</param>
        protected abstract void UpdateView(AxisBase xAxis, AxisBase yAxis, Viewport viewport, Size sizeDiu);

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ItemsSourceProperty)
            {
                OnDataChanged();
            }
            else if (e.Property == FrameworkElement.ActualWidthProperty || e.Property == FrameworkElement.ActualHeightProperty)
            {
                OnActualSizeChanged();
            }
        }

        protected virtual void OnActualSizeChanged()
        {
            var size = new Size(ActualWidth, ActualHeight);
            if (size != _currentSizeDiu)
            {
                _currentSizeDiu = size;
                UpdateView(_xAxis, _yAxis, _viewport, _currentSizeDiu);
            }
        }

        /// <summary>
        /// Raise the DataChanged event.
        /// </summary>
        protected virtual void OnDataChanged()
        {
            var handler = DataChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raise the AppearanceChanged event.
        /// </summary>
        protected virtual void OnAppearanceChanged()
        {
            var handler = AppearanceChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
