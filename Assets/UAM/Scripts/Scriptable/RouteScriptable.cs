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

        [JsonProperty("routeDatas")]
        [SerializeField]
        public List<RouteData> RouteDatas = new List<RouteData>();

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


        protected override void OnEnable()
        {
            base.OnEnable();
            ForceReload();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
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
            if (UAM.Route?.GUI_Simulator == null) return;
            var simulator = UAM.Route?.GUI_Simulator;
            try
            {
                StreamReader reader = new StreamReader("./Route.txt");
                RouteDatas.Clear();

                while(true)
                {
                    if(reader.EndOfStream == true)
                    {
                        break;
                    }
                    var line = reader.ReadLine();
                    var texts = line.Split(';');
                    var key = texts[0];
                    var src = texts[1];
                    var dest = texts[2];
                    var wayPoints = texts[3].Split(' ').ToList();
                    if (wayPoints.Count <= 2)
                    {
                        Debug.LogError($"[{GetType().Name}] [{key}] Route has one way");
                        continue;
                    }

                    wayPoints.Insert(0, src);
                    wayPoints.Add(dest);

                    string prePoint = null;
                    string curPoint = null;

                    List<Way> ways = new List<Way>();
                    for(int i = 0; i < wayPoints.Count; i++)
                    {
                        curPoint = wayPoints[i];
                        if(prePoint != null && curPoint != null)
                        {
                            Way way = null;

                            var wayA = simulator.LocationControl.Ways.Find(x => x.LocationA.Key == prePoint && x.LocationB.Key == curPoint);
                            var wayB = simulator.LocationControl.Ways.Find(x => x.LocationA.Key == curPoint && x.LocationB.Key == prePoint);
                            if(wayA != null && wayB == null)
                            {
                                way = wayA;
                            }
                            else if(wayA == null && wayB != null)
                            {
                                way = wayB;
                            }
                            else
                            {
                                ways = null;
                                break;
                            }
                            if(way != null)
                            {
                                ways.Add(way);
                            }
                        }
                        prePoint = wayPoints[i];
                    }

                    if(ways == null)
                    {
                        Debug.LogError($"[{GetType().Name}] [{key}] Duplicate or wrong way");
                        continue;
                    }

                    string log = $"Data[{key}]: ";
                    foreach(var way in ways)
                    {
                        log += $"{way.Key} / ";
                    }
                    Debug.Log(log);

                    RouteData route = new RouteData();
                    route.Key = key;
                    route.Source = src;
                    route.Destination = dest;
                    route.Ways.AddRange(ways.Select(x => x.Key));
                    this.RouteDatas.Add(route);
                }
            }
            catch(Exception ex)
            {
                Debug.LogError($"[{GetType().Name}] Failed to load, Ex: {ex}");
            }
        }
#endif


    }
}

