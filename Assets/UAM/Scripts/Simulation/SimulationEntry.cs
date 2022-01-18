using Alkemic.Collections;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

namespace Alkemic.UAM
{

    [Serializable]
    [HideReferenceObjectPicker]
    public class SimulationEntry : IKeyContainer<string>
    {
        [ReadOnly]
        [SerializeField]
        private string key;
        public string Key
        {
            set => key = value;
            get => key;
        }

        /// <summary>
        /// 이륙 간격 (분)
        /// </summary>
        [SerializeField]
        public float TakeOffDelay = 5f;

        [TabGroup("EVTOL")]
        [ListDrawerSettings(HideAddButton = true)]
        [SerializeField]
        private InnerKeyList<string, VTOLEntry> eVTOLEntries = new InnerKeyList<string, VTOLEntry>();
        public InnerKeyList<string, VTOLEntry> EVTOLEntries => eVTOLEntries;

        [TabGroup("EVTOL")]
        [Button]
        public void AddVTOLPreset(string key)
        {
            if (string.IsNullOrWhiteSpace(key) == true) return;
            if (EVTOLEntries.ContainsKey(key) == true) return;

            EVTOLEntries.Add(new VTOLEntry(key));
        }

        [TabGroup("VPort")]
        [ListDrawerSettings(HideAddButton = true)]
        [SerializeField]
        private InnerKeyList<string, VertiPortEntry> vertiPortEntries = new InnerKeyList<string, VertiPortEntry>();

        public InnerKeyList<string, VertiPortEntry> VertiPortEntries => vertiPortEntries;

        [TabGroup("VPort")]
        [Button]
        public void AddVertiPortPreset(string key)
        {
            if (string.IsNullOrWhiteSpace(key) == true) return;
            if (VertiPortEntries.ContainsKey(key) == true) return;

            VertiPortEntries.Add(new VertiPortEntry(key));
        }

        public SimulationEntry() { }

        public SimulationEntry(string key)
        {
            
            this.Key = key;
        }
    }
}

