using System.Windows;
using System.Windows.Media;

namespace NuPlot
{
    /// <summary>
    /// A plot where the data points are visualized as a stepped line.
    /// </summary>
    public class StepPlot : LinePlot
    {
        /// <summary>
        /// Draw the line.
        /// </summary>
        protected override void DrawLine(AxisBase xAxis, AxisBase yAxis, Viewport viewport, Size sizeDiu)
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

                var previousPoint = figure.StartPoint;

                var segment = new PolyLineSegment();
                while (enumeration.MoveNext())
                {
                    var point = viewport.NormalizedToCanvas(enumeration.Current, sizeDiu);

                    if (point.X != previousPoint.X)
                    {
                        segment.Points.Add(new Point(point.X, previousPoint.Y));
                    }

                    segment.Points.Add(point);

                    previousPoint = point;
                }
                figure.Segments.Add(segment);

                geometry.Figures.Add(figure);
            }

            return geometry;
        }
    }
}
