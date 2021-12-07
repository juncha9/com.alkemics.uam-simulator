using UnityEngine;

namespace Alkemic.UAM
{
    public class UAMManager : Singleton<UAMManager>
    {
        [PresetComponent]
        [SerializeField]
        private GameObject wayPointPrefab;
        public GameObject WayPointPrefab => wayPointPrefab;


    }


}

