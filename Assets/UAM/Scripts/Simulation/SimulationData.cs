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
    public class SimulationData : IKeyContainer<string>
    {
        [ReadOnly]
        [SerializeField]
        public string Key;
        string IKeyContainer<string>.Key => Key;

        /// <summary>
        /// 이륙 간격 (분)
        /// </summary>
        [SerializeField]
        public float TakeOffDelay = 5f;

        [VerticalGroup("VTOL")]
        [ListDrawerSettings(HideAddButton = true)]
        [SerializeField]
        private InnerKeyList<string, VTOLPreset> _VTOLPresets = new InnerKeyList<string, VTOLPreset>();
        public InnerKeyList<string, VTOLPreset> VTOLPresets => _VTOLPresets;
        [VerticalGroup("VTOL")]
        [Button]
        public void AddVTOLPreset(string key)
        {
            if (string.IsNullOrWhiteSpace(key) == true) return;
            if (VTOLPresets.ContainsKey(key) == true) return;

            VTOLPresets.Add(new VTOLPreset(key));
        }

        [VerticalGroup("VertiPort")]
        [ListDrawerSettings(HideAddButton = true)]
        [SerializeField]
        private InnerKeyList<string, VertiPortPreset> vertiPortPreset = new InnerKeyList<string, VertiPortPreset>();
        public InnerKeyList<string, VertiPortPreset> VertiPortPreset => vertiPortPreset;

        [VerticalGroup("VertiPort")]
        [Button]
        public void AddVertiPortPreset(string key)
        {
            if (string.IsNullOrWhiteSpace(key) == true) return;
            if (VertiPortPreset.ContainsKey(key) == true) return;

            VertiPortPreset.Add(new VertiPortPreset(key));
        }

        public SimulationData() { }

        public SimulationData(string key)
        {
            this.Key = key;
        }

        

    }


}

