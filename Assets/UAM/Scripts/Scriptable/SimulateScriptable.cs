using Alkemic.Collections;
using Alkemic.Scriptables;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Alkemic.UAM
{

    [ShowOdinSerializedPropertiesInInspector]
    public class SimulateScriptable : Scriptable
    {
        
        private string GUI_selectSimulationKey = "";

        [JsonIgnore]
        [ValueDropdown("@GUI_GetSimulationDatas()")]
        [LabelText("Select Simulation")]
        [ShowInInspector]
        public string GUI_SelectSimulationKey
        {
            set
            {
                GUI_selectSimulationKey = value;
                if(simulateDatas.ContainsKey(GUI_selectSimulationKey) == true)
                {
                    GUI_selectData = simulateDatas[GUI_selectSimulationKey];
                }
                else
                {
                    GUI_selectData = null;
                }
            }
            get => GUI_selectSimulationKey;
        }

        [ShowIf("@GUI_selectData != null")]
        [ShowInInspector]
        private SimulationData GUI_selectData = null;
        
        [HideInInspector]
        [OdinSerialize, NonSerialized]
        private InnerKeyList<string, SimulationData> simulateDatas = new InnerKeyList<string, SimulationData>();
        public InnerKeyList<string, SimulationData> SimulateDatas => simulateDatas;

        protected virtual void OnEnable()
        {
            if(SimulateDatas.Count > 0)
            {
                GUI_SelectSimulationKey = "";
            }
            else
            {
                GUI_SelectSimulationKey = SimulateDatas.First().Key;
            }
        }

        public IList<string> GUI_GetSimulationDatas()
        {
            return simulateDatas.Keys.ToList(); 
        }

        [Button]
        public void AddSimulationData(string key)
        {
            if (string.IsNullOrWhiteSpace(key) == true)
            {
                Debug.LogError($"[{GetType().Name}] Key must not be empty");
                return;
            }
            if (simulateDatas.ContainsKey(key) == true)
            {
                Debug.LogError($"[{GetType().Name}] SimulateData[{key}] is already exist");
                return;
            }
            simulateDatas.Add(new SimulationData(key));
        }




    }


}

