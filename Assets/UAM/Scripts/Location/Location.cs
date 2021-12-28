using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Specialized;
using System;
using System.Linq;
using Alkemic.Collections;
using Alkemic.UI;
using UnityEditor.UIElements;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Alkemic.UAM
{

    public enum LocationEditMode
    {
        None,
        DrawMode,
        WayMode,
    }

    [ExecuteInEditMode]
    [RequireComponent(typeof(DataCache))]
    public class Location : LeadComponent
    {
        public const string GROUP_WAYS = "Ways";
        public const string GROUP_WAY_EDITING = "WayEditing";

        #region [ EDIT_MODE ]

        [TitleGroup(GROUP_WAY_EDITING)]
        [ShowOnly]
        private LocationEditMode editMode = LocationEditMode.None;
        public LocationEditMode EditMode
        {
            set => editMode = value;
            get => editMode;
        }

        [TitleGroup(GROUP_WAY_EDITING)]
        [GUIColor("@EditMode == LocationEditMode.DrawMode ? Color.yellow : Color.white")]
        [Button]
        private void DrawMode()
        {
            if (this.EditMode == LocationEditMode.DrawMode)
            {
                EditMode = LocationEditMode.None;
            }
            else
            {
                EditMode = LocationEditMode.DrawMode;
            }
        }

        [TitleGroup(GROUP_WAY_EDITING)]
        [GUIColor("@EditMode == LocationEditMode.WayMode ? Color.yellow : Color.white")]
        [Button]
        private void WayMode()
        {
            if (this.EditMode == LocationEditMode.WayMode)
            {
                EditMode = LocationEditMode.None;
            }
            else
            {
                EditMode = LocationEditMode.WayMode;
            }

        }

        #endregion

        [PresetComponent]
        [SerializeField]
        private Transform airAnchor;
        public Transform AirAnchor => airAnchor;

        [PresetComponent]
        [SerializeField]
        private Transform groundAnchor;
        public Transform GroundAnchor => groundAnchor;

        [CacheGroup]
        [Debug]
        private UAMSimulator simulator;
        public UAMSimulator Simulator => simulator;

        [PresetComponent]
        [SerializeField]
        private Transform wayParent;
        public Transform WayParent => wayParent;

        [CacheGroup]
        [Debug]
        private LocationControl parentLocationControl;
        public LocationControl ParentLocationControl => parentLocationControl;

        [InfoBox("Duplicate", "@ParentLocationControl.CheckIsDuplicated(this.key)", InfoMessageType = InfoMessageType.Error)]
        [PropertyGroup]
        [SerializeField]
        private string key;
        public string Key
        {
            protected set
            {
                key = value;
            }
            get => key;
        }

        [TitleGroup(GROUP_WAYS)]
        [ListDrawerSettings(HideAddButton = true)]
        [SerializeField]
        private DestroyableList<Way> nextWays = new DestroyableList<Way>();
        public DestroyableList<Way> NextWays => nextWays;

        [TitleGroup(GROUP_WAYS)]
        [ShowOnly]
        public List<Way> PreWays
        {
            get
            {
                if (ParentLocationControl == null)
                {
                    return new List<Way>();
                }
                else
                {
                    return ParentLocationControl.Locations
                    .Where(x => x != null && x.NextWays.Contains(y => y != null && y.LocationB == this))
                    .Select(x => x.NextWays.Find(y => y.LocationB == this))
                    .ToList();
                }
            }
        }

        [TitleGroup(GROUP_WAYS)]
        [ShowOnly]
        public List<Way> ableWays
        {
            get
            {
                List<Way> locations = new List<Way>();
                locations.AddRange(NextWays);
                locations.AddRange(PreWays.Where(x => x.IsOneWay == false));
                return locations;
            }
        }

        [InstanceGroup]
        [ShowOnly]
        private DestroyableList<VTOL> existsVTOLS = new DestroyableList<VTOL>();
        public DestroyableList<VTOL> ExistsVTOLS => existsVTOLS;

        private void OnDrawGizmos()
        {
            var style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 12;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleLeft;
            Handles.Label(transform.position + (Vector3.up * 1000f), $"{this.Key}", style);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            SetupName();
            //Manage Way

            SyncWays();
        }


        protected virtual void SetupName()
        {
            if (EditorHelper.IsPrefabMode == false)
            {
                this.name = $"{GetType().Name} [{Key}]";
            }

            DataCache?.Set(DataCache.Path.KEY, Key);
        }

        private void SyncWays()
        {
            var _ways = GetComponentsInChildren<Way>().ToList();

            var addWays = _ways.Where(x => nextWays.Contains(x) == false).ToArray();
            foreach(var way in addWays)
            {
                nextWays.Add(way);
            }

            var removeWays = nextWays.Where(x => x == null || _ways.Contains(x) == false).ToArray();
            foreach(var way in removeWays)
            {
                nextWays.Remove(way);
            }
            
            /*
            foreach (var way in _ways)
            {
                if (way.LocationA == null || way.LocationB == null)
                {
                    UnityEditor.EditorApplication.delayCall += () => { DestroyImmediate(way.gameObject); };
                }
            }
            */
        }


        protected override void OnPreAwake()
        {
            base.OnPreAwake();

            this.CacheComponentInParent(ref simulator);
            this.CacheComponentInParent(ref parentLocationControl);
        }

        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(simulator != null, $"[{name}:{GetType().Name}] {nameof(simulator)} is null", gameObject);
            Debug.Assert(parentLocationControl != null, $"[{name}:{GetType().Name}] {nameof(parentLocationControl)} is null", gameObject);

            if (parentLocationControl != null && parentLocationControl.Locations.Contains(this) == false)
            {
                parentLocationControl.Locations.Add(this);
            }
            StartAutoCoroutine(SyncWayRoutine());
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            ExistsVTOLS.CollectionChanged += OnExistVTOLsCollectionChanged;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            ExistsVTOLS.CollectionChanged -= OnExistVTOLsCollectionChanged;
        }


        private IEnumerator SyncWayRoutine()
        {
            WaitForSeconds delay = new WaitForSeconds(0.5f);

            while (true)
            {
                SyncWays();
                yield return delay;
            }
        }

        private void OnExistVTOLsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (object item in e.NewItems)
                {
                    var target = item as VTOL;
                    if (target == null) continue;

                    target.OnLocationArrived.Invoke(this);
                }
            }
        }


        [PropertyGroup]
        [Button]
        private void GenerateKey()
        {
            this.Key = Guid.NewGuid().ToString().Split('-')[0];
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        [TitleGroup(GROUP_WAY_EDITING)]
        [Button]
        public void AddWay(
            [ValueDropdown("@ParentLocationControl?.GetLocationList() ?? new List<string>()")]
            string targetLocationKey, bool isOneWay = false)
        {
            if (parentLocationControl == null)
            {
                Debug.LogError($"[{name}:{GetType().Name}] Parent location is not exist", gameObject);
                return;
            }

            var location = parentLocationControl.Locations.Find(x => x.Key == targetLocationKey);
            if (location != null)
            {
                AddWay(location);
            }
            else
            {
                Debug.LogError($"[{name}:{GetType().Name}] Location[{targetLocationKey}] is not exist in this instance", gameObject);
            }
        }

        public Way AddWay(Location location, bool isOneWay = false)
        {
            if (ableWays.Contains(x => x.LocationB == location) == true)
            {
                Debug.LogError($"[{name}:{GetType().Name}] Location is already exist", gameObject);
                return null;
            }
            if (wayParent == null)
            {
                Debug.LogError($"[{name}:{GetType().Name}] Way parent is not exist", gameObject);
                return null;
            }
            var go = new GameObject();
            go.transform.SetParent(wayParent);
            go.transform.localPosition = new Vector3(0f, 0f, 0f);
            var newWay = go.AddComponent<Way>();
            newWay.Setup(location, isOneWay);
            nextWays.Add(newWay);
            return newWay;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Hit")
            {
                var vtol = other.GetComponentInParent<VTOL>();
                if (vtol != null)
                {
                    if (ExistsVTOLS.Contains(vtol) == false)
                    {
                        ExistsVTOLS.Add(vtol);
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Hit")
            {
                var vtol = other.GetComponentInParent<VTOL>();
                if (vtol != null)
                {
                    ExistsVTOLS.Remove(vtol);
                }
            }
        }



    }


}
