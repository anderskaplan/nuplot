using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace NuPlot
{
    /// <summary>
    /// A custom control used for plotting two-dimensional data.
    /// </summary>
    [ContentPropertyAttribute("Plots")]
    public sealed partial class Plot : UserControl
    {
        #region Dependency properties

        public static readonly DependencyProperty XAxisProperty = DependencyProperty.Register("XAxis", typeof(AxisBase), typeof(Plot));
        public static readonly DependencyProperty YAxisProperty = DependencyProperty.Register("YAxis", typeof(AxisBase), typeof(Plot));

        #endregion

        #region Private data members

        private PlotCollection _plots = new PlotCollection();
        private Viewport _defaultViewport = new Viewport(-0.02, 1.02, -0.02, 1.02); // 2% margin
        private Viewport _viewport;
        private bool _suppressEvents;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public Plot()
        {
            InitializeComponent();
            _plots.CollectionChanged += Plots_CollectionChanged;
        }

        /// <summary>
        /// Logical X axis. The default is a LinearAxis.
        /// </summary>
        public AxisBase XAxis
        {
            get { return (AxisBase)GetValue(XAxisProperty); }
            set { SetValue(XAxisProperty, value); }
        }

        /// <summary>
        /// Logical Y axis. The default is a LinearAxis.
        /// </summary>
        public AxisBase YAxis
        {
            get { return (AxisBase)GetValue(YAxisProperty); }
            set { SetValue(YAxisProperty, value); }
        }

        /// <summary>
        /// Plot(s) to be displayed.
        /// </summary>
        public PlotCollection Plots
        {
            get { return _plots; }
        }

        /// <summary>
        /// Default viewport in normalized coordinates.
        /// </summary>
        public Viewport DefaultViewport
        {
            get { return _defaultViewport; }
            set { _defaultViewport = value; }
        }

        #region Private and protected methods

        private void ResetView()
        {
            FitAxisRangesToData();
            _viewport = _defaultViewport;
            _xAxisView.SetView(XAxis, _viewport.XMin, _viewport.XMax);
            _yAxisView.SetView(YAxis, _viewport.YMin, _viewport.YMax);
            foreach (var plot in _plots)
            {
                plot.SetView(XAxis, YAxis, _viewport);
            }
        }

        /// <summary>
        /// Size the axes to fit the data, if the axes are configured to do that.
        /// Suppresses events during the operation.
        /// The return value indicates whether the range actually changed.
        /// </summary>
        private bool FitAxisRangesToData()
        {
            bool changed = false;

            var wasSuppressed = _suppressEvents;
            try
            {
                _suppressEvents = true;

                if (XAxis.ShouldFitRangeToData || YAxis.ShouldFitRangeToData)
                {
                    Trace.WriteLine(string.Format("Plot '{0}': FitAxisRangesToData.", Name));
                    XAxis.StartFittingRangeToData();
                    YAxis.StartFittingRangeToData();
                    foreach (var plot in _plots)
                    {
                        var points = plot.GetPoints();
                        XAxis.FitRange(points.Select(p => p.X));
                        YAxis.FitRange(points.Select(p => p.Y));
                    }
                    changed |= XAxis.FinishFittingRangeToData();
                    changed |= YAxis.FinishFittingRangeToData();
                }
            }
            finally
            {
                _suppressEvents = wasSuppressed;
            }

            return changed;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            // set defaults for dependency properties. cannot do that until the object is properly constructed.
            // (and since we want separate instances we cannot use the dependency property metadata default either)
            var wasSuppressing = _suppressEvents;
            try
            {
                _suppressEvents = true;
                XAxis = new LinearAxis();
                YAxis = new LinearAxis();
            }
            finally
            {
                _suppressEvents = wasSuppressing;
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == XAxisProperty || e.Property == YAxisProperty)
            {
                if (e.OldValue != null)
                {
                    ((AxisBase)e.OldValue).RangeChanged -= Axis_RangeChanged;
                }
                if (e.NewValue != null)
                {
                    ((AxisBase)e.NewValue).RangeChanged += Axis_RangeChanged;
                }

                if (!_suppressEvents)
                {
                    ResetView();
                }
            }
        }

        private void Axis_RangeChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;

            Trace.WriteLine(string.Format("Plot '{0}': received Axis_RangeChanged event.", Name));
            ResetView();
        }

        void Plot_DataChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;

            Trace.WriteLine(string.Format("Plot '{0}': received Plot_DataChanged event.", Name));
            ResetView();
        }

        void Plot_AppearanceChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;

            ((PlotBase)sender).SetView(XAxis, YAxis, _viewport);
        }

        private void Plots_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e == null) return;

            bool plotsChanged = false;

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                var plots = _grid.Children.OfType<PlotBase>().ToList();
                plotsChanged |= RemovePlots(plots);
            }
            else
            {
                plotsChanged |= AddPlots(e.NewItems);
                plotsChanged |= RemovePlots(e.OldItems);
            }

            if (plotsChanged)
            {
                ResetView();
            }
        }

        private bool AddPlots(IList plots)
        {
            bool plotsChanged = false;
            if (plots != null)
            {
                foreach (PlotBase plot in plots)
                {
                    plot.DataChanged += Plot_DataChanged;
                    plot.AppearanceChanged += Plot_AppearanceChanged;
                    plot.SetValue(Grid.RowProperty, 0);
                    plot.SetValue(Grid.ColumnProperty, 1);
                    _grid.Children.Add(plot);
                    plotsChanged = true;
                }
            }
            return plotsChanged;
        }

        private bool RemovePlots(IList plots)
        {
            bool plotsChanged = false;
            if (plots != null)
            {
                foreach (PlotBase plot in plots)
                {
                    plot.DataChanged -= Plot_DataChanged;
                    plot.AppearanceChanged -= Plot_AppearanceChanged;
                    _grid.Children.Remove(plot);
                    plotsChanged = true;
                }
            }
            return plotsChanged;
        }

        #endregion
    }
}
