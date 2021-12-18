using Alkemic.Scriptables;
using Alkemic.Editors;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Alkemic.Collections;
using Alkemic.UI;
using Newtonsoft.Json;
using System;

namespace Alkemic.UAM
{


    public class RouteScriptable : Scriptable
    {

#if UNITY_EDITOR
        [HeaderGroup]
        [JsonIgnore, NonSerialized]
        //[ValueDropdown("@Alkemic.UAM.UAMSimulator.GetSimulators()")]
        [ShowInInspector]
        public UAMSimulator GUI_Simulator = null;

        [Button]
        private void ForceReload()
        {
            GUI_Simulator = GameObject.FindObjectOfType<UAMSimulator>();
        }

        private void OnEnable()
        {
            GUI_Simulator = GameObject.FindObjectOfType<UAMSimulator>();
        }

        private void OnDisable()
        {
            GUI_Simulator = null;
        }

        [JsonIgnore]
        public RouteData EditingRoute
        {
            get
            {
                return RouteDatas.Find(x => x.IsEdit == true);
            }
        }
#endif

        [JsonProperty("routeDatas")]
        [SerializeField]
        public List<RouteData> RouteDatas = new List<RouteData>();

        [Button("SaveJSON")]
        public bool SaveJSON(string path) //where T : Scriptable
        {
            /*
            if (this is T == false)
            {
                Debug.LogError($"[{typeof(T).Name}] Instance is not Type[{typeof(T).Name}]");
                return false;
            }
            */
            //T _inst = this as T;
            return IOHelper.SaveJSON(this, path);
        }

        [Button("LoadJSON")]
        public bool LoadJSON(string path)
        {
            RouteScriptable inst = IOHelper.LoadJSON<RouteScriptable>(path);
            if(inst == null)
            {
                Debug.LogError($"[{nameof(RouteScriptable)}] {DebugEx.FAILED} to load, Path: {path}");
                return false;
            }

            this.RouteDatas.Clear();
            this.RouteDatas.AddRange(inst.RouteDatas);
            return true;
        }


    }


}

