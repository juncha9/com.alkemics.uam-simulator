using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace Alkemic.UAM
{
    [Serializable]
    public class RouteData
    {
       
        [JsonProperty("vertiPortKey")]
        public string VertiPortKey = "";

        [JsonProperty("ways")]
        public List<string> WayKeys = new List<string>();

        [JsonIgnore]
        public bool isEditing
        {
            get
            {
                return UAM.Route.EditingRouteData == this;
            }
        }

        [GUIColor("@isEditing?Color.yellow : Color.white")]
        [Button]
        public void Edit()
        {
            if(isEditing == true)
            {
                UAM.Route.EditingRouteData = null;
            }
            else
            {
                UAM.Route.EditingRouteData = this;
            }
            
        }

    }


}

