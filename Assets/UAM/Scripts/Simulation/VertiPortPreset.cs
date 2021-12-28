using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Alkemic.UAM
{
    [Serializable]
    [HideReferenceObjectPicker]
    public class VertiPortPreset : IKeyContainer<string>
    {
        [ReadOnly]
        [SerializeField]
        public string Key;

        string IKeyContainer<string>.Key => Key;

        [OdinSerialize, NonSerialized]
        private Dictionary<string, int> _VTOLInitCounts = new Dictionary<string, int>();
        public Dictionary<string, int> VTOLInitCounts => _VTOLInitCounts;

        [SerializeField]
        public float Delay;

        public VertiPortPreset() { }
        public VertiPortPreset(string key)
        {
            this.Key = key;
        }
    }


}

