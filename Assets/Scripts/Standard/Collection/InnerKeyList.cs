

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace UAM
{

    [HideReferenceObjectPicker]
    [Serializable]
    public class InnerKeyList<KEY,T> : ObservableList<T> where T : class, IKeyContainer<KEY>, new()
    {
    
        public T this[KEY key]
        {
            get
            {
                try
                {
                    return (from item in this
                            where Equals(item.key, key)
                            select item).First();
                }
                catch
                {
                    return null;
                }
            }
        }

        public ICollection<KEY> keys
        {
            get
            {
                return this.Select(x => x.key).ToList();
            }
        }

        public InnerKeyList() : base()
        {

        }

        public InnerKeyList(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                if (ContainsKey(item.key) == true)
                {
                    continue;
                }
                this.Add(item);
            }
        }

        public override void Add(T item)
        {
            if(item == null)
            {
                return;
            }
            if (this.ContainsKey(item.key) == true)
            {
                throw new ArgumentException($"Item has specified key[{item.key}] is already exist");
            }
            base.Add(item);
        }

        public override void AddRange(IEnumerable<T> collection)
        {
            foreach(var item in collection)
            {
                if (ContainsKey(item.key) == true)
                {
                    throw new ArgumentException($"Item has specified key[{item.key}] is already exist");
                }
            }
            base.AddRange(collection);
        }

        public override void Insert(int index, T item)
        {
            if (ContainsKey(item.key) == true)
            {
                throw new ArgumentException($"Item has specified key[{item.key}] is already exist");
            }
            base.Insert(index, item);
        }

        public override void InsertRange(int index, IEnumerable<T> collection)
        {
            foreach(var item in collection)
            {
                if(ContainsKey(item.key) == true)
                {
                    throw new ArgumentException($"Item has specified key[{item.key}] is already exist");
                }
            }
            base.InsertRange(index, collection);
        }

        public bool Remove(KEY key)
        {
            var item = this[key];
            if(item != null)
            {
                this.Remove(item);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Move(KEY key, int index)
        {
            var item = this[key];
            if (item == null)
            {
                throw new Exception($"Item has specified key is not exist");
            }
            if (item != null)
            {
                Move(item, index);
            }
        }

        public bool ContainsKey(KEY key)
        {
            return this.Any(x => Equals(x.key, key));
        }

        public void MatchBy(IEnumerable<KEY> matchKeys)
        {
            try
            {
                if (matchKeys == null) throw new ArgumentNullException($"{nameof(matchKeys)} is null");
                var keys = this.Select(x => x.key).ToList();
                var removingKeys = keys.Except(matchKeys).ToList();
                foreach (var removeKey in removingKeys)
                {
                    RemoveAll(x => removeKey.Equals(x.key));
                }
                var addingKeys = matchKeys.Except(keys).Distinct().ToArray();
                foreach (var key in addingKeys)
                {
                    var newContainer = (T)Activator.CreateInstance(typeof(T), key);
                    if (newContainer == null) throw new Exception($"Cannot create instance of type[{typeof(T)}]");
                    Add(newContainer);
                }
            }
            catch(Exception ex)
            {
                Debug.LogError($"Error on match collection by keys[{typeof(KEY).Name}], Ex: {ex}");
            }
        }

    }





}
