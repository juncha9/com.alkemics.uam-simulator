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
    [Serializable]
    public class SimulationScriptable : Scriptable
    {
       
        [TitleGroup("Simulation")]
        [HorizontalGroup("Simulation/Control")]
        [HideLabel]
        [ValueDropdown("@GetSimulationKeys()")]
        [ShowInInspector]
        private string GUI_selectSimulationKey = "";


        [LabelText("Selected")]
        [ShowIf("@GUI_selectEntry != null")]
        [ShowInInspector]
        private SimulationEntry GUI_selectEntry
        {
            set { }
            get
            {
                if(simulationEntries.ContainsKey(GUI_selectSimulationKey) == true)
                {
                    return simulationEntries[GUI_selectSimulationKey];
                }
                else
                {
                    return null;
                }
            }
        }
        

        [HideInInspector]
        [OdinSerialize, NonSerialized]
        private InnerKeyList<string, SimulationEntry> simulationEntries = new InnerKeyList<string, SimulationEntry>();

        [JsonProperty("simulationEntries")]
        public InnerKeyList<string, SimulationEntry> SimulationEntries => simulationEntries;


        protected override void OnEnable()
        {
            base.OnEnable();
            GUI_selectSimulationKey = SimulationEntries?.FirstOrDefault()?.Key ?? "";
        }


        public IList<string> GetSimulationKeys()
        {
            return simulationEntries.Keys.ToList();
        }

        [HorizontalGroup("Simulation/Control")]
        [VerticalGroup("Simulation/Control/Item")]
        [Button("Add", Style = ButtonStyle.CompactBox)]
        public void AddSimulationEntry(string key)
        {
            if (string.IsNullOrWhiteSpace(key) == true)
            {
                Debug.LogError($"[{GetType().Name}] Key must not be empty");
                return;
            }
            if (simulationEntries.ContainsKey(key) == true)
            {
                Debug.LogError($"[{GetType().Name}] SimulateData[{key}] is already exist");
                return;
            }
            simulationEntries.Add(new SimulationEntry(key));
            this.GUI_selectSimulationKey = key;
        }


        [VerticalGroup("Simulation/Control/Item")]
        [ShowIf("@GUI_selectEntry != null")]
        [Button("Remove")]
        public void RemoveCurSimulationEntry()
        {
            simulationEntries.RemoveAll(x => x.Key == GUI_selectEntry.Key);
            GUI_selectSimulationKey = "";
        }


    }


}

