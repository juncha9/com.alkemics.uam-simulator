using Newtonsoft.Json;
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

    }


}

