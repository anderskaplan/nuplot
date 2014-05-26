using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NuPlot;

namespace NuPlot.Tests
{
    [TestFixture]
    public class TimeStepTests
    {
        [Test]
        public void ShouldCreateFromATimeSpan()
        {
            var t = new TimeStep(TimeSpan.FromMinutes(1.1), TimeUnit.Seconds);
            Assert.That(t, Is.EqualTo(new TimeStep(66, TimeUnit.Seconds)), "1.1 minutes converted to seconds: 66 s");

            t = new TimeStep(TimeSpan.FromMinutes(1.1), TimeUnit.Minutes);
            Assert.That(t, Is.EqualTo(new TimeStep(1.1, TimeUnit.Minutes)), "1.1 minutes converted to minutes: 1.1 min");
        }

        [Test]
        public void ShouldOffsetADateTime()
        {
            var b = new DateTime(2001, 2, 11, 14, 23, 8); // 2001-02-11 14:23:08

            Assert.That(b + new TimeStep(1, TimeUnit.Seconds), Is.EqualTo(b + TimeSpan.FromSeconds(1)), "added 1 s");
            Assert.That(b + new TimeStep(2, TimeUnit.Minutes), Is.EqualTo(b + TimeSpan.FromMinutes(2)), "added 2 min");
            Assert.That(b + new TimeStep(2.5, TimeUnit.Hours), Is.EqualTo(b + TimeSpan.FromHours(2.5)), "added 2.5 h");
            Assert.That(b + new TimeStep(9.2, TimeUnit.Days), Is.EqualTo(b + TimeSpan.FromDays(9.2)), "added 9.2 days");
            Assert.That(b + new TimeStep(1, TimeUnit.Months), Is.EqualTo(new DateTime(2001, 2 + 1, 11, 14, 23, 8)), "added 1 month");
            Assert.That(b + new TimeStep(1, TimeUnit.Years), Is.EqualTo(new DateTime(2001 + 1, 2, 11, 14, 23, 8)), "added 1 year");
            Assert.That(b + new TimeStep(1.1, TimeUnit.Months), Is.EqualTo(new DateTime(2001, 2 + 1, 11 + 3, 14, 23, 8)), "added 1.1 months - rounded to 1 month, 3 days");
            Assert.That(b + new TimeStep(0.1, TimeUnit.Years), Is.EqualTo(b + TimeSpan.FromDays(37)), "added 0.1 years - rounded to 37 days");
        }
    }
}
