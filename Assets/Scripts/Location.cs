using JetBrains.Annotations;
using Linefy;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UAM;
using System.Collections.Specialized;
using System;
using UnityEngine.Serialization;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UAM
{

    public enum LocationEditMode
    {
        None,
        DrawMode,
        WayMode,
    }


    public class Location : Behavior
    {

        #region [ EDIT_MODE ]

        [ReadOnly, ShowInInspector]
        private LocationEditMode editMode = LocationEditMode.None;
        public LocationEditMode EditMode
        {
            set => editMode = value;
            get => editMode;
        }

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


        [ReadOnly, ShowInInspector]
        private UAMSimulator parentSimulator;
        public UAMSimulator ParentSimulator
        {
            get => parentSimulator;
        }

        [ReadOnly, ShowInInspector]
        private LocationControl parentLocationControl;
        public LocationControl ParentLocationControl
        {
            get => parentLocationControl;
        }

        [ListDrawerSettings(HideAddButton = true)]
        [SerializeField]
        private DestroyableList<Way> nextWays = new DestroyableList<Way>();
        public DestroyableList<Way> NextWays => nextWays;

        [ReadOnly, ShowInInspector]
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
                    return ParentLocationControl.locations
                    .Where(x => x.NextWays.Contains(y => y.To == this))
                    .Select(x => x.NextWays.Find(y => y.To == this))
                    .ToList();
                }
            }
        }

        [ReadOnly, ShowInInspector]
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

        [SerializeField]
        private string key;
        public string Key
        {
            protected set => key = value;
            get => key;
        }

        [ReadOnly, ShowInInspector]
        private DestroyableList<EVTOL> m_ExistsVTOLS = new DestroyableList<EVTOL>();
        public DestroyableList<EVTOL> existsVTOLS => m_ExistsVTOLS;

        protected override void OnValidate()
        {
            base.OnValidate();
            name = $"{GetType().Name}[{Key}]";
            nextWays.RemoveAll(x => x == null);

            foreach(var way in GetComponents<Way>())
            {
                if (nextWays.Contains(way) == false)
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.delayCall += () =>
                    {
                        DestroyImmediate(way);
                    };
#endif
                }
            }

        }

        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            if (parentSimulator == null)
            {
                parentSimulator = GetComponentInParent<UAMSimulator>();
            }
            if (parentLocationControl == null)
            {
                parentLocationControl = GetComponentInParent<LocationControl>();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (parentLocationControl != null && parentLocationControl.locations.Contains(this) == false)
            {
                parentLocationControl.locations.Add(this);
            }

            nextWays.CollectionChanged += NextWaysCollectionChanged;

            existsVTOLS.CollectionChanged += ExistVTOLsCollectionChanged;
        }

        private void ExistVTOLsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (object item in e.NewItems)
                {
                    var target = item as EVTOL;
                    if (target == null) continue;

                    target.LocationArrived.Invoke(this);
                }
            }
        }



        private void NextWaysCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                var way = e.OldItems[0] as Way;
                Destroy(way);
            }
            else if(e.Action == NotifyCollectionChangedAction.Reset)
            {
                var ways = GetComponents<Way>();
                foreach(var way in ways)
                {
                    Destroy(way);
                }
            }
        }


        [Button]
        private void GenerateKey()
        {
            this.Key = Guid.NewGuid().ToString().Split('-')[0];
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        [Button]
        public void AddWay(
            [ValueDropdown("@ParentLocationControl?.GetLocationList() ?? new List<string>()")]
            string key, bool isOneWay = false)
        {
            if (parentLocationControl == null)
            {
                Debug.LogError($"[{name}] Parent location is not exist", gameObject);
                return;
            }

            var location = parentLocationControl.locations.Find(x => x.Key == key);
            if (location != null)
            {
                if (ableWays.Contains(x => x.To == location) == true)
                {
                    Debug.LogError($"[{name}] Location is already exist", gameObject);
                    return;
                }

                var newWay = gameObject.AddComponent<Way>();
                newWay.Setup(this, location, isOneWay);
                nextWays.Add(newWay);
            }
            else
            {
                Debug.LogError($"[{name}] Location[{key}] is not exist in this instance", gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Hit")
            {
                var vtol = other.GetComponentInParent<EVTOL>();
                if (vtol != null)
                {
                    if (existsVTOLS.Contains(vtol) == false)
                    {
                        existsVTOLS.Add(vtol);
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Hit")
            {
                var vtol = other.GetComponentInParent<EVTOL>();
                if (vtol != null)
                {
                    existsVTOLS.Remove(vtol);
                }
            }
        }



    }


}
