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
    public class DateTimeAxisTests
    {
        private DateTimeAxis axis;

        [SetUp]
        public void SetUp()
        {
            axis = new DateTimeAxis();
        }

        [Test]
        public void ShouldConsiderDateTimeAValidDataType()
        {
            Assert.That(axis.IsValidType(typeof(DateTime)), "DateTime IsValidType");
            axis.WorldMin = DateTime.Now;
        }

        [Test]
        public void ShouldConsiderStringAValidDataType()
        {
            Assert.Ignore();
        }

        [Test]
        public void ShouldFailOnInvalidTypes()
        {
            Assert.That(axis.IsValidType(typeof(Point)), Is.False);
        }

        [Test]
        public void ShouldMapDatesToNormalizedCoordinates()
        {
            axis.WorldMin = new DateTime(2000, 1, 1);
            axis.WorldMax = new DateTime(2000, 1, 2);
            Assert.That(axis.WorldToNormalized(new DateTime(2000, 1, 1)), Is.EqualTo(0));
            Assert.That(axis.WorldToNormalized(new DateTime(2000, 1, 2)), Is.EqualTo(1));
            Assert.That(axis.WorldToNormalized(new DateTime(2000, 1, 3)), Is.EqualTo(2));
            Assert.That(axis.NormalizedToWorld(0), Is.EqualTo(new DateTime(2000, 1, 1)));
            Assert.That(axis.NormalizedToWorld(1), Is.EqualTo(new DateTime(2000, 1, 2)));
            Assert.That(axis.NormalizedToWorld(2), Is.EqualTo(new DateTime(2000, 1, 3)));
        }

        [Test]
        public void ShouldInterpretStrings()
        {
            Assert.Ignore();
        }

        [Test]
        public void ShouldAutoSizeToFitGivenDataPoints()
        {
            // auto sizing with fixed/non-fixed values
            Assert.Ignore();
        }
    }
}
