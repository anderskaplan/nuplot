using System.Windows;
using System.Windows.Media;

namespace NuPlot
{
    /// <summary>
    /// A point plot. Also known as scatterplot or x-y-plot.
    /// </summary>
    public class PointPlot : PlotBase
    {
        #region Dependency properties

        public static readonly DependencyProperty MarkerStrokeProperty = DependencyProperty.Register("MarkerStroke", typeof(Brush), typeof(PointPlot), new PropertyMetadata(Brushes.Blue));
        public static readonly DependencyProperty MarkerStrokeThicknessProperty = DependencyProperty.Register("MarkerStrokeThickness", typeof(double), typeof(PointPlot));
        public static readonly DependencyProperty MarkerFillProperty = DependencyProperty.Register("MarkerFill", typeof(Brush), typeof(PointPlot));
        public static readonly DependencyProperty MarkerSizeProperty = DependencyProperty.Register("MarkerSize", typeof(double), typeof(PointPlot), new PropertyMetadata(5.0));

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public PointPlot()
            : base(true)
        {
        }

        /// <summary>
        /// The Brush that specifies how the outline of the point markers is painted.
        /// </summary>
        public Brush MarkerStroke
        {
            get { return (Brush)GetValue(MarkerStrokeProperty); }
            set { SetValue(MarkerStrokeProperty, value); }
        }

        /// <summary>
        /// The width of the point marker outline.
        /// </summary>
        public double MarkerStrokeThickness
        {
            get { return (double)GetValue(MarkerStrokeThicknessProperty); }
            set { SetValue(MarkerStrokeThicknessProperty, value); }
        }

        /// <summary>
        /// The Brush that specifies how the interior of the point markers is painted.
        /// </summary>
        public Brush MarkerFill
        {
            get { return (Brush)GetValue(MarkerFillProperty); }
            set { SetValue(MarkerFillProperty, value); }
        }

        /// <summary>
        /// The size of the markers (diameter or similar) in diu.
        /// </summary>
        public double MarkerSize
        {
            get { return (double)GetValue(MarkerSizeProperty); }
            set { SetValue(MarkerSizeProperty, value); }
        }

        /// <summary>
        /// Standard method override.
        /// </summary>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == MarkerStrokeProperty || 
                e.Property == MarkerStrokeThicknessProperty || 
                e.Property == MarkerFillProperty || 
                e.Property == MarkerSizeProperty)
            {
                OnAppearanceChanged();
            }
        }

        /// <summary>
        /// Draw a sample marker.
        /// </summary>
        public override void DrawMarkerSample(DrawingContext context, Size sizeDiu)
        {
            var pen = (MarkerStroke != null && MarkerStrokeThickness > 0) ? new Pen(MarkerStroke, MarkerStrokeThickness) : null;
            context.DrawEllipse(MarkerFill, pen, new Point(sizeDiu.Width / 2, sizeDiu.Height / 2), MarkerSize / 2, MarkerSize / 2);
        }

        /// <summary>
        /// Draw the plot.
        /// </summary>
        protected override void UpdateView(AxisBase xAxis, AxisBase yAxis, Viewport viewport, Size sizeDiu)
        {
            ClearVisuals();
            DrawMarkers(xAxis, yAxis, viewport, sizeDiu);
        }

        /// <summary>
        /// Draw the markers.
        /// </summary>
        protected virtual void DrawMarkers(AxisBase xAxis, AxisBase yAxis, Viewport viewport, Size sizeDiu)
        {
            if (xAxis != null && yAxis != null && 
                !Viewport.IsNullOrEmpty(viewport) && 
                sizeDiu.Width > 0 && sizeDiu.Height > 0 && 
                MarkerSize > 0)
            {
                var markers = new DrawingVisual();
                using (var context = markers.RenderOpen())
                {
                    context.PushTransform(new ScaleTransform(1, -1, 0, sizeDiu.Height / 2));

                    var pen = (MarkerStroke != null && MarkerStrokeThickness > 0) ? new Pen(MarkerStroke, MarkerStrokeThickness) : null;

                    foreach (var p in GetNormalizedPoints(xAxis, yAxis))
                    {
                        var center = viewport.NormalizedToCanvas(p, sizeDiu);
                        context.DrawEllipse(MarkerFill, pen, center, MarkerSize / 2, MarkerSize / 2);
                    }
                }
                AddVisual(markers);
            }
        }
    }
}
