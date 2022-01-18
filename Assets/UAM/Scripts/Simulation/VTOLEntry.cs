using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Alkemic.UAM
{
    [Serializable]
    [HideReferenceObjectPicker]
    public class VTOLEntry : IKeyContainer<string>
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

        [OnValueChanged("set_" + nameof(MinSpeed))]
        [MinMaxSlider(UAMDefine.MinVTOLSpeed, UAMDefine.MaxVTOLSpeed)]
        [SerializeField]
        private float minSpeed = UAMDefine.MinVTOLSpeed;
        [JsonProperty("minSpeed")]
        public float MinSpeed
        {
            set
            {
                if (value < UAMDefine.MinVTOLSpeed)
                {
                    minSpeed = UAMDefine.MinVTOLSpeed;
                }
                else if (value > UAMDefine.MaxVTOLSpeed)
                {
                    minSpeed = UAMDefine.MaxVTOLSpeed;
                }
                else
                {
                    minSpeed = value;
                }
            }
            get => maxSpeed;
        }

        [OnValueChanged("set_"+nameof(MaxSpeed))]
        [MinMaxSlider(UAMDefine.MinVTOLSpeed, UAMDefine.MaxVTOLSpeed)]
        [SerializeField]
        private float maxSpeed = UAMDefine.MinVTOLSpeed;
        [JsonProperty("maxSpeed")]
        public float MaxSpeed
        {
            set
            {
                if(value < UAMDefine.MinVTOLSpeed)
                {
                    maxSpeed = UAMDefine.MinVTOLSpeed;                    
                }
                else if(value > UAMDefine.MaxVTOLSpeed)
                {
                    maxSpeed = UAMDefine.MaxVTOLSpeed;
                }
                else
                {
                    maxSpeed = value;
                }
            }
            get => maxSpeed;
        }

        public VTOLEntry() { }
        public VTOLEntry(string key)
        {
            this.Key = key;
        }
    }


}

