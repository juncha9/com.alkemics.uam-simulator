using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace Alkemic.UAM
{
    public class UAMManager : Singleton<UAMManager>
    {
        [PresetComponent]
        [SerializeField]
        private GameObject _LT_VTOLPrefab;
        public GameObject LT_VTOLPrefab => _LT_VTOLPrefab;

        [PresetComponent]
        [SerializeField]
        private GameObject _ST_VTOLPrefab;
        public GameObject ST_VTOLPrefab => _ST_VTOLPrefab;

        [PresetComponent]
        [SerializeField]
        private GameObject wayPointPrefab;
        public GameObject WayPointPrefab => wayPointPrefab;

        [OptionGroup]
        [SerializeField]
        private string routesPath;

        public void Test()
        {
            

        }


    }


}

