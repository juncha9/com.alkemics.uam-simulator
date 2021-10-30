using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace UAM
{
    [Serializable]
    public class ObservableHashSet<T> : SortedSet<T>
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }

        public ObservableHashSet() : base()
        {

        }

        public new bool Add(T item)
        {
            bool result = base.Add(item);
            if (result == true)
            { 
                OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
            return result;
        }

        public new void Clear()
        {
            base.Clear();
            OnCollectionChanged(
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new bool Remove(T item)
        {
            bool result = base.Remove(item);
            if(result == true)
            {
                OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            }
            return result;
        }

        public new int RemoveWhere(Predicate<T> match)
        {
            var targets = (this).Where(x => match.Invoke(x)).ToList();
            int result = base.RemoveWhere(match);
            if(result > 0)
            {
                OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, targets));
            }
            return result;
        }

        public void SetBy(IEnumerable<T> tags)
        {
            this.Clear();
            foreach (var tag in tags.Distinct())
            {
                this.Add(tag);
            }
        }

    }
}

