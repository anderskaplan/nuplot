using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace NuPlot
{
    /// <summary>
    /// A dynamic collection of PlotBase objects that provides notifications when items get added, removed, or when the whole list is refreshed.
    /// </summary>
    public class PlotCollection : ObservableCollection<PlotBase>
    {
        private bool _suppressCollectionChangedEvents;

        /// <summary>
        /// Add a range of items.
        /// </summary>
        public void AddRange(IEnumerable<PlotBase> items)
        {
            var startingIndex = base.Count;

            var wasSuppressed = _suppressCollectionChangedEvents;
            try
            {
                _suppressCollectionChangedEvents = true;
                foreach (var item in items)
                {
                    base.Add(item);
                }
            }
            finally
            {
                _suppressCollectionChangedEvents = wasSuppressed;
            }

            base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items.ToList(), startingIndex));
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressCollectionChangedEvents)
            {
                base.OnCollectionChanged(e);
            }
        }
    }
}
