using System;
using System.Collections;
using System.Diagnostics;

namespace NuPlot
{
    /// <summary>
    /// Logical axis for real numbers, i.e., any kind of numbers that can be represented by doubles.
    /// </summary>
    public class LinearAxis : AxisBase
    {
        private const int _largeTickCountLimit = 500;

        private double? _worldMin;
        private double? _worldMax;
        private double _actualMin = 0;
        private double _actualMax = 1;
        private bool _isStartingRangeFitting;
        private double? _largeTickStep;
        private double? _largeTickValue;
        private bool _alwaysIncludeZero;
        private double _oldMin;
        private double _oldMax;

        /// <summary>
        /// World coordinate minimum. 
        /// Use this property to constrain the axis, or set it to null (which is the default) to let the axis fit the range to the data automatically.
        /// </summary>
        public double? WorldMin
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
        public double? WorldMax
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
        /// Distance between large ticks in world coordinates. Will be deduced automatically if null.
        /// </summary>
        public double? LargeTickStep
        {
            get { return _largeTickStep; }
            set
            {
                if (value.HasValue)
                {
                    if (value.Value <= 0) throw new ArgumentException("Value must be positive.", "LargeTickStep");
                }

                if (_largeTickStep != value)
                {
                    _largeTickStep = value;
                    OnAppearanceChanged();
                }
            }
        }

        /// <summary>
        /// Reference large tick value in world coordinates; other large ticks will be placed relative to this one. Will be deduced automatically if null.
        /// </summary>
        public double? LargeTickValue
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

        /// <summary>
        /// Should the axis always include zero?
        /// </summary>
        public bool AlwaysIncludeZero
        {
            get { return _alwaysIncludeZero; }
            set
            {
                if (_alwaysIncludeZero != value)
                {
                    _alwaysIncludeZero = value;
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
                var x = Convert(value);
                return (x - _actualMin) / (_actualMax - _actualMin);
            }
            else
            {
                return 0;
            }
        }

        public override object NormalizedToWorld(double value)
        {
            return _actualMin + value * (_actualMax - _actualMin);
        }

        public override string FormatValue(object value, string format, IFormatProvider provider)
        {
            var x = Convert(value);

            // kludge to avoid abnormal formatting close to zero.
            if (Math.Abs(x) < 1e-15)
            {
                x = 0.0;
            }

            return x.ToString(format, provider);
        }

        public override bool IsValidType(Type type)
        {
            return type == typeof(double) || type == typeof(float) || type == typeof(int);
        }

        public override bool ShouldFitRangeToData
        {
            get { return (WorldMin == null || WorldMax == null); }
        }

        public override void StartFittingRangeToData()
        {
            _oldMin = _actualMin;
            _oldMax = _actualMax;
            _actualMin = _worldMin ?? 0;
            _actualMax = _worldMax ?? 1;
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
            if (_alwaysIncludeZero)
            {
                if (_actualMin > 0 && _actualMax > 0)
                {
                    _actualMax = Math.Max(_actualMin, _actualMax);
                    _actualMin = 0;
                }
                else if (_actualMin < 0 && _actualMax < 0)
                {
                    _actualMin = Math.Min(_actualMin, _actualMax);
                    _actualMax = 0;
                }
            }

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

            double actualLargeTickStep = _largeTickStep ?? ChooseLargeTickStep(sizeDiu);
            double actualLargeTickValue = ChooseLargeTickValue(actualLargeTickStep);

            var visibleMax = Math.Max(_actualMin, _actualMax);

            int count = 0;
            for (double x = actualLargeTickValue; x <= visibleMax && count++ < _largeTickCountLimit; x += actualLargeTickStep)
            {
                yield return x;
            }
        }

        public override string GetLargeTickLabelFormat(double sizeDiu)
        {
            if (sizeDiu <= 0) throw new ArgumentException("The axis size must be positive.", "sizeDiu");

            return "g5";
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
        /// Convert a real-world value to a double.
        /// </summary>
        protected virtual double Convert(object value)
        {
            if (value is double) return (double)value;
            if (value is float) return (double)(float)value;
            if (value is int) return (double)(int)value;

            throw new ArgumentException(string.Format("Invalid type '{0}'.", value.GetType()), "value");
        }

        #region Private methods

        /// <summary>
        /// Choose a suitable large tick step (in world coordinates) automatically.
        /// </summary>
        private double ChooseLargeTickStep(double sizeDiu)
        {
            var visibleRangeWorld = Math.Abs(_actualMax - _actualMin);
            if (visibleRangeWorld == 0)
            {
                visibleRangeWorld = 1;
            }

            return AxisUtils.ChooseLargeTickStep(visibleRangeWorld, sizeDiu);
        }

        /// <summary>
        /// Choose a suitable start position for large ticks (in world coordinates) automatically.
        /// </summary>
        private double ChooseLargeTickValue(double largeTickStep)
        {
            Debug.Assert(largeTickStep > 0);

            var visibleMin = Math.Min(_actualMin, _actualMax);
            var referencePoint = _largeTickValue ?? 0;

            double n = Math.Ceiling((visibleMin - referencePoint) / largeTickStep);
            return referencePoint + n * largeTickStep;
        }

        #endregion
    }
}
