
using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;

namespace UAM
{
    [Serializable]
    [DictionaryDrawerSettings(IsReadOnly = true,DisplayMode = DictionaryDisplayOptions.OneLine)]
    public class EnumDictionary<ENUM, VALUE> : IDictionary<string,VALUE> where ENUM : struct, Enum
    {
        [NonSerialized, OdinSerialize]
        private System.Collections.Generic.Dictionary<string, VALUE> m_Dict = new System.Collections.Generic.Dictionary<string, VALUE>();

        public VALUE this[ENUM enumValue]
        {
            set
            {
                m_Dict[enumValue.ToString()] = value;
            }
            get
            {
                if (m_Dict.ContainsKey(enumValue.ToString()))
                {
                    return m_Dict[enumValue.ToString()];
                }
                else
                {
                    return default(VALUE);
                }
            }
        }
        [Button]
        public void UpdateKeys(ICollection<object> garbageCollection = null)
        {
            var names = Enum.GetNames(typeof(ENUM));
            var removingKeys = m_Dict.Keys.Except(names).ToArray();
            foreach(var key in removingKeys)
            {
                Debug.Log("EnumDict removing: " + key);
                var temp = m_Dict[key];
                m_Dict.Remove(key);
                if (garbageCollection != null)
                {
                    garbageCollection.Add(temp);
                }
            }

            var addingKeys = names.Except(m_Dict.Keys).ToArray();
            foreach(var key in addingKeys)
            {
                Debug.Log("EnumDict adding " + key);
                m_Dict.Add(key, default(VALUE));
                
            }
        }

        #region Interface
       
        public int Count => ((ICollection<KeyValuePair<string, VALUE>>)m_Dict).Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<string, VALUE>>)m_Dict).IsReadOnly;

        public ICollection<string> Keys => ((IDictionary<string, VALUE>)m_Dict).Keys;

        public ICollection<VALUE> Values => ((IDictionary<string, VALUE>)m_Dict).Values;

        public VALUE this[string key]
        {
            get => ((IDictionary<string, VALUE>)m_Dict)[key];
            set => ((IDictionary<string, VALUE>)m_Dict)[key] = value;
        }

        public void Add(string key, VALUE value)
        {
            ((IDictionary<string, VALUE>)m_Dict).Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return ((IDictionary<string, VALUE>)m_Dict).ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return ((IDictionary<string, VALUE>)m_Dict).Remove(key);
        }

        public bool TryGetValue(string key, out VALUE value)
        {
            return ((IDictionary<string, VALUE>)m_Dict).TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<string, VALUE> item)
        {
            ((IDictionary<string, VALUE>)m_Dict).Add(item);
        }

        public void Clear()
        {
            ((IDictionary<string, VALUE>)m_Dict).Clear();
        }

        public bool Contains(KeyValuePair<string, VALUE> item)
        {
            return ((IDictionary<string, VALUE>)m_Dict).Contains(item);
        }

        public void CopyTo(KeyValuePair<string, VALUE>[] array, int arrayIndex)
        {
            ((IDictionary<string, VALUE>)m_Dict).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, VALUE> item)
        {
            return ((IDictionary<string, VALUE>)m_Dict).Remove(item);
        }

        public IEnumerator<KeyValuePair<string, VALUE>> GetEnumerator()
        {
            return ((IDictionary<string, VALUE>)m_Dict).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<string, VALUE>)m_Dict).GetEnumerator();
        }

        #endregion
    }

}

