using Alkemic.Scriptables;
using System;
using UnityEngine;

namespace Alkemic.UAM
{
    public static class UAM
    {
        public const string KEY_ROUTE_SCRIPTABLE = "route";
        public const string KEY_SIMULATE_SCRIPTABLE = "simulate";

        [HideInInspector]
        public static RouteScriptable Route => ScriptableDatabase.Scriptables[KEY_ROUTE_SCRIPTABLE] as RouteScriptable;

        [HideInInspector]
        public static SimulateScriptable Simulate => ScriptableDatabase.Scriptables[KEY_SIMULATE_SCRIPTABLE] as SimulateScriptable;

    }


}

