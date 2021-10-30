using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace UAM
{
    [HideReferenceObjectPicker]
    [Serializable]
    public class NullableDictionary<KEY, VALUE> : ObservableDictionary <KEY, VALUE> where VALUE : class
    {

        public override VALUE this[KEY key] 
        {
            set => base[key] = value;
            get
            {
                if(this.ContainsKey(key) == true)
                {
                    return base[key];
                }
                else
                {
                    return null;
                }
            }
        }

        public NullableDictionary() : base()
        {
            
        }
        public NullableDictionary(int capacity) : base(capacity)
        {
            
        }
        public NullableDictionary(IEqualityComparer<KEY> comparer) : base(comparer)
        {
            
        }
        public NullableDictionary(IDictionary<KEY, VALUE> dictionary) : base(dictionary)
        {
            
        }
        public NullableDictionary(IDictionary<KEY, VALUE> dictionary, IEqualityComparer<KEY> comparer) : base(dictionary,comparer)
        {
            
        }
        public NullableDictionary(int capacity, IEqualityComparer<KEY> comparer) : base(capacity, comparer)
        {
            
        }

        public static implicit operator Dictionary<KEY,VALUE>(NullableDictionary<KEY,VALUE> dictionary)
        {
            Dictionary<KEY, VALUE> dict = new Dictionary<KEY, VALUE>();
            foreach(var key in dictionary.Keys)
            {
                dict[key] = dictionary[key];
            }
            return dict;
        }

    }


}