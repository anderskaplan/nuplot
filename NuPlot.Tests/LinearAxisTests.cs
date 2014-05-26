using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NuPlot;
using System.Windows;

namespace NuPlot.Tests
{
    [TestFixture]
    public class LinearAxisTests
    {
        private LinearAxis axis;

        [SetUp]
        public void SetUp()
        {
            axis = new LinearAxis();
        }

        [Test]
        public void ShouldAcceptIntegersAndFloats()
        {
            Assert.That(axis.IsValidType(typeof(double)), "double IsValidType");
            axis.WorldMin = 0.1;

            Assert.That(axis.IsValidType(typeof(float)), "float IsValidType");
            axis.WorldMin = 0.1f;

            Assert.That(axis.IsValidType(typeof(int)), "int IsValidType");
            axis.WorldMin = 1d;
        }

        [Test]
        public void ShouldFailOnInvalidTypes()
        {
            Assert.That(axis.IsValidType(typeof(Point)), Is.False);
        }

        [Test]
        public void ShouldAutoSizeToFitGivenDataPoints()
        {
            axis.StartFittingRangeToData();
            axis.FitRange(new double[] { 3.0, 1.7, 11.9, 9.0 });
            axis.FinishFittingRangeToData();

            Assert.That(axis.ActualWorldMin, Is.EqualTo(1.7));
            Assert.That(axis.ActualWorldMax, Is.EqualTo(11.9));
        }

        [Test]
        public void ShouldAutoSizeToFitGivenDataPointsWithEndsFixed()
        {
            axis.WorldMin = -1;
            axis.WorldMax = 7;
            axis.StartFittingRangeToData();
            axis.FitRange(new double[] { 3.0, 1.7, 11.9, 9.0 });
            axis.FinishFittingRangeToData();

            Assert.That(axis.ActualWorldMin, Is.EqualTo(-1));
            Assert.That(axis.ActualWorldMax, Is.EqualTo(7));
        }

        [Test]
        public void ShouldAlwaysIncludeZeroWhenConfiguredToDoSo()
        {
            axis.AlwaysIncludeZero = true;
            axis.StartFittingRangeToData();
            axis.FitRange(new double[] { 3.0, 1.7, 11.9, 9.0 });
            axis.FinishFittingRangeToData();

            Assert.That(axis.ActualWorldMin, Is.EqualTo(0));
            Assert.That(axis.ActualWorldMax, Is.EqualTo(11.9));

            axis.StartFittingRangeToData();
            axis.FitRange(new double[] { -3.0, -1.7, -11.9, -9.0 });
            axis.FinishFittingRangeToData();

            Assert.That(axis.ActualWorldMin, Is.EqualTo(-11.9));
            Assert.That(axis.ActualWorldMax, Is.EqualTo(0));
        }

        [Test]
        public void ShouldPlaceTicksAutomatically()
        {

            // test world min < max, min = max, min > max

            Assert.Ignore();
        }

    }
}
