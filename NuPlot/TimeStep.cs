using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace NuPlot
{
    /// <summary>
    /// Class representing a time span, specified as quantity and unit.
    /// Immutable.
    /// </summary>
    public class TimeStep
    {
        private const double _daysPerYear = 365;
        private const double _daysPerMonth = _daysPerYear / 12;

        /// <summary>
        /// Quantity.
        /// </summary>
        public double Quantity { get; private set; }

        /// <summary>
        /// Unit.
        /// </summary>
        public TimeUnit Unit { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TimeStep(double quantity, TimeUnit unit)
        {
            Quantity = quantity;
            Unit = unit;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="timeSpan">TimeSpan to convert.</param>
        /// <param name="unit">Units to convert into.</param>
        public TimeStep(TimeSpan timeSpan, TimeUnit unit)
        {
            Unit = unit;
            switch (unit)
            {
                case TimeUnit.Seconds:
                    Quantity = timeSpan.TotalSeconds;
                    break;
                case TimeUnit.Minutes:
                    Quantity = timeSpan.TotalMinutes;
                    break;
                case TimeUnit.Hours:
                    Quantity = timeSpan.TotalHours;
                    break;
                case TimeUnit.Days:
                    Quantity = timeSpan.TotalDays;
                    break;
                case TimeUnit.Months:
                    Quantity = timeSpan.TotalDays / _daysPerMonth;
                    break;
                case TimeUnit.Years:
                    Quantity = timeSpan.TotalDays / _daysPerYear;
                    break;
                default:
                    throw new ApplicationException(); // unexpected enum value
            }
        }

        /// <summary>
        /// Standard method.
        /// </summary>
        public static DateTime operator +(DateTime dateTime, TimeStep delta)
        {
            switch (delta.Unit)
            {
                case TimeUnit.Seconds:
                    return dateTime + TimeSpan.FromSeconds(delta.Quantity);
                case TimeUnit.Minutes:
                    return dateTime + TimeSpan.FromMinutes(delta.Quantity);
                case TimeUnit.Hours:
                    return dateTime + TimeSpan.FromHours(delta.Quantity);
                case TimeUnit.Days:
                    return dateTime + TimeSpan.FromDays(delta.Quantity);
                case TimeUnit.Months:
                    {
                        var fullMonths = (int)Math.Floor(delta.Quantity);
                        if (fullMonths != 0)
                        {
                            dateTime = dateTime.AddMonths(fullMonths);
                        }
                        var fractionMonths = delta.Quantity - fullMonths;
                        var fullDays = (int)Math.Floor(fractionMonths * _daysPerMonth + 0.5);
                        if (fullDays != 0)
                        {
                            dateTime = dateTime.AddDays(fullDays);
                        }
                        return dateTime;
                    }
                case TimeUnit.Years:
                    {
                        var fullYears = (int)Math.Floor(delta.Quantity);
                        if (fullYears != 0)
                        {
                            dateTime = dateTime.AddYears(fullYears);
                        }
                        var fractionYears = delta.Quantity - fullYears;
                        var fullDays = (int)Math.Floor(fractionYears * _daysPerYear + 0.5);
                        if (fullDays != 0)
                        {
                            dateTime = dateTime.AddDays(fullDays);
                        }
                        return dateTime;
                    }
                default:
                    throw new ApplicationException(); // unexpected enum value
            }
        }

        /// <summary>
        /// Divide a TimeSpan by a TimeStep.
        /// </summary>
        public static double operator /(TimeSpan timeSpan, TimeStep divideBy)
        {
            return new TimeStep(timeSpan, divideBy.Unit).Quantity / divideBy.Quantity;
        }

        /// <summary>
        /// Multiply a TimeStep by a real value.
        /// </summary>
        public static TimeStep operator *(double multiplier, TimeStep timeStep)
        {
            return new TimeStep(multiplier * timeStep.Quantity, timeStep.Unit);
        }

        /// <summary>
        /// Standard method.
        /// </summary>
        public static bool operator ==(TimeStep first, TimeStep second)
        {
            if (object.ReferenceEquals(first, second))
            {
                // both are null, or both are same instance
                return true;
            }
            else if (((object)first == null) || ((object)second == null))
            {
                // one is null, but not both
                return false;
            }
            else
            {
                return first.Equals(second);
            }
        }

        /// <summary>
        /// Standard method.
        /// </summary>
        public static bool operator !=(TimeStep first, TimeStep second)
        {
            return !(first == second);
        }

        /// <summary>
        /// Standard method.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as TimeStep);
        }

        /// <summary>
        /// Standard method.
        /// </summary>
        public bool Equals(TimeStep other)
        {
            if (other == null) return false;

            return Quantity == other.Quantity && Unit == other.Unit;
        }

        /// <summary>
        /// Standard method.
        /// </summary>
        public override int GetHashCode()
        {
            return Quantity.GetHashCode() ^ Unit.GetHashCode();
        }

        /// <summary>
        /// Standard method.
        /// </summary>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {1}", Quantity, Unit);
        }
    }

    public enum TimeUnit
    {
        Seconds,
        Minutes,
        Hours,
        Days,
        Months,
        Years
    }
}
