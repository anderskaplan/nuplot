using System.Windows;
using System.Windows.Media;

namespace NuPlot
{
    /// <summary>
    /// A point plot where the points are connected by lines.
    /// </summary>
    public class LinePlot : PointPlot
    {
        #region Dependency properties

        public static readonly DependencyProperty LineStrokeProperty = DependencyProperty.Register("LineStroke", typeof(Brush), typeof(LinePlot), new PropertyMetadata(Brushes.Blue));
        public static readonly DependencyProperty LineStrokeThicknessProperty = DependencyProperty.Register("LineStrokeThickness", typeof(double), typeof(LinePlot), new PropertyMetadata(1.0));

        #endregion

        /// <summary>
        /// The Brush that specifies how the line is painted.
        /// </summary>
        public Brush LineStroke
        {
            get { return (Brush)GetValue(LineStrokeProperty); }
            set { SetValue(LineStrokeProperty, value); }
        }

        /// <summary>
        /// The width of the line.
        /// </summary>
        public double LineStrokeThickness
        {
            get { return (double)GetValue(LineStrokeThicknessProperty); }
            set { SetValue(LineStrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Standard method override.
        /// </summary>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == LineStrokeProperty ||
                e.Property == LineStrokeThicknessProperty)
            {
                OnAppearanceChanged();
            }
        }

        /// <summary>
        /// Draw a sample marker.
        /// </summary>
        public override void DrawMarkerSample(DrawingContext context, Size sizeDiu)
        {
            var pen = new Pen(LineStroke, LineStrokeThickness);
            context.DrawLine(pen, new Point(0, sizeDiu.Height / 2), new Point(sizeDiu.Width, sizeDiu.Height / 2));
            base.DrawMarkerSample(context, sizeDiu);
        }

        /// <summary>
        /// Draw the plot.
        /// </summary>
        protected override void UpdateView(AxisBase xAxis, AxisBase yAxis, Viewport viewport, Size sizeDiu)
        {
            ClearVisuals();
            DrawLine(xAxis, yAxis, viewport, sizeDiu);
            DrawMarkers(xAxis, yAxis, viewport, sizeDiu);
        }

        /// <summary>
        /// Draw the line.
        /// </summary>
        protected virtual void DrawLine(AxisBase xAxis, AxisBase yAxis, Viewport viewport, Size sizeDiu)
        {
            if (xAxis != null && yAxis != null && 
                !Viewport.IsNullOrEmpty(viewport) && 
                sizeDiu.Width > 0 && sizeDiu.Height > 0 && 
                LineStroke != null && LineStrokeThickness > 0)
            {
                var line = new DrawingVisual();
                using (var context = line.RenderOpen())
                {
                    context.PushTransform(new ScaleTransform(1, -1, 0, sizeDiu.Height / 2));
                    var pen = new Pen(LineStroke, LineStrokeThickness);
                    context.DrawGeometry(null, pen, GetLineGeometry(xAxis, yAxis, viewport, sizeDiu));
                }
                AddVisual(line);
            }
        }

        private Geometry GetLineGeometry(AxisBase xAxis, AxisBase yAxis, Viewport viewport, Size sizeDiu)
        {
            var geometry = new PathGeometry();

            var enumeration = GetNormalizedPoints(xAxis, yAxis).GetEnumerator();
            if (enumeration.MoveNext())
            {
                var figure = new PathFigure();
                figure.StartPoint = viewport.NormalizedToCanvas(enumeration.Current, sizeDiu);

                var segment = new PolyLineSegment();
                while (enumeration.MoveNext())
                {
                    segment.Points.Add(viewport.NormalizedToCanvas(enumeration.Current, sizeDiu));
                }
                figure.Segments.Add(segment);

                geometry.Figures.Add(figure);
            }

            return geometry;
        }
    }
}
