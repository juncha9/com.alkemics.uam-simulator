using Alkemic.Scriptables;
using UnityEngine;

namespace Alkemic.UAM
{
    public static class UAM
    {
        public const string KEY_ROUTE = "route";

        public const string KEY_SIMULATION = "simulation";

        [HideInInspector]
        public static RouteScriptable Route => ScriptableDB.Scriptables[KEY_ROUTE] as RouteScriptable;

        [HideInInspector]
        public static SimulationScriptable Simulation => ScriptableDB.Scriptables[KEY_SIMULATION] as SimulationScriptable;

    }


}

