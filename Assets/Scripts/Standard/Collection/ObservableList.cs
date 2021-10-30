using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

namespace UAM
{

    [HideReferenceObjectPicker]
    [Serializable]
    public class ObservableList<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>,
                                     IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IList, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        [SerializeField]
        private List<T> m_List;

        public T this[int index]
        {
            get
            {
                return m_List[index];
            }
            set
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException("The specified index is out of range.");
                var oldItem = m_List[index];
                m_List[index] = value;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem, index));
            }
        }

        object IList.this[int index]
        {
            get
            {
                return ((IList)m_List)[index];
            }
            set
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException("The specified index is out of range.");
                var oldItem = ((IList)m_List)[index];
                ((IList)m_List)[index] = value;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem, index));
            }
        }

        public int Count => m_List.Count;

        public bool IsReadOnly => ((IList<T>)m_List).IsReadOnly;

        bool IList.IsFixedSize => ((IList)m_List).IsFixedSize;

        public bool IsSynchronized => ((IList)m_List).IsSynchronized;

        public object SyncRoot => ((IList)m_List).SyncRoot;

        public ObservableList()
        {
            m_List = new List<T>();
        }

        public ObservableList(int capacity)
        {
            m_List = new List<T>(capacity);
        }

        public ObservableList(IEnumerable<T> collection)
        {
            m_List = new List<T>(collection);
        }


        public static implicit operator List<T>(ObservableList<T> collection)
        {
            return collection.m_List;
        }

        public static explicit operator ObservableList<T>(List<T> collection)
        {
            var list = new ObservableList<T>();
            list.m_List = collection;
            return list;
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, args);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(T item)
        {
            return m_List.Contains(item);
        }

        bool IList.Contains(object value)
        {
            return ((IList)m_List).Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return ((IList)m_List).IndexOf(value);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_List.CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int index)
        {
            ((IList)m_List).CopyTo(array, index);
        }

        public void Clear()
        {
            m_List.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        int IList.Add(object value)
        {
            int index = Count;
            var result = ((IList)m_List).Add(value);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, null, index));
            return result;
        }

        public virtual void Add(T item)
        {
            int index = Count;
            m_List.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public virtual void AddRange(IEnumerable<T> collection)
        {
            int index = Count;
            m_List.AddRange(collection);
            var iList = collection.ToList();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, iList, index));
        }

        public virtual void Insert(int index, T item)
        {
            m_List.Insert(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        void IList.Insert(int index, object value)
        {
            ((IList)m_List).Insert(index, value);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
        }

        public virtual void InsertRange(int index, IEnumerable<T> collection)
        {
            m_List.InsertRange(index, collection);
            var iList = collection.ToList();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, iList, index));
        }

        public int IndexOf(T item)
        {
            return m_List.IndexOf(item);
        }

        public virtual bool Remove(T item)
        {
            int index = IndexOf(item);
            var result = m_List.Remove(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            return result;
        }

        void IList.Remove(object value)
        {
            if (value is T == false)
                throw new InvalidCastException($"The item is not {typeof(T).Name}");
            int index = IndexOf((T)value);
            ((IList)m_List).Remove(value);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
        }

        public int RemoveAll(Predicate<T> match)
        {
            int count = 0;
            foreach (var item in m_List.ToArray())
            {
                if (match.Invoke(item) == true)
                {
                    this.Remove(item);
                }
                count++;
            }
            return count;
        }

        public void RemoveRange(int index, int count)
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException("The specified index is out of range.");
            List<T> iList = new List<T>();
            for (int i = 0; i < count; i++)
            {
                int _index = index + i;
                if (_index < Count)
                {
                    iList.Add(m_List[_index]);
                }
            }
            m_List.RemoveRange(index, count);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, iList, index));
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException("The specified index is out of range.");
            var oldItem = m_List[index];
            m_List.RemoveAt(index);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
        }

        public void Move(int oldIndex, int newIndex)
        {
            if (newIndex == oldIndex) return;
            if (this.Count < 2) return;

            if ((oldIndex < 0 || oldIndex >= Count) || (newIndex < 0 || newIndex >= Count))
                throw new IndexOutOfRangeException("The specified index is out of range.");

            var item = m_List[oldIndex];

            if (oldIndex < newIndex)
            {
                //MoveRight
                for (int i = oldIndex; i < newIndex; i++)
                {
                    m_List[i] = m_List[i + 1];
                }
                m_List[newIndex] = item;
            }
            else if (oldIndex > newIndex)
            {
                //MoveLeft
                for (int i = oldIndex; i > newIndex; i--)
                {
                    m_List[i] = m_List[i - 1];
                }

                m_List[newIndex] = item;
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));
        }

        public void Move(T item, int index)
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException("The specified index is out of range.");
            var curIndex = IndexOf(item);
            if(curIndex < 0)
            {
                throw new Exception("Collection does not contain item");
            }

            if (curIndex == index)
            {
                return;
            }
            else if (curIndex < index)
            {
                //MoveRight
                for (int i = curIndex; i < index; i++)
                {
                    m_List[i] = m_List[i + 1];
                }
                m_List[index] = item;
            }
            else if (curIndex > index)
            {
                //MoveLeft
                for (int i = curIndex; i > index; i--)
                {
                    m_List[i] = m_List[i - 1];
                }

                m_List[index] = item;
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, index, curIndex));
        }


        public void Swap(int indexA, int indexB)
        {
            if (indexA == indexB) return;

            if ((indexA < 0 || indexA >= Count) || (indexB < 0 || indexB >= Count))
                throw new IndexOutOfRangeException("The specified index is out of range.");
            var temp = this[indexA];
            this[indexA] = this[indexB];
            this[indexB] = temp;
        }

        public void Swap(T itemA, T itemB)
        {
            if (Equals(itemA, itemB) == true) return;
            int indexA = this.IndexOf(itemA);
            int indexB = this.IndexOf(itemB);
            this.Swap(indexA, indexB);
        }


        /*
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
            => m_List.BinarySearch(index, count, item, comparer);
        public int BinarySearch(T item)
            => m_List.BinarySearch(item);
        public int BinarySearch(T item, IComparer<T> comparer)
            => m_List.BinarySearch(item, comparer);
        */
        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
            => m_List.ConvertAll(converter);
        public void CopyTo(T[] array)
            => m_List.CopyTo(array);
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
            => m_List.CopyTo(index, array, arrayIndex, count);
        public bool Exists(Predicate<T> match)
            => m_List.Exists(match);
        public T Find(Predicate<T> match)
            => m_List.Find(match);
        public List<T> FindAll(Predicate<T> match)
            => m_List.FindAll(match);
        public void ForEach(Action<T> action)
            => m_List.ForEach(action);
        public int FindIndex(int startIndex, int count, Predicate<T> match)
            => m_List.FindIndex(startIndex, count, match);
        public int FindIndex(int startIndex, Predicate<T> match)
            => m_List.FindIndex(startIndex, match);
        public int FindIndex(Predicate<T> match)
            => m_List.FindIndex(match);
        public T FindLast(Predicate<T> match)
            => m_List.FindLast(match);
        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
            => m_List.FindLastIndex(startIndex, count, match);
        public int FindLastIndex(int startIndex, Predicate<T> match)
            => m_List.FindLastIndex(startIndex, match);
        public int FindLastIndex(Predicate<T> match)
            => m_List.FindLastIndex(match);
        public List<T> GetRange(int index, int count)
            => m_List.GetRange(index, count);
        public int IndexOf(T item, int index, int count)
            => m_List.IndexOf(item, index, count);
        public int IndexOf(T item, int index)
            => m_List.IndexOf(item, index);
        public int LastIndexOf(T item)
            => m_List.LastIndexOf(item);
        public int LastIndexOf(T item, int index)
            => m_List.LastIndexOf(item, index);
        public int LastIndexOf(T item, int index, int count)
            => m_List.LastIndexOf(item, index, count);
        public void Reverse(int index, int count)
            => m_List.Reverse(index, count);
        public void Reverse()
            => m_List.Reverse();
        
        /*
        public void Sort(int index, int count, IComparer<T> comparer)
            => m_List.Sort(index, count, comparer);

        */

        /// <summary>
        /// Sort list using bubble sort and Move method
        /// </summary>
        public void Sort()
        {
            for (int i = 0; i < this.Count; i++)
            {
                for (int j = 0; j < this.Count - (i + 1); j++)
                {
                    if (Comparer.Default.Compare(this[i], this[j + 1]) > 0)
                    {
                        this.Move(j, j + 1);
                    }
                }
            }
        }

        /// <summary>
        /// Sort list using bubble sort and Move method
        /// </summary>
        public void Sort(IComparer<T> comparer)
        {
            for (int i = 0; i < this.Count; i++)
            {
                for (int j = 0; j < this.Count - (i + 1); j++)
                {
                    if (comparer.Compare(this[j], this[j + 1]) > 0)
                    {
                        this.Move(j, j + 1);
                    }
                }
            }
        }

        /// <summary>
        /// Sort list using bubble sort and Move method
        /// </summary>
        public void Sort(Comparison<T> comparison)
        {
            for (int i = 0; i < this.Count; i++)
            {
                for (int j = 0; j < this.Count - (i + 1); j++)
                {
                    if (comparison.Invoke(this[j], this[j + 1]) > 0)
                    {
                        this.Move(j, j + 1);
                    }
                }
            }
        }

        public void TrimExcess()
            => m_List.TrimExcess();
        public bool TrueForAll(Predicate<T> match)
            => m_List.TrueForAll(match);

    }


}