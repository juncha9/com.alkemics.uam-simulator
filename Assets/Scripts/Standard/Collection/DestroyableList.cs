using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UAM
{
    [HideReferenceObjectPicker]
    [Serializable]
    public class DestroyableList <T> : ObservableList <T> where T : IDestroyable
    {
        public DestroyableList() : base()
        {
            SetupEvents();
        }

        public DestroyableList(int capacity) : base(capacity)
        {
            SetupEvents();
        }

        public DestroyableList(IEnumerable<T> collection) : base(collection)
        {
            SetupEvents();
        }

        private void SetupEvents()
        {
            this.CollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach(var item in e.NewItems.Cast<T>())
                {
                    item.onDestroy.AddListener(RemoveItem);
                }

            }
            else if(e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach(var item in e.OldItems.Cast<T>())
                {
                    item.onDestroy.RemoveListener(RemoveItem);
                }
                foreach(var item in e.NewItems.Cast<T>())
                {
                    item.onDestroy.AddListener(RemoveItem);
                }
            }
        }

        private void RemoveItem(IDestroyable obj)
        {
            if(obj is T)
            {
                var target = (T)obj;
                this.Remove(target);
            }
        }
    }
}
