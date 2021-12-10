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
        [SerializeField]
        public List<RouteData> RouteDatas = new List<RouteData>();

        #region [ EDITING ]


        [ShowInInspector]
        [NonSerialized]
        private bool isEditing = false;

        [ShowIf("@isEditing == true")]
        [ValueDropdown("@Alkemic.UAM.UAMSimulator.GetSimulators()")]
        [ShowInInspector]
        private UAMSimulator simulator;

        [ShowIf("@isEditing == true && simulator != null")]
        [ValueDropdown("@GetWays(simulator)")]
        public List<Way> editWays = new List<Way>();

        #endregion

        private IList<Way> GetWays(UAMSimulator simulator)
        {
            if(simulator == null || simulator.LocationControl == null) return new List<Way>();

            return simulator.LocationControl.Ways;
        }

        [Button]
        public void EditRoute()
        {
            if(isEditing == true)
            {
                isEditing = false;
            }
            else
            {
                isEditing = true;
            }


        }
    }


}

