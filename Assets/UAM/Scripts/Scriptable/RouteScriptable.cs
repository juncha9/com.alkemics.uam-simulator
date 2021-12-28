using Alkemic.Scriptables;
using Alkemic.UI;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using UnityEngine;

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
            GUI_Simulator = GameObject.FindObjectsOfType<UAMSimulator>().Where(x => x != null).FirstOrDefault();
        }

        private void OnEnable()
        {
            ForceReload();
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
            if (inst == null)
            {
                Debug.LogError($"[{nameof(RouteScriptable)}] {DebugEx.FAILED} to load, Path: {path}");
                return false;
            }

            this.RouteDatas.Clear();
            this.RouteDatas.AddRange(inst.RouteDatas);
            return true;
        }

        [Button]
        private void ForceLoadRoute()
        {
            try
            {
                StreamReader reader = new StreamReader("./Route.txt");
                while(true)
                {
                    if(reader.EndOfStream == true)
                    {
                        break;
                    }
                    var line = reader.ReadLine();
                    var texts = line.Split(';');
                    var src = texts[0];
                    var dest = texts[1];
                    var route = texts[2];
                    Debug.Log(src);
                    Debug.Log(dest);
                    Debug.Log(route);


                }
            }
            catch(Exception ex)
            {
                Debug.LogError($"[{GetType().Name}] Failed to load, Ex: {ex}");
            }
        }
    }


}

