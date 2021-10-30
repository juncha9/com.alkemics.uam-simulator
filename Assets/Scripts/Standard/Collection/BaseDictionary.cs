using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace UAM
{
    [HideReferenceObjectPicker]
    [Serializable]
    public class BaseDictionary<KEY, VALUE> : ICollection<KeyValuePair<KEY, VALUE>>, IEnumerable<KeyValuePair<KEY, VALUE>>, IEnumerable,
                            IDictionary<KEY, VALUE>, IReadOnlyCollection<KeyValuePair<KEY, VALUE>>,
                            IReadOnlyDictionary<KEY, VALUE>, ICollection, IDictionary
                            //,ISerializationCallbackReceiver, ISupportsPrefabSerialization

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

        public bool CompareByKey(KEY key, IDictionary<KEY, VALUE> target)
        {
            if (this.ContainsKey(key) == false) return false;
            if (target == null) return false;
            if (target.ContainsKey(key) == false) return false;

            VALUE thisValue = this[key];
            VALUE targetValue = target[key];

            if(thisValue != null)
            {
                return thisValue.Equals(targetValue);
            }
            else
            {
                return targetValue.Equals(thisValue);
            }
        }

        private Dictionary<KEY, VALUE> m_Dictionary;

        protected Dictionary<KEY, VALUE> dictionary
        {
            get => m_Dictionary;
        }

        public BaseDictionary()
        {
            m_Dictionary = new Dictionary<KEY, VALUE>();
        }

        public BaseDictionary(IDictionary<KEY, VALUE> dictionary)
        {
            m_Dictionary = new Dictionary<KEY, VALUE>(dictionary);
        }

        public BaseDictionary(IEqualityComparer<KEY> comparer)
        {
            m_Dictionary = new Dictionary<KEY, VALUE>(comparer);
        }

        public BaseDictionary(IDictionary<KEY, VALUE> dictionary, IEqualityComparer<KEY> comparer)
        {
            m_Dictionary = new Dictionary<KEY, VALUE>(dictionary, comparer);
        }

        public virtual VALUE this[KEY key]
        {
            set
            {
                m_Dictionary[key] = value;
            }
            get
            {
                return m_Dictionary[key];
            }
        }

        public object this[object key]
        {
            set
            {
                if (key is KEY == false) return;
                if (value is VALUE == false) return;
                this[(KEY)key] = (VALUE)value;
            }
            get
            {
                if (key is KEY == false) throw new ArgumentException($"{nameof(key)} is not Type({typeof(KEY)})");
                return this[(KEY)key];
            }
        }

        public virtual void Add(KEY key, VALUE value)
        {
            m_Dictionary.Add(key, value);
        }

        public void Add(object key, object value)
        {
            if (key is KEY == false) throw new ArgumentException($"{nameof(key)} is not Type({typeof(KEY)})");
            if (value is VALUE == false) throw new ArgumentException($"{nameof(value)} is not Type({typeof(VALUE)})");
            Add((KEY)key, (VALUE)value);
        }

        public void Add(KeyValuePair<KEY, VALUE> item)
        {
            Add(item.Key, item.Value);
        }

        public virtual bool Remove(KEY key)
        {
            return m_Dictionary.Remove(key);
        }

        public void Remove(object key)
        {
            if (key is KEY == false) return;
            Remove((KEY)key);
        }

        public bool Remove(KeyValuePair<KEY, VALUE> item)
        {
            return Remove(item.Key);
        }

        public virtual void Clear()
        {
            m_Dictionary.Clear();
        }

        #region *** Interface

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
            return m_Dictionary.ContainsKey(key);
        }

        public bool ContainsValue(VALUE value)
        {
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

        /*
        [SerializeField]
        private List<TKey> m_SerializeKeys = new List<TKey>();

        [SerializeField]
        private List<TValue> m_SerializeValues = new List<TValue>();

        public void OnBeforeSerialize()
        {
            m_SerializeKeys.Clear();
            m_SerializeValues.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in m_Dictionary)
            {
                m_SerializeKeys.Add(pair.Key);
                m_SerializeValues.Add(pair.Value);
            }
        } 

        public void OnAfterDeserialize()
        {
            this.Clear();
            if (m_SerializeKeys.Count != m_SerializeValues.Count)
            {
                throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));
            }
            m_Dictionary.Clear();
            for (int i = 0; i < m_SerializeKeys.Count; i++)
            { 
                m_Dictionary.Add(m_SerializeKeys[i], m_SerializeValues[i]); 
            }
        }

        public SerializationData SerializationData 
        {
            set => this.SerializationData = value;
            get => this.SerializationData;
            
        }
        */


        #endregion *** Interface
    }
}