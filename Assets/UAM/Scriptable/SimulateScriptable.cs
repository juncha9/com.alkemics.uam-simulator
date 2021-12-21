using Alkemic.Collections;
using Alkemic.Scriptables;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;

namespace Alkemic.UAM
{



    [ShowOdinSerializedPropertiesInInspector]
    public class SimulateScriptable : Scriptable
    {

        private string selectSimulateKey;

        [ValueDropdown("@SimulateDatas")]
        [ShowInInspector]
        private SimulateData selectSimulate
        {
            get
            {
                if (string.IsNullOrWhiteSpace(selectSimulateKey) == true)
                {
                    return null;
                }
                else if (simulateDatas.ContainsKey(selectSimulateKey) == true)
                {
                    return null;
                }
                else
                {
                    return null;
                }
            }
        }


        [HideInInspector]
        [OdinSerialize, NonSerialized]
        private InnerKeyList<string, SimulateData> simulateDatas = new InnerKeyList<string, SimulateData>();
        public InnerKeyList<string, SimulateData> SimulateDatas => simulateDatas;


        [Button]
        public void AddSimulate(string key)
        {
            if (simulateDatas.ContainsKey(key) == true)
            {
                Debug.LogError($"[{nameof(SimulateScriptable)}] SimulateData[{key}] is already exist");
                return;
            }
        }




    }


}

