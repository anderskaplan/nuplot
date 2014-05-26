using System.Globalization;

namespace NuPlot
{
    /// <summary>
    /// Struct representing a point in world coordinates.
    /// </summary>
    public struct WorldPoint
    {
        /// <summary>
        /// X coordinate.
        /// </summary>
        public object X;

        /// <summary>
        /// Y coordinate.
        /// </summary>
        public object Y;

        /// <summary>
        /// Constructor.
        /// </summary>
        public WorldPoint(object x, object y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Standard method.
        /// </summary>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}, {1})", X, Y);
        }
    }
}
