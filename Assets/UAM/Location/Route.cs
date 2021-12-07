using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Alkemic.UAM
{
    [Serializable]
    public class Route : BaseComponent
    {
        public const string GROUP_EDIT = "Edit";

        [CacheGroup]
        [Debug]
        private LocationControl parentLocationControl;
        public LocationControl ParentLocationControl => parentLocationControl;

        private bool useEdit = false;
        public bool UseEdit => useEdit;

        [ShowIf("@UseEdit == true")]
        [ValueDropdown("@ParentLocationControl?.GetWayList()")]
        [ShowInInspector]
        private List<Way> editingWays = new List<Way>();

        [OptionGroup]
        [HideIf("@UseEdit == true")]
        [SerializeField]
        private string startLocationKey;

        [OptionGroup]
        [HideIf("@UseEdit == true")]
        [SerializeField]
        private string endLocationKey;

        [OptionGroup]
        [HideIf("@UseEdit == true")]
        [SerializeField]
        private List<string> wayKeys = new List<string>();
        public List<string> WayKeys => wayKeys;

        [InstanceGroup]
        [RuntimeOnly]
        private Location startLocation;

        [InstanceGroup]
        [RuntimeOnly]
        public Location endLocation;

        [InstanceGroup]
        [RuntimeOnly]
        private List<Way> ways = new List<Way>();
        public List<Way> Ways => ways;

        protected override void OnPreAwake()
        {
            base.OnPreAwake();

            this.CacheComponentInParent(ref parentLocationControl);
        }

        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(parentLocationControl != null, $"[{name}] {nameof(parentLocationControl)} is null", gameObject);

            foreach(var key in wayKeys)
            {
                var way = parentLocationControl.Ways.Find(x => x.Key == key);
                if(way != null)
                {
                    ways.Add(way);
                }
                else
                {
                    Debug.LogError($"[{name}] Way[{key}] instance is not exist", gameObject);
                }
            }
            var firstWay = ways.FirstOrDefault();
            if(firstWay != null)
            {
                startLocation = firstWay.From;
            }
            var lastWay = ways.LastOrDefault();
            if(lastWay != null)
            {
                endLocation = lastWay.To;
            }
        }


        [TitleGroup(GROUP_EDIT)]
        [GUIColor("@useEdit == true ? Color.yellow : Color.white")]
        [Button("EditMode")]
        private void ToggleEditMode()
        { 

            if(useEdit == true)
            {
                useEdit = false;
                this.wayKeys.Clear();
                this.wayKeys.AddRange(editingWays.Select(x => x.Key));
                this.editingWays.Clear();
            }
            else
            {
                useEdit = true;
            }
        
        }

        

    }
}