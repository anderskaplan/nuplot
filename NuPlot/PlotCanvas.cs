using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace NuPlot
{
    /// <summary>
    /// A Canvas with customized handling of visuals.
    /// </summary>
    public class PlotCanvas : Canvas
    {
        private List<DrawingVisual> _visuals;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="useCustomVisualsHandling">Flag indicating if custom visuals handling should be enabled, overriding the default Canvas behavior.</param>
        public PlotCanvas(bool useCustomVisualsHandling)
        {
            if (useCustomVisualsHandling)
            {
                _visuals = new List<DrawingVisual>();
            }
        }

        /// <summary>
        /// Add a visual to the collection.
        /// </summary>
        public void AddVisual(DrawingVisual visual)
        {
            if (_visuals == null) throw new InvalidOperationException("Custom visuals handling is disabled.");
            _visuals.Add(visual);
            base.AddVisualChild(visual);
            base.AddLogicalChild(visual);
        }

        /// <summary>
        /// Clear the collection of visuals.
        /// </summary>
        public void ClearVisuals()
        {
            if (_visuals == null) throw new InvalidOperationException("Custom visuals handling is disabled.");
            foreach (var visual in _visuals)
            {
                base.RemoveVisualChild(visual);
                base.RemoveLogicalChild(visual);
            }
            _visuals.Clear();
        }

        /// <summary>
        /// Get the number of visuals in the collection.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get 
            {
                if (_visuals != null)
                {
                    return _visuals.Count;
                }
                else
                {
                    return base.VisualChildrenCount;
                }
            }
        }

        /// <summary>
        /// Access a visual by index.
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {
            if (_visuals != null)
            {
                return _visuals[index];
            }
            else
            {
                return base.GetVisualChild(index);
            }
        }
    }
}
