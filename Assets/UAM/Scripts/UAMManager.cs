using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Alkemic.UAM
{

    

    public class UAMManager : Singleton<UAMManager>
    {
        [PresetComponent]
        [SerializeField]
        private GameObject _VTOLPrefab;
        public GameObject VTOLPrefab => _VTOLPrefab;

        [PresetComponent]
        [SerializeField]
        private GameObject wayPrefab;
        public GameObject WayPrefab => wayPrefab;


        [PresetComponent]
        [SerializeField]
        private GameObject wayPointPrefab;
        public GameObject WayPointPrefab => wayPointPrefab;

        [PropertyGroup]
        [ShowOnly]
        private SimulationData curSimulationData = null;
        public SimulationData CurSimulationData => curSimulationData;

        protected override void Awake()
        {
            base.Awake();

            var defaultSimulationData = UAM.Simulate.SimulateDatas.FirstOrDefault();
            if(defaultSimulationData != null)
            {
                this.curSimulationData = defaultSimulationData;
            }
            else
            {
                Debug.LogError($"[{name}:{GetType().Name}] Simulation data is not exist", gameObject);
            }

        }


    }


}

