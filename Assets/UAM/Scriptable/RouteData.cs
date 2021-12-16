using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Alkemic.UAM
{
    [Serializable]
    public class RouteData
    {
        [JsonProperty("key")]
        public string Key = "";

        [JsonProperty("vertiPort")]
        [HideIf("@isEdit == true")]
        public string VertiPort = "";

        [JsonProperty("ways")]
        [HideIf("@isEdit == true")]
        public List<string> Ways = new List<string>();

#if UNITY_EDITOR

        private bool isEdit = false;
        public bool IsEdit => isEdit;

        private static UAMSimulator GUI_Simulator => UAM.Route?.GUI_Simulator;

        [ValueDropdown("@GetVertiPorts()")]
        [ShowIf("@isEdit == true")]
        [ShowInInspector]
        public VertiPort GUI_VertiPort = null;

        [ValueDropdown("@GetWays()")]
        [ShowIf("@isEdit == true")]
        [ShowInInspector]
        public List<Way> GUI_Ways = new List<Way>();

        private static IList<VertiPort> GetVertiPorts()
        {
            if (GUI_Simulator?.LocationControl == null) return new List<VertiPort>();

            return GUI_Simulator.LocationControl.Locations
                .Where(x => x is VertiPort)
                .Select(x => x as VertiPort)
                .ToList();
        }

        private static IList<Way> GetWays()
        {
            if (GUI_Simulator?.LocationControl == null) return new List<Way>();

            return GUI_Simulator.LocationControl.Ways;
        }

        [ShowIf("@GUI_Simulator?.LocationControl != null")]
        [GUIColor("@isEdit ? Color.yellow : Color.white")]
        [Button("Edit")]
        private void GUI_Edit()
        {
            if (isEdit == false)
            {
                isEdit = true;
                GUI_SetupEditingProperties();
           }
            else
            {
                isEdit = false;
                GUI_UpdateByEditProperties();
            }
        }

        private void GUI_SetupEditingProperties()
        {
            var findedVertiPort = GUI_Simulator.LocationControl.Locations.Find(x => x.Key == VertiPort) as VertiPort;
            if (findedVertiPort != null)
            {
                this.GUI_VertiPort = findedVertiPort;
            }
            else
            {
                this.GUI_VertiPort = null;
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

        private void GUI_UpdateByEditProperties()
        {
            this.VertiPort = this.GUI_VertiPort?.Key ?? "";
            this.Ways.Clear();
            this.Ways.AddRange(GUI_Ways.Select(x => x.Key));
        }
#endif

    }
}

