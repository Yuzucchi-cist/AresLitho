using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace AresLitho.Commons.ExtendObservableCollection
{
    public class ExtendObservableCollection<T> : ObservableCollection<T>
    {
        public void AddRange(IEnumerable<T> collection)
        {
            ((List<T>)Items).AddRange(collection);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
