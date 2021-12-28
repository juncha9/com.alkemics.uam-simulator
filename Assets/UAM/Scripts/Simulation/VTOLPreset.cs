using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Alkemic.UAM
{
    [Serializable]
    [HideReferenceObjectPicker]
    public class VTOLPreset : IKeyContainer<string>
    {
        [ReadOnly]
        [SerializeField]
        public string Key;
        string IKeyContainer<string>.Key => Key;

        [SerializeField]
        public float MaxSpeed;

        public VTOLPreset() { }
        public VTOLPreset(string key)
        {
            this.Key = key;
        }
    }


}

