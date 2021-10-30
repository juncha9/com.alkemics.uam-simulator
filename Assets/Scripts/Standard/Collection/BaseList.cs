using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;


namespace UAM
{

    [HideReferenceObjectPicker]
    [Serializable]
    public class BaseList<T> : ICollection<T>, IEnumerable<T>, IEnumerable,
                               IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>,
                               ICollection, IList  
    {


        [SerializeField]
        private List<T> m_List = null;
      
        protected List<T> list
        {
            set => m_List = value;
            get => m_List;
        }


        public BaseList()
        {
            m_List = new List<T>();
        }
        
        public BaseList(IEnumerable<T> collection)
        {
            m_List = new List<T>(collection);
        }


        public virtual T this[int index]
        {
            get
            {             
                return m_List[index];
            }
            set
            {
                m_List[index] = value;
            }

        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                if (value is T == false) throw new ArgumentException($"{nameof(value)} is not Type({typeof(T)})");
                this[index] = (T)value;
            }

        }

        public virtual void Add(T item)
        {
            m_List.Add(item);
        }

        public int Add(object value)
        {
            if (value is T == false)
            {
                throw new ArgumentException($"{nameof(value)} is not Type({typeof(T)})");
            }
            Add((T)value);
            return IndexOf((T)value);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                Add(item);
            }
        }

        public virtual void Insert(int index, T item)
        {
            m_List.Insert(index, item);
        }

        public void Insert(int index, object value)
        {
            if (value is T == false) throw new ArgumentException($"{nameof(value)} is not Type({typeof(T)})");
            Insert(index, (T)value);
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            int i = index;
            foreach (var item in collection)
            {
                Insert(i, item);
                i++;
            }
        }

        public virtual bool Remove(T item)
        {
            return m_List.Remove(item);
        }

        public void Remove(object value)
        {
            if (value is T == false) return;
            Remove((T)value);

        }

        public virtual void RemoveAt(int index)
        {
            m_List.RemoveAt(index);
        }

        public virtual int RemoveAll(Predicate<T> match)
        {
            return m_List.RemoveAll(match);
        }
        public virtual void RemoveRange(int index, int count)
        {
            m_List.RemoveRange(index, count);
        }

        public virtual void Clear()
        {
            m_List.Clear();
        }

        public virtual bool Swap(int indexA, int indexB)
        {
            T itemA = this[indexA];
            T itemB = this[indexB];
            if (itemA.Equals(itemB)) return false;
            T temp = this[indexA];
            this[indexA] = this[indexB];
            this[indexB] = temp;
            return true;
        }

        public virtual bool Swap(T itemA, T itemB)
        {
            if (itemA.Equals(itemB)) return false;
            int indexA = IndexOf(itemA);
            int indexB = IndexOf(itemB);
            if (indexA < 0 || indexB < 0)
            {
                throw new ArgumentException($"{GetType().Name} does not contain items");
            }
            T temp = this[indexA];
            this[indexA] = this[indexB];
            this[indexB] = temp;
            return true;
        }


        public virtual void Reverse(int index, int count)
        {
            m_List.Reverse(index, count);
        }
        public virtual void Reverse()
        {
            m_List.Reverse();
        }
        public virtual void Sort(Comparison<T> comparison)
        {
            m_List.Sort(comparison);
        }
        public virtual void Sort(int index, int count, IComparer<T> comparer)
        {
            m_List.Sort(index, count, comparer);
        }
        public virtual void Sort()
        {
            m_List.Sort();
        }
        public virtual void Sort(IComparer<T> comparer)
        {
            m_List.Sort(comparer);
        }


        #region *** List

        public bool Contains(T item)
        {
            return m_List.Contains(item);
        }

        public bool Contains(object value)
        {
            if (value is T == false) return false;
            return Contains((T)value);
        }

        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            return m_List.BinarySearch(index, count, item, comparer);
        }

        public int BinarySearch(T item)
        {
            return m_List.BinarySearch(item);
        }

        public int BinarySearch(T item, IComparer<T> comparer)
        {
            return m_List.BinarySearch(item, comparer);
        }

        public bool Exists(Predicate<T> match)
        {
            return m_List.Exists(match);
        }

        public T Find(Predicate<T> match)
        {
            return m_List.Find(match);
        }

        public List<T> FindAll(Predicate<T> match)
        {
            return m_List.FindAll(match);
        }

        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            return m_List.FindIndex(startIndex, count, match);
        }

        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return m_List.FindIndex(startIndex, match);
        }

        public int FindIndex(Predicate<T> match)
        {
            return m_List.FindIndex(match);
        }

        public T FindLast(Predicate<T> match)
        {
            return m_List.FindLast(match);
        }

        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            return m_List.FindLastIndex(startIndex, count, match);
        }

        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return m_List.FindLastIndex(startIndex, match);
        }

        public int FindLastIndex(Predicate<T> match)
        {
            return m_List.FindLastIndex(match);
        }

        public void ForEach(Action<T> action)
        {
            m_List.ForEach(action);
        }

        public List<T> GetRange(int index, int count)
        {
            return m_List.GetRange(index, count);
        }

        public int IndexOf(T item, int index, int count)
        {
            return m_List.IndexOf(item, index, count);
        }

        public int IndexOf(T item, int index)
        {
            return m_List.IndexOf(item, index);
        }

        public T[] ToArray()
        {
            return m_List.ToArray();
        }

        public void TrimExcess()
        {
            m_List.TrimExcess();
        }
        public bool TrueForAll(Predicate<T> match)
        {
            return m_List.TrueForAll(match);
        }


        #endregion *** List

        #region *** Interface

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            m_List.CopyTo(array, arrayIndex);
        }

        public virtual void CopyTo(Array array, int index)
        {
            ((ICollection)m_List).CopyTo(array, index);
        }

        public int Count => m_List.Count;

        public bool IsReadOnly => ((ICollection<T>)m_List).IsReadOnly;

        public bool IsSynchronized => ((ICollection)m_List).IsSynchronized;

        public object SyncRoot => ((ICollection)m_List).SyncRoot;

        public bool IsFixedSize => ((IList)m_List).IsFixedSize;

        public int IndexOf(T item)
        {
            return m_List.IndexOf(item);
        }

        public int IndexOf(object value)
        {
            return ((IList)m_List).IndexOf(value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)m_List).GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)m_List).GetEnumerator();
        }

        #endregion *** Interface

    }
}