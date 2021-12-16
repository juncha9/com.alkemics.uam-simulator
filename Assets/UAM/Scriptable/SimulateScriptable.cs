using Alkemic.Collections;
using Alkemic.Scriptables;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Alkemic.UAM
{
    [ShowOdinSerializedPropertiesInInspector]
    public class SimulateScriptable : Scriptable
    {
        [OdinSerialize, NonSerialized]
        private Dictionary<string, SimulateData> simulateDatas = new Dictionary<string, SimulateData>();
        public Dictionary<string, SimulateData> SimulateDatas => simulateDatas;


    }


}

