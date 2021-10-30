using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UAM
{
    [HideReferenceObjectPicker]
    [Serializable]
    public class UniqueList<T> : ObservableList<T>
    {
        public UniqueList() : base()
        {
        }

        public UniqueList(int capacity) : base(capacity)
        {
        }

        public UniqueList(IEnumerable<T> collection) : base()
        {
            var items = collection.Distinct().ToArray();
            foreach(var item in items)
            {
                this.Add(item);
            }
        }

        public static explicit operator UniqueList<T>(List<T> list)
        {
            return new UniqueList<T>(list);
        }


        public override void Add(T item)
        {
            if (Contains(item) == true)
            {
                throw new ArgumentException("The specified item is already exist");
            }

            base.Add(item);
        }

        public override void AddRange(IEnumerable<T> collection)
        {
            foreach(var item in collection)
            {
                if (Contains(item) == true)
                {
                    throw new ArgumentException("The specified item is already exist");
                }
            }
            base.AddRange(collection);
        }

        public override void Insert(int index, T item)
        {
            if (Contains(item) == true)
            {
                throw new ArgumentException("The specified item is already exist");
            }
            base.Insert(index, item);

        }

        public override void InsertRange(int index, IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                if (Contains(item) == true)
                {
                    throw new ArgumentException("The specified item is already exist");
                }
            }
            base.InsertRange(index, collection);
        }



    }

}