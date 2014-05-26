using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace NuPlot
{
    /// <summary>
    /// Custom control displaying a sample data point from a plot. For use in legends.
    /// </summary>
    public class LegendItem : PlotCanvas
    {
        public static readonly DependencyProperty PlotProperty = DependencyProperty.Register("Plot", typeof(PlotBase), typeof(LegendItem));

        private Size _currentSizeDiu;

        /// <summary>
        /// Constructor.
        /// </summary>
        public LegendItem()
            : base(true)
        {
        }

        /// <summary>
        /// Attached plot.
        /// </summary>
        public PlotBase Plot
        {
            get { return (PlotBase)GetValue(PlotProperty); }
            set { SetValue(PlotProperty, value); }
        }

        private void DrawSample()
        {
            ClearVisuals();

            var plot = Plot;
            if (plot != null && _currentSizeDiu.Width > 0 && _currentSizeDiu.Height > 0)
            {
                var visual = new DrawingVisual();
                using (var context = visual.RenderOpen())
                {
                    context.PushTransform(new ScaleTransform(1, -1, 0, _currentSizeDiu.Height / 2));
                    plot.DrawMarkerSample(context, _currentSizeDiu);
                }
                AddVisual(visual);
            }
        }

        protected override void OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == PlotProperty)
            {
                var oldPlot = e.OldValue as PlotBase;
                if (oldPlot != null)
                {
                    oldPlot.AppearanceChanged -= Plot_AppearanceChanged;
                }
                var newPlot = e.NewValue as PlotBase;
                if (newPlot != null)
                {
                    newPlot.AppearanceChanged += Plot_AppearanceChanged;
                }
                DrawSample();
            }
            else if (e.Property == ActualHeightProperty || e.Property == ActualWidthProperty)
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
                DrawSample();
            }
        }

        private void Plot_AppearanceChanged(object sender, EventArgs e)
        {
            DrawSample();
        }
    }
}
