using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace UAM
{
    [HideReferenceObjectPicker]
    [Serializable]
    public class ObservableDictionary<KEY, VALUE> : ICollection<KeyValuePair<KEY, VALUE>>, IEnumerable<KeyValuePair<KEY, VALUE>>, IEnumerable,
                                                   IDictionary<KEY, VALUE>, IReadOnlyCollection<KeyValuePair<KEY, VALUE>>,
                                                   IReadOnlyDictionary<KEY, VALUE>, ICollection, IDictionary, 
                                                    INotifyCollectionChanged
    {

        public static bool CompareByKey(KEY key, IDictionary<KEY, VALUE> objectA, IDictionary<KEY, VALUE> objectB)
        {
            if (objectA.ContainsKey(key) == false) return false;
            if (objectB.ContainsKey(key) == false) return false;

            VALUE valueA = objectA[key];

            VALUE valueB = objectB[key];

            if (valueA != null)
            {
                return valueA.Equals(valueB);
            }
            else

            {
                return valueB.Equals(valueA);
            }
        }


        public event NotifyCollectionChangedEventHandler CollectionChanged;

        [SerializeField]
        private Dictionary<KEY, VALUE> m_Dictionary;
        public Dictionary<KEY, VALUE> dictionary
        {
            get => m_Dictionary;
        }

        public virtual VALUE this[KEY key]
        {
            set
            {
                int index = IndexOf(key);
                var temp = this[key];
                m_Dictionary[key] = value;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                    new KeyValuePair<KEY, VALUE>(key, value),
                    new KeyValuePair<KEY, VALUE>(key, temp),
                    index));    
            }
            get
            {
                return m_Dictionary[key];
            }
        }

        object IDictionary.this[object key] 
        {
            set
            {
                if (key is KEY == false) throw new ArgumentException($"{nameof(key)} is not {typeof(KEY)}");
                if (value is VALUE == false) throw new ArgumentException($"{nameof(value)} is not {typeof(VALUE)}");
                this[(KEY)key] = (VALUE)value;
            }

            get
            {
                if (key is KEY == false) throw new ArgumentException($"{nameof(key)} is not {typeof(KEY)}");
                return this[(KEY)key];
            }

        }


        #region Interface

        public IEnumerable<KEY> Keys => ((IReadOnlyDictionary<KEY, VALUE>)m_Dictionary).Keys;

        public IEnumerable<VALUE> Values => ((IReadOnlyDictionary<KEY, VALUE>)m_Dictionary).Values;

        ICollection IDictionary.Keys => ((IDictionary)m_Dictionary).Keys;

        ICollection IDictionary.Values => ((IDictionary)m_Dictionary).Values;

        public int Count
        {
            get
            {

                if (m_Dictionary != null)
                {
                    return m_Dictionary.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public ObservableDictionary()
        {
            m_Dictionary = new Dictionary<KEY, VALUE>();
        }
        public ObservableDictionary(int capacity)
        {
            m_Dictionary = new Dictionary<KEY, VALUE>(capacity);
        }
        public ObservableDictionary(IEqualityComparer<KEY> comparer)
        {
            m_Dictionary = new Dictionary<KEY, VALUE>(comparer);
        }
        public ObservableDictionary(IDictionary<KEY, VALUE> dictionary)
        {
            m_Dictionary = new Dictionary<KEY, VALUE>(dictionary);
        }
        public ObservableDictionary(IDictionary<KEY, VALUE> dictionary, IEqualityComparer<KEY> comparer)
        {
            m_Dictionary = new Dictionary<KEY, VALUE>(dictionary, comparer);
        }
        public ObservableDictionary(int capacity, IEqualityComparer<KEY> comparer)
        {
            m_Dictionary = new Dictionary<KEY, VALUE>(capacity, comparer);
        }

        public int IndexOf(KEY key)
        {
            var index = 0;
            foreach (var k in Keys)
            {
                if (Equals(k, key))
                    return index;
                index++;
            }
            return -1;
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }

        public void Add(object key, object value)
        {
            if (key is KEY == false) throw new ArgumentException($"{nameof(key)} is not {typeof(KEY)}");
            if (value is VALUE == false) throw new ArgumentException($"{nameof(value)} is not {typeof(VALUE)}");

            this.Add((KEY)key, (VALUE)value);
        }

        public void Add(KEY key, VALUE value)
        {
            int index = this.Count;
            m_Dictionary.Add(key, value);
            OnCollectionChanged(
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<KEY, VALUE>(key, value), index));
        }

        public void Add(KeyValuePair<KEY, VALUE> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            m_Dictionary.Clear();
            OnCollectionChanged(
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Remove(object key)
        {
            if (key is KEY == false) throw new ArgumentException($"{nameof(key)} is not {typeof(KEY)}");
            this.Remove((KEY)key);
        }

        public bool Remove(KEY key)
        {
            int index = IndexOf(key);
            m_Dictionary.TryGetValue(key, out VALUE value);
            var result = m_Dictionary.Remove(key);
            if(result == true)
            {
                OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<KEY, VALUE>(key, value), index));
            }
            return result;
        }

        public bool Remove(KeyValuePair<KEY, VALUE> item)
        {
            return this.Remove(item.Key);
        }

        public bool IsReadOnly => ((ICollection<KeyValuePair<KEY, VALUE>>)m_Dictionary).IsReadOnly;

        ICollection<KEY> IDictionary<KEY, VALUE>.Keys => ((IDictionary<KEY, VALUE>)m_Dictionary).Keys;

        ICollection<VALUE> IDictionary<KEY, VALUE>.Values => ((IDictionary<KEY, VALUE>)m_Dictionary).Values;

        public bool IsSynchronized => ((ICollection)m_Dictionary).IsSynchronized;

        public object SyncRoot => ((ICollection)m_Dictionary).SyncRoot;

        public bool IsFixedSize => ((IDictionary)m_Dictionary).IsFixedSize;

        public IEqualityComparer<KEY> Comparer => m_Dictionary.Comparer;

        

        public bool TryGetValue(KEY key, out VALUE value)
        {
            return m_Dictionary.TryGetValue(key, out value);
        }

        public bool Contains(object key)
        {
            return ((IDictionary)m_Dictionary).Contains(key);
        }

        public bool Contains(KeyValuePair<KEY, VALUE> item)
        {
            return ((ICollection<KeyValuePair<KEY, VALUE>>)m_Dictionary).Contains(item);
        }

        public bool ContainsKey(KEY key)
        {
            if (key == null) return false;
            return m_Dictionary.ContainsKey(key);
        }

        public bool ContainsValue(VALUE value)
        {
            if (value == null) return false;
            return m_Dictionary.ContainsValue(value);
        }

        public void CopyTo(KeyValuePair<KEY, VALUE>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<KEY, VALUE>>)m_Dictionary).CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)m_Dictionary).CopyTo(array, index);
        }

        public IEnumerator<KeyValuePair<KEY, VALUE>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<KEY, VALUE>>)m_Dictionary).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)m_Dictionary).GetEnumerator();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)m_Dictionary).GetEnumerator();
        }

        #endregion



    }


}