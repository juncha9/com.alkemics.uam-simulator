using System;
using System.Collections.Generic;
using UnityEngine;

namespace Alkemic.UAM
{
    [Serializable]
    public class VertiPortData : IKeyContainer<string>
    {
        [SerializeField]
        private string key;
        public string Key
        {
            set => key = value;
            get => key;
        }

        /// <summary>
        /// 시간, 분만 적용
        /// </summary>
        public List<VTOLReserveData> VTOLReserves = new List<VTOLReserveData>();
    }


}

