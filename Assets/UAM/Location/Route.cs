using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        [CacheComponent]
        private LocationControl locationControl;
        public LocationControl LocationControl => locationControl;

        [PropertyGroup]
        [ShowOnly]
        private Location source;
        public Location Source => source;

        [PropertyGroup]
        [ShowOnly]
        private Location destination;
        public Location Destination => destination;

        [InstanceGroup]
        [RuntimeOnly]
        private List<Way> ways = new List<Way>();
        public List<Way> Ways => ways;


        protected override void OnPreAwake()
        {
            base.OnPreAwake();

            this.CacheComponentInParent(ref locationControl);
        }

        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(locationControl != null, $"[{name}:{GetType().Name}] {nameof(locationControl)} is null", gameObject);
        }

        public void Setup(RouteData data)
        {
            if (data == null)
            {
                Debug.LogError($"[{name}:{GetType().Name}] Failed to setting route, Data is null", gameObject);
                return;
            }

            this.key = data.Key;

            this.source = locationControl.Locations.Find(x => x.Key == data.Source);

            this.destination = locationControl.Locations.Find(x => x.Key == data.Destination);


            foreach (var key in data.Ways)
            {
                var way = locationControl.Ways.Find(x => x.Key == key);
                if (way != null)
                {
                    ways.Add(way);
                }
                else
                {
                    Debug.LogError($"[{name}:{GetType().Name}] Way[{key}] instance is not exist", gameObject);
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

        public IList<(Way way, Location targetLocation)> ToMoveTaskItems()
        {
            
            try
            {
                List<(Way way, Location targetLocation)> moveTaskItems = new List<(Way way, Location targetLocation)>();
                //경로 
                Location preLocation = null;
                Location nextLocation = null;
                int i = 0;
                foreach (var way in this.Ways)
                {
                    if (i == 0)
                    {
                        if (way.LocationA.Key == Source.Key)
                        {
                            preLocation = way.LocationA;
                            nextLocation = way.LocationB;
                        }
                        else if (way.LocationB.Key == Source.Key)
                        {
                            preLocation = way.LocationB;
                            nextLocation = way.LocationA;
                        }
                        else
                        {
                            throw new System.Exception("Location not match");
                        }
                    }
                    else
                    {
                        if (Equals(way.LocationA, nextLocation) == true)
                        {
                            preLocation = way.LocationA;
                            nextLocation = way.LocationB;
                        }
                        else if (Equals(way.LocationB, nextLocation) == true)
                        {
                            preLocation = way.LocationB;
                            nextLocation = way.LocationA;
                        }
                        else
                        {
                            throw new System.Exception("Location not match");
                        }
                    }

                    var taskDefine = (way, nextLocation);
                    moveTaskItems.Add(taskDefine);
                    i++;
                }
                return moveTaskItems;
            }
            catch(Exception ex)
            {
                Debug.LogError($"[{name}:{GetType().Name}] Faield to convert Route info to TaskDefine", gameObject);
                return null;
            }
        }
    }
}