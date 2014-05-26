using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Windows;
using NuPlot;
using System.Windows.Media;
using System.Collections;

namespace NuPlot.Tests
{
    [TestFixture, RequiresSTA]
    public class DataBindingAndTransformationTests
    {
        private class TestPlot : PlotBase
        {
            public TestPlot()
                : base(false)
            {
            }

            protected override void UpdateView(AxisBase xAxis, AxisBase yAxis, Viewport viewport, Size canvasSize)
            {
                throw new NotImplementedException();
            }

            public override void DrawMarkerSample(DrawingContext context, Size sizeDiu)
            {
                throw new NotImplementedException();
            }
        }

        private static Point[] _points = new Point[]
        {
            new Point(1.1, 1.2),
            new Point(3.14, 2.71),
            new Point(0, 10) 
        };

        [Test]
        public void ShouldDataBindToAnArrayOfPoints()
        {
            var plot = new TestPlot();
            var axis = new LinearAxis();
            plot.ItemsSource = _points;

            Assert.That(plot.GetNormalizedPoints(axis, axis), Is.EquivalentTo(_points));
        }

        [Test]
        public void ShouldDataBindToAPointCollection()
        {
            var plot = new TestPlot();
            var axis = new LinearAxis();
            plot.ItemsSource = new PointCollection(_points);

            Assert.That(plot.GetNormalizedPoints(axis, axis), Is.EquivalentTo(_points));
        }

        [Test]
        public void ShouldX()
        {
            Assert.Ignore("lots of bindings left to test ...");
        }

        // bind to enumeration of points

        // bind to property

        // error: write-only property

        // error: invalid property/field name

        // error: heterogeneous enumeration

        // error: type/axis mismatch


    }
}
