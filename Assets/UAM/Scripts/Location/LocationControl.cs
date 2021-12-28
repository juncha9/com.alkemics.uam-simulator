using Alkemic.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Alkemic.UAM
{
    public class LocationControl : LeadComponent
    {

        [InstanceGroup]
        [SerializeField]
        private DestroyableList<Location> locations = new DestroyableList<Location>();
        public DestroyableList<Location> Locations => locations;

        public IList<VertiPort> vertiPorts
        {
            get
            {
                return (from loc in locations
                        where loc is VertiPort
                        select loc as VertiPort).ToList();
            }
        }

        [InstanceGroup]
        [SerializeField]
        private DestroyableList<Way> ways = new DestroyableList<Way>();
        public DestroyableList<Way> Ways => ways;

        public List<string> GetLocationList()
        {
            return Locations.Select(x => x.Key).Distinct().ToList();
        }

        public List<Way> GetWayList()
        {
            return this.GetComponentsInChildren<Way>().ToList();
        }

        public List<string> GetWayKeys()
        {
            return Ways.Select(x => x.Key).Distinct().ToList();
        }

        public bool CheckIsDuplicated(string key)
        {
            if (string.IsNullOrEmpty(key) == true)
            {
                return false;
            }
            return Locations.Where(x => x.Key == key).Count() > 1;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            Reload();
        }
        protected override void Awake()
        {
            base.Awake();
            //Reload();
        }

        public Way FindWay(string key)
        {
            return ways.Find(x => x.Key == key);
        }

        public void Reload()
        {
            SyncLocations();
            SyncWays();
        }

        private void SyncLocations()
        {
            var childLocations = GetComponentsInChildren<Location>();

            foreach (var loc in childLocations)
            {
                if (this.Locations.Contains(loc) == false)
                {
                    this.Locations.Add(loc);
                }
            }

            foreach (var loc in this.Locations.ToArray())
            {
                if (childLocations.Contains(x => Equals(x, loc)) == false)
                {
                    this.Locations.Remove(loc);
                }
            }
        }

        private void SyncWays()
        {
            var childWays = GetComponentsInChildren<Way>();

            foreach (var way in childWays)
            {
                if (this.Ways.Contains(way) == false)
                {
                    this.ways.Add(way);
                }
            }

            foreach (var way in this.Ways.ToArray())
            {
                if (childWays.Contains(x => Equals(x, way)) == false)
                {
                    this.Ways.Remove(way);
                }
            }
        }






    }


}