using Alkemic.Scriptables;
using Alkemic.Editors;
using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Alkemic.UAM
{

    [Serializable]
    public class RouteScriptable : Scriptable
    {
        
        [ShowInInspector]
        public RouteData EditingRouteData = null;

        [SerializeField]
        public List<RouteData> RouteDatas = new List<RouteData>();

        #region [ EDITING ]

        [ShowIf("@EditingRouteData != null")]
        [ValueDropdown("@Alkemic.UAM.UAMSimulator.GetSimulators()")]
        [ShowInInspector]
        private UAMSimulator simulator;

        #endregion

        private IList<Way> GetWays(UAMSimulator simulator)
        {
            if(simulator == null || simulator.LocationControl == null) return new List<Way>();

            return simulator.LocationControl.Ways;
        }

        [ShowIf("@EditingRouteData != null && simulator != null")]
        [Button]
        public void AddWay([ValueDropdown("@GetWays(simulator)")] Way way)
        {
            if (EditingRouteData == null) return;


        }
    }


}

