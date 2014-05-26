using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuPlot
{
    /// <summary>
    /// Utility class for formatting of axes.
    /// </summary>
    public static class AxisUtils
    {
        private const double _minLargeTickStepDiu = 50;
        private static double[] _mantissas = { 1.0, 2.0, 5.0 };

        /// <summary>
        /// Choose a large tick step size.
        /// Algorithm based on the NPlot.LinearAxis.DetermineLargeTickStep method, Copyright (C) 2003-2006 Matt Howlett and others.
        /// </summary>
        /// <param name="visibleRangeWorld">Visible range in world coordinates.</param>
        /// <param name="sizeDiu">Size of the axis, in device-independent units.</param>
        /// <returns>A suitable step size for large ticks.</returns>
        public static double ChooseLargeTickStep(double visibleRangeWorld, double sizeDiu)
        {
            if (visibleRangeWorld <= 0) throw new ArgumentException("The visible range must be positive.", "visibleRangeWorld");
            if (sizeDiu <= 0) throw new ArgumentException("The axis size must be positive.", "sizeDiu");

            double approxTickStepWorld = (_minLargeTickStepDiu / sizeDiu) * visibleRangeWorld;

            // split into exponent and mantissa. approxTickStep = mantissa * 10^exponent
            double exponent = Math.Floor(Math.Log10(approxTickStepWorld));
            double mantissa = Math.Pow(10.0, Math.Log10(approxTickStepWorld) - exponent);

            // determine next whole mantissa below the approximate tick step.
            int mantissaIndex = _mantissas.Length - 1;
            //while (mantissaIndex > 0 && mantissa < _mantissas[mantissaIndex - 1])
            //{
            //    mantissaIndex--;
            //}
            for (int i = 1; i < _mantissas.Length; ++i) // TODO ugly!
            {
                if (mantissa < _mantissas[i])
                {
                    mantissaIndex = i - 1;
                    break;
                }
            }

            // then choose next larger spacing.
            mantissaIndex++;
            if (mantissaIndex == _mantissas.Length)
            {
                mantissaIndex = 0;
                exponent += 1.0;
            }

            // make sure that the returned value is such that at least two large tick marks will be displayed.
            double tickStepWorld = Math.Pow(10.0, exponent) * _mantissas[mantissaIndex];
            double tickStepDiu = (tickStepWorld / visibleRangeWorld) * sizeDiu;
            while (tickStepDiu > sizeDiu / 2)
            {
                mantissaIndex--;
                if (mantissaIndex < 0)
                {
                    mantissaIndex = _mantissas.Length - 1;
                    exponent -= 1.0;
                }

                tickStepWorld = Math.Pow(10.0, exponent) * _mantissas[mantissaIndex];
                tickStepDiu = (tickStepWorld / visibleRangeWorld) * sizeDiu;
            }

            // and we're done.
            return Math.Pow(10.0, exponent) * _mantissas[mantissaIndex];
        }
    }
}
