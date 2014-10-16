using System;
using System.Collections;
using System.Diagnostics;

namespace NuPlot
{
    /// <summary>
    /// Logical axis for DateTime objects.
    /// </summary>
    public class DateTimeAxis : AxisBase
    {
        private const int _largeTickCountLimit = 500;

        private DateTime? _worldMin;
        private DateTime? _worldMax;
        private TimeStep _largeTickStep;
        private DateTime? _largeTickValue;
        private DateTime _actualMin = new DateTime(1900, 1, 1);
        private DateTime _actualMax = new DateTime(1900, 1, 2);
        private bool _isStartingRangeFitting;
        private DateTime _oldMin;
        private DateTime _oldMax;

        /// <summary>
        /// World coordinate minimum. 
        /// Use this property to constrain the axis, or set it to null (which is the default) to let the axis fit the range to the data automatically.
        /// </summary>
        public DateTime? WorldMin
        {
            get { return _worldMin; }
            set
            {
                if (_worldMin != value)
                {
                    _worldMin = value;
                    OnRangeChanged();
                }
            }
        }

        /// <summary>
        /// World coordinate maximum. 
        /// Use this property to constrain the axis, or set it to null (which is the default) to let the axis fit the range to the data automatically.
        /// </summary>
        public DateTime? WorldMax
        {
            get { return _worldMax; }
            set
            {
                if (_worldMax != value)
                {
                    _worldMax = value;
                    OnRangeChanged();
                }
            }
        }

        /// <summary>
        /// Distance between large ticks. Will be deduced automatically if null.
        /// </summary>
        public TimeStep LargeTickStep
        {
            get { return _largeTickStep; }
            set
            {
                if (value != null)
                {
                    if (value.Quantity <= 0) throw new ArgumentException("Value must be positive.", "LargeTickStep");
                }

                if (_largeTickStep != value)
                {
                    _largeTickStep = value;
                    OnAppearanceChanged();
                }
            }
        }

        /// <summary>
        /// Reference large tick value; other large ticks will be placed relative to this one. Will be deduced automatically if null.
        /// </summary>
        public DateTime? LargeTickValue
        {
            get { return _largeTickValue; }
            set
            {
                if (_largeTickValue != value)
                {
                    _largeTickValue = value;
                    OnAppearanceChanged();
                }
            }
        }

        #region Base class overrides

        public override object ActualWorldMin
        {
            get { return _actualMin; }
        }

        public override object ActualWorldMax
        {
            get { return _actualMax; }
        }

        public override double WorldToNormalized(object value)
        {
            if (_actualMax != _actualMin)
            {
                var d = Convert(value);
                return (d.Ticks - _actualMin.Ticks) / (double)(_actualMax.Ticks - _actualMin.Ticks);
            }
            else
            {
                return 0;
            }
        }

        public override object NormalizedToWorld(double value)
        {
            long ticks = (long)(_actualMin.Ticks + value * (_actualMax.Ticks - _actualMin.Ticks));
            return new DateTime(ticks);
        }

        public override string FormatValue(object value, string format, IFormatProvider provider)
        {
            var d = Convert(value);
            return d.ToString(format, provider);
        }

        public override bool IsValidType(Type type)
        {
            return type == typeof(DateTime);
        }

        public override bool ShouldFitRangeToData
        {
            get { return (WorldMin == null || WorldMax == null); }
        }

        public override void StartFittingRangeToData()
        {
            _oldMin = _actualMin;
            _oldMax = _actualMax;
            _actualMin = _worldMin ?? new DateTime(1900, 1, 1);
            _actualMax = _worldMax ?? new DateTime(1900, 1, 2);
            _isStartingRangeFitting = true;
        }

        public override void FitRange(IEnumerable data)
        {
            foreach (var value in data)
            {
                var x = Convert(value);
                if (WorldMin == null)
                {
                    if (_isStartingRangeFitting || x < _actualMin)
                    {
                        _actualMin = x;
                    }
                }
                if (WorldMax == null)
                {
                    if (_isStartingRangeFitting || x > _actualMax)
                    {
                        _actualMax = x;
                    }
                }
                _isStartingRangeFitting = false;
            }
        }

        public override bool FinishFittingRangeToData()
        {
            if (_actualMin != _oldMin || _actualMax != _oldMax)
            {
                //Trace.WriteLine(string.Format("Fit-range-to-data operation changed the range to {0} .. {1}.", _actualMin, _actualMax));
                OnRangeChanged();
                return true;
            }
            else
            {
                //Trace.WriteLine("Fit-range-to-data operation caused no change.");
                return false;
            }
        }

        public override IEnumerable PlaceLargeTicks(double sizeDiu)
        {
            if (sizeDiu <= 0) throw new ArgumentException("The axis size must be positive.", "sizeDiu");

            TimeStep actualLargeTickStep = _largeTickStep ?? ChooseLargeTickStep(sizeDiu);
            DateTime actualLargeTickValue = _largeTickValue ?? ChooseLargeTickValue(actualLargeTickStep);

            var visibleMax = (_actualMin < _actualMax) ? _actualMax : _actualMin;

            int count = 0;
            for (DateTime x = actualLargeTickValue; x <= visibleMax && count++ < _largeTickCountLimit; x += actualLargeTickStep)
            {
                yield return x;
            }
        }

        public override string GetLargeTickLabelFormat(double sizeDiu)
        {
            if (sizeDiu <= 0) throw new ArgumentException("The axis size must be positive.", "sizeDiu");

            TimeStep actualLargeTickStep = _largeTickStep ?? ChooseLargeTickStep(sizeDiu);

            switch (actualLargeTickStep.Unit)
            {
                case TimeUnit.Seconds:
                    if (actualLargeTickStep.Quantity >= 1)
                    {
                        return "HH:mm:ss";
                    }
                    else
                    {
                        return "HH:mm:ss.FFFFFFF";
                    }
                case TimeUnit.Minutes:
                case TimeUnit.Hours:
                    return "HH:mm";
                case TimeUnit.Days:
                    return "yyyy-MM-dd";
                case TimeUnit.Months:
                    return "MMM yyyy";
                case TimeUnit.Years:
                    return "yyyy";
                default:
                    throw new ApplicationException(); // unexpected enum value
            }
        }

        protected override void OnRangeChanged()
        {
            if (_worldMin.HasValue)
            {
                _actualMin = _worldMin.Value;
            }
            if (_worldMax.HasValue)
            {
                _actualMax = _worldMax.Value;
            }

            base.OnRangeChanged();
        }

        #endregion

        /// <summary>
        /// Convert a real-world value to a DateTime.
        /// </summary>
        protected virtual DateTime Convert(object value)
        {
            if (value is DateTime) return (DateTime)value;

            throw new ArgumentException(string.Format("DateTimeAxis: Invalid type '{0}'.", value.GetType()), "value");
        }

        #region Private methods

        private TimeStep ChooseLargeTickStep(double sizeDiu)
        {
            TimeSpan visibleRangeWorld = (_actualMax > _actualMin) ? _actualMax - _actualMin : _actualMin - _actualMax;
            if (visibleRangeWorld == TimeSpan.Zero)
            {
                return new TimeStep(1, TimeUnit.Days);
            }

            var unit = ChooseTimeUnit(visibleRangeWorld);
            var quantity = AxisUtils.ChooseLargeTickStep(new TimeStep(visibleRangeWorld, unit).Quantity, sizeDiu);
            if (unit != TimeUnit.Seconds)
            {
                quantity = Math.Max(1, quantity);
            }
            return new TimeStep(quantity, unit);
        }

        private TimeUnit ChooseTimeUnit(TimeSpan delta)
        {
            if (delta <= TimeSpan.Zero) throw new ArgumentException("The time span must be positive.", "delta");

            const int daysPerMonth = 30;
            const int monthsPerYear = 12;

            if (delta < TimeSpan.FromMinutes(10))
            {
                return TimeUnit.Seconds;
            }
            else if (delta < TimeSpan.FromHours(2))
            {
                return TimeUnit.Minutes;
            }
            else if (delta < TimeSpan.FromDays(2))
            {
                return TimeUnit.Hours;
            }
            else if (delta < TimeSpan.FromDays(4 * daysPerMonth))
            {
                return TimeUnit.Days;
            }
            else if (delta < TimeSpan.FromDays(3.5 * monthsPerYear * daysPerMonth))
            {
                return TimeUnit.Months;
            }
            else
            {
                return TimeUnit.Years;
            }
        }

        private DateTime ChooseLargeTickValue(TimeStep largeTickStep)
        {
            Debug.Assert(largeTickStep.Quantity > 0);

            var visibleMin = (_actualMin < _actualMax) ? _actualMin : _actualMax;

            DateTime referencePoint = new DateTime();
            switch (largeTickStep.Unit)
            {
                case TimeUnit.Seconds:
                    referencePoint = new DateTime(visibleMin.Year, visibleMin.Month, visibleMin.Day, visibleMin.Hour, visibleMin.Minute, visibleMin.Second);
                    break;
                case TimeUnit.Minutes:
                    referencePoint = new DateTime(visibleMin.Year, visibleMin.Month, visibleMin.Day, visibleMin.Hour, visibleMin.Minute, 0);
                    break;
                case TimeUnit.Hours:
                    referencePoint = new DateTime(visibleMin.Year, visibleMin.Month, visibleMin.Day, visibleMin.Hour, 0, 0);
                    break;
                case TimeUnit.Days:
                    referencePoint = new DateTime(visibleMin.Year, visibleMin.Month, visibleMin.Day);
                    break;
                case TimeUnit.Months:
                    referencePoint = new DateTime(visibleMin.Year, visibleMin.Month, 1);
                    break;
                case TimeUnit.Years:
                    referencePoint = new DateTime(visibleMin.Year, 1, 1);
                    break;
                default:
                    throw new ApplicationException(); // unexpected enum value
            }
            if (_largeTickValue.HasValue)
            {
                referencePoint = _largeTickValue.Value;
            }

            double n = Math.Ceiling((visibleMin - referencePoint) / largeTickStep);
            return referencePoint + n * largeTickStep;
        }

        #endregion
    }
}
