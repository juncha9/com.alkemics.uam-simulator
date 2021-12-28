using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Alkemic.UAM
{
    [Serializable]
    public class RouteData
    {
        [JsonProperty("key")]
        public string Key = "";

        [JsonProperty("source")]
        [HideIf("@isEdit == true")]
        public string Source;

        [JsonProperty("destination")]
        [HideIf("@isEdit == true")]
        public string Destination;

        [JsonProperty("ways")]
        [HideIf("@isEdit == true")]
        public List<string> Ways = new List<string>();

#if UNITY_EDITOR

        [NonSerialized]
        private bool isEdit = false;
        [JsonIgnore]
        public bool IsEdit => isEdit;

        private static UAMSimulator GUI_Simulator => UAM.Route?.GUI_Simulator;

        [JsonIgnore, NonSerialized]
        [ValueDropdown("@GetVertiPorts()")]
        [ShowIf("@isEdit == true")]
        [ShowInInspector]
        public Location GUI_Source = null;

        [JsonIgnore, NonSerialized]
        [ValueDropdown("@GetWays()")]
        [ShowIf("@isEdit == true")]
        [ShowInInspector]
        public List<Way> GUI_Ways = new List<Way>();

        public static IList<VertiPort> GetVertiPorts()
        {
            if (GUI_Simulator?.LocationControl == null) return new List<VertiPort>();

            return GUI_Simulator.LocationControl.Locations
                .Where(x => x is VertiPort)
                .Select(x => x as VertiPort)
                .ToList();
        }

        public static IList<Way> GetWays()
        {
            if (GUI_Simulator?.LocationControl == null) return new List<Way>();

            return GUI_Simulator.LocationControl.Ways;
        }

        private string GUI_CheckDestinationKey()
        {
            if(GUI_Ways.Count >= 2)
            {
                var lastWay = this.GUI_Ways[GUI_Ways.Count - 1];
                var preLastWay = this.GUI_Ways[GUI_Ways.Count - 2];
                
                if(lastWay.LocationA.Key == preLastWay.LocationA.Key || lastWay.LocationA.Key == preLastWay.LocationB.Key)
                {
                    //LocationB가 최종목적지
                    return lastWay.LocationB.Key;
                }
                else if(lastWay.LocationB.Key == preLastWay.LocationA.Key || lastWay.LocationB.Key == preLastWay.LocationB.Key)
                {
                    //LocationA가 최종목적지
                    return lastWay.LocationA.Key;
                }
                else
                {
                    //올바르지 않은 로케이션일 경우
                    Debug.LogError($"[{GetType().Name}] Ways are not valid");
                    return null;
                }
            }
            else if(GUI_Ways.Count == 1)
            {
                var way = this.GUI_Ways[0];
                if(way.LocationA.Key == GUI_Source.Key)
                {
                    //LocationB가 도착지
                    return way.LocationB.Key;
                }
                else if(way.LocationB.Key == GUI_Source.Key)
                {
                    //LocationA가 도착지
                    return way.LocationA.Key;
                }
                else
                {
                    Debug.LogError($"[{GetType().Name}] Ways are not valid");
                    return null;
                }
            }
            else
            {
                Debug.LogError($"[{GetType().Name}] No way exist");
                return null;
            }
        }

        [ShowIf("@GUI_Simulator?.LocationControl != null")]
        [GUIColor("@isEdit ? Color.yellow : Color.white")]
        [Button("Edit")]
        private void GUI_Edit()
        {
            if (isEdit == false)
            {
                isEdit = true;
                GUI_OnStartEdit();
            }
            else
            {
                isEdit = false;
                GUI_OnEndEdit();
            }
        }

        private void GUI_OnStartEdit()
        {
            var findedSource = GUI_Simulator.LocationControl.Locations.Find(x => x.Key == Source);
            if (findedSource != null)
            {
                this.GUI_Source = findedSource;
            }
            else
            {
                this.GUI_Source = null;
            }

            this.GUI_Ways.Clear();
            foreach (var key in Ways)
            {
                var _findedWay = GUI_Simulator.LocationControl.Ways.Find(x => x.Key == key);
                if (_findedWay != null)
                {
                    this.GUI_Ways.Add(_findedWay);
                }
                else
                {
                    this.GUI_Ways.Add(null);
                }
            }
        }

        private void GUI_OnEndEdit()
        {
            this.Source = this.GUI_Source?.Key ?? "";
            this.Destination = GUI_CheckDestinationKey() ?? "";

            this.Ways.Clear();
            this.Ways.AddRange(GUI_Ways.Select(x => x.Key));
        }
#endif

    }
}

