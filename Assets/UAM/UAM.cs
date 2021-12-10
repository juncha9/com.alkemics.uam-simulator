using Alkemic.Scriptables;
using System;
using UnityEngine;

namespace Alkemic.UAM
{
    public static class UAM
    {
        public const string KEY_ROUTE_SCRIPTABLE = "route";


        [HideInInspector]
        public static RouteScriptable RouteContainer => ScriptableDatabase.Scriptables[KEY_ROUTE_SCRIPTABLE] as RouteScriptable;
    }


}

