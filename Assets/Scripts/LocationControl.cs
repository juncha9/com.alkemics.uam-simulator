using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UAM
{
    public class LocationControl : Behavior
    {
        [SerializeField]
        private Transform m_LocationParent;
        public Transform locationParent
        {
            private set => m_LocationParent = value;
            get => m_LocationParent;
        }
        
        [ReadOnly, ShowInInspector]
        private DestroyableList<Location> m_Locations = new DestroyableList<Location>();
        public DestroyableList<Location> locations => m_Locations;

        public List<string> GetLocationList()
        {
            return locations.Select(x => x.Key).Distinct().ToList();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            var childLocations = GetComponentsInChildren<WayPoint>();

            foreach(var loc in childLocations)
            {
                if(this.locations.Contains(loc) == false)
                {
                    this.locations.Add(loc);
                }
            }

            foreach(var loc in this.locations.ToArray())
            {
                if(childLocations.Contains(x => Equals(x, loc)) == false)
                {
                    this.locations.Remove(loc);
                }
            }
        }


    }


}