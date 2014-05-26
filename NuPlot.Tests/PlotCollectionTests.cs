using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NuPlot;
using System.Collections.Specialized;

namespace NuPlot.Tests
{
    [TestFixture, RequiresSTA]
    public class PlotCollectionTests
    {
        [Test]
        public void ShouldAddASingleItem()
        {
            var collection = new PlotCollection();
            collection.Add(new LinePlot());

            int eventCount = 0;
            collection.CollectionChanged += (s, e) =>
            {
                eventCount++;
                Assert.That(e.Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
                Assert.That(e.NewItems.Count, Is.EqualTo(1));
                Assert.That(e.OldItems, Is.Null);
                Assert.That(e.NewStartingIndex, Is.EqualTo(1));
            };

            collection.Add(new LinePlot());
            Assert.That(eventCount, Is.EqualTo(1));
        }

        [Test]
        public void ShouldRemoveASingleItem()
        {
            var collection = new PlotCollection();
            collection.Add(new LinePlot());

            int eventCount = 0;
            collection.CollectionChanged += (s, e) =>
            {
                eventCount++;
                Assert.That(e.Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
                Assert.That(e.NewItems, Is.Null);
                Assert.That(e.OldItems.Count, Is.EqualTo(1));
            };

            collection.RemoveAt(0);
            Assert.That(eventCount, Is.EqualTo(1));
        }

        [Test]
        public void ShouldClearAllItems()
        {
            var collection = new PlotCollection();
            collection.Add(new LinePlot());
            collection.Add(new PointPlot());

            int eventCount = 0;
            collection.CollectionChanged += (s, e) =>
            {
                eventCount++;
                Assert.That(e.Action, Is.EqualTo(NotifyCollectionChangedAction.Reset));
                Assert.That(e.NewItems, Is.Null);
                Assert.That(e.OldItems, Is.Null);
            };

            collection.Clear();
            Assert.That(eventCount, Is.EqualTo(1));
        }

        [Test]
        public void ShouldAddARangeOfItemsAndOnlyRaiseASingleEvent()
        {
            var collection = new PlotCollection();
            int eventCount = 0;
            collection.CollectionChanged += (s, e) =>
            {
                eventCount++;
                Assert.That(e.Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
                Assert.That(e.NewItems.Count, Is.EqualTo(3));
                Assert.That(e.OldItems, Is.Null);
                Assert.That(e.NewStartingIndex, Is.EqualTo(0));
            };

            collection.AddRange(new PlotBase[]{ new LinePlot(), new PointPlot(), new StepPlot() });
            Assert.That(eventCount, Is.EqualTo(1));
        }


    }
}
