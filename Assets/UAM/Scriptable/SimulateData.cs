using Alkemic.Collections;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Alkemic.UAM
{

    [Serializable]
    [HideReferenceObjectPicker]
    public class SimulateData : IKeyContainer<string>
    {

        [SerializeField]
        private string key;
        public string Key
        {
            set => key = value;
            get => key;
        }

        /// <summary>
        /// VTOL 종류별 최고속도
        /// </summary>
        [SerializeField]
        private Dictionary<VTOLType, float> maxSpeedsByVTOLType = new Dictionary<VTOLType, float>();
        public Dictionary<VTOLType, float> MaxSpeedsByVTOLType => maxSpeedsByVTOLType;

        /// <summary>
        /// 이륙 간격 (분)
        /// </summary>
        [SerializeField]
        public float TakeOffDelay = 5f;

        /// <summary>
        /// 버티포트별 데이터
        /// </summary>
        [SerializeField]
        public InnerKeyList<string, VertiPortData> VertiPortDatas = new InnerKeyList<string, VertiPortData>();

    }


}

