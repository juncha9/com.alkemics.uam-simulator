using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Alkemic.UAM
{
    [Serializable]
    [HideReferenceObjectPicker]
    public class VertiPortEntry : IKeyContainer<string>
    {
        [ReadOnly]
        [SerializeField]
        private string key;
        [JsonProperty("key")]
        public string Key
        {
            set => key = value;
            get => key;
        }

        [OdinSerialize, NonSerialized]
        private Dictionary<string, int> _VTOLCounts = new Dictionary<string, int>();
        [JsonProperty("evtolCount")]
        public Dictionary<string, int> VTOLCounts => _VTOLCounts;

        public VertiPortEntry() { }
        public VertiPortEntry(string key)
        {
            this.Key = key;
        }
    }


}

