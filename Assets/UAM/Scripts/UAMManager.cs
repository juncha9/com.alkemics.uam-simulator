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
        private SimulationEntry curSimulationEntry = null;
        public SimulationEntry CurSimulationEntry => curSimulationEntry;

        protected override void Awake()
        {
            base.Awake();

            var defaultSimulationData = UAM.Simulation.SimulationEntries.FirstOrDefault();
            if(defaultSimulationData != null)
            {
                this.curSimulationEntry = defaultSimulationData;
            }
            else
            {
                Debug.LogError($"[{name}:{GetType().Name}] Simulation data is not exist", gameObject);
            }

        }


    }


}

