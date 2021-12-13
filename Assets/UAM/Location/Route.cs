using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Alkemic.UAM
{
    [Serializable]
    public class Route : BaseComponent
    {
        [PropertyGroup]
        [ShowOnly]
        private string key;
        public string Key
        {
            set => key = value;
            get => key;
        }

        [CacheGroup]
        [Debug]
        private LocationControl parentLocationControl;
        public LocationControl ParentLocationControl => parentLocationControl;

        [InstanceGroup]
        [RuntimeOnly]
        private List<Way> ways = new List<Way>();
        public List<Way> Ways => ways;

        protected override void OnPreAwake()
        {
            base.OnPreAwake();

            this.CacheComponentInParent(ref parentLocationControl);
        }

        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(parentLocationControl != null, $"[{name}] {nameof(parentLocationControl)} is null", gameObject);
        }

        public void Setup(RouteData data)
        {
            if(data == null)
            {
                Debug.LogError($"[{name}] Failed to setting route, Data is null", gameObject);
                return;
            }

            this.key = data.Key;
            foreach(var key in data.Ways)
            {
                var way = parentLocationControl.Ways.Find(x => x.Key == key);
                if (way != null)
                {
                    ways.Add(way);
                }
                else
                {
                    Debug.LogError($"[{name}] Way[{key}] instance is not exist", gameObject);
                }
            }
            /*
            var firstWay = ways.FirstOrDefault();
            if (firstWay != null)
            {
                startLocation = firstWay.LocationA;
            }
            var lastWay = ways.LastOrDefault();
            if (lastWay != null)
            {
                endLocation = lastWay.LocationB;
            }
            */
        }
    }
}