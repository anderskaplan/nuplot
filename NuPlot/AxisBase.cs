using System;
using System.Collections;

namespace NuPlot
{
    /// <summary>
    /// Base class for a logical axis.
    /// A logical axis provides a mapping between world coordinates (which depend on the type of axis and can be pretty much anything) and normalized coordinates. 
    /// In other words, it maps the range [WorldMin, WorldMax] to [0, 1].
    /// </summary>
    public abstract class AxisBase
    {
        /// <summary>
        /// Event raised when the axis range has changed.
        /// </summary>
        public event EventHandler RangeChanged;

        /// <summary>
        /// Event raised when the axis appearance (e.g. placement of ticks) has changed.
        /// </summary>
        public event EventHandler AppearanceChanged;

        /// <summary>
        /// World coordinate minimum: actual value.
        /// </summary>
        public abstract object ActualWorldMin { get; }

        /// <summary>
        /// World coordinate maximum: actual value.
        /// </summary>
        public abstract object ActualWorldMax { get; }

        /// <summary>
        /// Convert from world to normalized coordinates.
        /// </summary>
        public abstract double WorldToNormalized(object value);

        /// <summary>
        /// Convert from normalized to world coordinates. Note that this will only be an approximation if the world value type is discrete.
        /// </summary>
        public abstract object NormalizedToWorld(double value);

        /// <summary>
        /// Format a value (in world coordinates) for display.
        /// </summary>
        public abstract string FormatValue(object value, string format, IFormatProvider provider);

        /// <summary>
        /// Is it possible to use values of the specified type with this axis?
        /// </summary>
        public abstract bool IsValidType(Type type);

        /// <summary>
        /// Does this axis support automatic fitting of the range to actual data?
        /// </summary>
        public virtual bool ShouldFitRangeToData
        {
            get 
            {
                // default behavior: automatic fitting not supported.
                return false; 
            }
        }

        /// <summary>
        /// Start an operation to fit the range to actual data.
        /// This method call is always followed by zero or more calls to FitRange, and finally FinishFittingRangeToData.
        /// </summary>
        public virtual void StartFittingRangeToData()
        {
            // default behavior: do nothing
        }

        /// <summary>
        /// Fit the range to actual data. This method will be invoked once for each attached plot.
        /// </summary>
        public virtual void FitRange(IEnumerable data)
        {
            // default behavior: do nothing
        }

        /// <summary>
        /// Finish an operation fo fit the range to actual data. See also StartFittingToData.
        /// The AxisRange event will be raised if the axis range changes.
        /// The return value indicates if the range actually changed.
        /// </summary>
        public virtual bool FinishFittingRangeToData()
        {
            // default behavior: do nothing
            return false;
        }

        /// <summary>
        /// Get the placement of large ticks in world coordinates.
        /// </summary>
        public abstract IEnumerable PlaceLargeTicks(double sizeDiu);

        /// <summary>
        /// Get the format string for large tick labels.
        /// </summary>
        public abstract string GetLargeTickLabelFormat(double sizeDiu);

        /// <summary>
        /// Raise the RangeChanged event.
        /// </summary>
        protected virtual void OnRangeChanged()
        {
            var handler = RangeChanged;
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
