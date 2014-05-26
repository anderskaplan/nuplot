using System;
using System.Windows;

namespace NuPlot
{
    /// <summary>
    /// Class describing a viewport, i.e., the visible part of a 2D plane. All coordinates are normalized.
    /// Immutable.
    /// </summary>
    public class Viewport
    {
        public static Viewport Unity
        {
            get { return new Viewport(0, 1, 0, 1); }
        }

        public double XMin { get; private set; }
        public double XMax { get; private set; }
        public double YMin { get; private set; }
        public double YMax { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Viewport(double xMin, double xMax, double yMin, double yMax)
        {
            if (xMin > xMax) throw new ArgumentException("Negative range not allowed.", "xMin");
            if (yMin > yMax) throw new ArgumentException("Negative range not allowed.", "yMin");

            XMin = xMin;
            XMax = xMax;
            YMin = yMin;
            YMax = yMax;
        }

        public static bool IsNullOrEmpty(Viewport viewport)
        {
            return (viewport == null || viewport.XMin >= viewport.XMax || viewport.YMin >= viewport.YMax);
        }

        public Point NormalizedToCanvas(Point point, Size canvasSize)
        {
            if (XMin < XMax && YMin < YMax)
            {
                return new Point(canvasSize.Width * (point.X - XMin) / (XMax - XMin), canvasSize.Height * (point.Y - YMin) / (YMax - YMin));
            }
            else
            {
                return new Point();
            }
        }
    }
}
