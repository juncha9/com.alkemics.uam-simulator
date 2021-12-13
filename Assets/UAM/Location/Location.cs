using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Specialized;
using System;
using System.Linq;
using Alkemic.Collections;
using Alkemic.UI;

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

        [CacheGroup]
        [Debug]
        private UAMSimulator parentSimulator;
        public UAMSimulator ParentSimulator => parentSimulator;

        [CacheGroup]
        [Debug]
        private LocationControl parentLocationControl;
        public LocationControl ParentLocationControl => parentLocationControl;

        [PresetComponent]
        [SerializeField]
        private TextWrapper keyTextMesh;

        [InfoBox("Duplicate", "@ParentLocationControl.CheckIsDuplicated(this.key)", InfoMessageType = InfoMessageType.Error)]
        [PropertyGroup]
        [SerializeField]
        private string key;
        public string Key
        {
            protected set => key = value;
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

        protected override void OnValidate()
        {
            base.OnValidate();

            ReloadName();

            if (keyTextMesh != null)
            {
                this.keyTextMesh.Text = this.Key;
            }

            //Manage Way

            nextWays.RemoveAll(x => x == null);

            foreach (var way in GetComponentsInChildren<Way>())
            {
                if (way.LocationA == null || way.LocationB == null)
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.delayCall += () => { DestroyImmediate(way.gameObject); };
#endif
                }
            }
        }

        


        protected override void OnPreAwake()
        {
            base.OnPreAwake();

            this.CacheComponentInParent(ref parentSimulator);
            this.CacheComponentInParent(ref parentLocationControl);
        }

        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(parentSimulator != null, $"[{name}] {nameof(parentSimulator)} is null", gameObject);
            Debug.Assert(parentLocationControl != null, $"[{name}] {nameof(parentLocationControl)} is null", gameObject);

            if (parentLocationControl != null && parentLocationControl.Locations.Contains(this) == false)
            {
                parentLocationControl.Locations.Add(this);
            }

            nextWays.CollectionChanged += OnNextWaysCollectionChanged;

            ExistsVTOLS.CollectionChanged += OnExistVTOLsCollectionChanged;
        }

        private void OnDrawGizmos()
        {

            var style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 12;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleLeft;
            Handles.Label(transform.position + (Vector3.up * 1000f), $"{this.Key}", style);


        }

        private void ReloadName()
        {
            if (Helper.IsPrefabMode == false)
            {
                this.name = $"{GetType().Name} [{Key}]";
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

                    target.LocationArrived.Invoke(this);
                }
            }
        }

        private void OnNextWaysCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var way = e.OldItems[0] as Way;
                Destroy(way);
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                var ways = GetComponents<Way>();
                foreach (var way in ways)
                {
                    Destroy(way);
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
                Debug.LogError($"[{name}] Parent location is not exist", gameObject);
                return;
            }

            var location = parentLocationControl.Locations.Find(x => x.Key == targetLocationKey);
            if (location != null)
            {
                AddWay(location);
            }
            else
            {
                Debug.LogError($"[{name}] Location[{targetLocationKey}] is not exist in this instance", gameObject);
            }
        }

        public void AddWay(Location location, bool isOneWay = false)
        {
            if (ableWays.Contains(x => x.LocationB == location) == true)
            {
                Debug.LogError($"[{name}] Location is already exist", gameObject);
                return;
            }
            var go = new GameObject();
            go.transform.SetParent(this.transform);
            go.transform.localPosition = new Vector3(0f, 0f, 0f);
            var newWay = go.AddComponent<Way>();
            newWay.Setup(this, location, isOneWay);
            nextWays.Add(newWay);
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
