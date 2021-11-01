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
    
    public class Location : Behavior
    {
        public enum EditMode
        {
            None,
            DrawMode,
            WayMode,
        }

        private const float MAX_WIDTH_FACTOR = 3f;
        private const float MIN_WIDTH_FACTOR = -1f;
        private const float DEFAULT_LINE_WIDTH = 2f;

        [ReadOnly, ShowInInspector]
        private EditMode m_EditMode = EditMode.None;
        public EditMode editMode
        {
            set => m_EditMode = value;
            get => m_EditMode;
        }

        [ReadOnly, ShowInInspector]
        private UAMSimulator m_ParentSimulator;

        [ReadOnly, ShowInInspector]
        private LocationControl m_ParentLocationControl;

        [SerializeField]
        private GameObject m_UAMPrefab;

        [SerializeField]
        private string m_Key;
        public string key
        {
            private set => m_Key = value;
            get => m_Key;
        }
        
        [ReadOnly, ShowInInspector]
        private Lines m_WayLines = null;

        [ReadOnly, ShowInInspector]
        private Lines m_OneWayLines = null;

        [ListDrawerSettings(HideAddButton = true)]
        [SerializeField]
        private List<Location> m_OneSideLocations = new List<Location>();
        public List<Location> oneSideLocations => m_OneSideLocations;

        [ListDrawerSettings(HideAddButton = true)]
        [SerializeField]
        private List<Location> m_NextLocations = new List<Location>();
        public List<Location> nextLocations => m_NextLocations;

        [ReadOnly, ShowInInspector]
        public List<Location> preLocations
        {
            get
            {
                if (m_ParentLocationControl == null)
                {
                    return new List<Location>();
                }
                else
                {
                    return m_ParentLocationControl.locations
                    .Where(x => x.nextLocations.Contains(this) == true)
                    .ToList();
                }
            }
        }

        [ReadOnly, ShowInInspector]
        public List<Location> ableLocations
        {
            get
            {
                List<Location> locations = new List<Location>();
                locations.AddRange(nextLocations);
                locations.AddRange(preLocations);
                locations.AddRange(oneSideLocations);
                return locations;
            }
        }

        [ReadOnly, ShowInInspector]
        private DestroyableList<EVTOL> m_HandleVTOLS = new DestroyableList<EVTOL>();
        public DestroyableList<EVTOL> handleVTOLS => m_HandleVTOLS;

        [GUIColor("@editMode == EditMode.DrawMode ? Color.yellow : Color.white")]
        [Button]
        private void DrawMode()
        {
            if(this.editMode == EditMode.DrawMode)
            {
                editMode = EditMode.None;
            }
            else
            {
                editMode = EditMode.DrawMode;
            }
        }

        [GUIColor("@editMode == EditMode.WayMode ? Color.yellow : Color.white")]
        [Button]
        private void WayMode()
        {
            if(this.editMode == EditMode.WayMode)
            {
                editMode = EditMode.None;
            }
            else
            {
                editMode = EditMode.WayMode;
            }

        }

        [Button]
        private void GenerateKey()
        {
            this.key = Guid.NewGuid().ToString().Split('-')[0];
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        #region [ EDITOR ]

        private void ResizeLines()
        {
            if (m_WayLines == null || (m_NextLocations.Count != m_WayLines.count))
            {
                m_WayLines = new Lines(m_NextLocations.Count);
            }
            if (m_OneWayLines == null || (m_OneSideLocations.Count != m_OneWayLines.count))
            {
                m_OneWayLines = new Lines(m_OneSideLocations.Count);
            }
        }

        private void OnDrawGizmos()
        {
            ResizeLines();

            Color lineColor;
            float lineWidth;

            if (m_OneSideLocations != null)
            {
                lineColor = m_ParentSimulator?.oneWayLineColor ?? Color.red;
                lineWidth = m_ParentSimulator?.lineWidth ?? DEFAULT_LINE_WIDTH;
                for (int i = 0; i < m_OneSideLocations.Count; i++)
                {

                    if (m_OneSideLocations[i] == null) continue;
                    m_OneWayLines[i] = new Line(new Vector3(0, 0, 0),
                                m_OneSideLocations[i].transform.position - this.transform.position,
                                lineColor,
                                lineColor,
                                lineWidth + MAX_WIDTH_FACTOR,
                                lineWidth + MIN_WIDTH_FACTOR);
                }
            }
            if (m_OneWayLines != null && m_OneWayLines.count > 0)
            {
                m_OneWayLines.DrawNow(transform.localToWorldMatrix);
            }

            if (m_NextLocations != null)
            {
                lineColor = m_ParentSimulator?.lineColor ?? Color.yellow;
                lineWidth = m_ParentSimulator?.lineWidth ?? DEFAULT_LINE_WIDTH;
                for (int i = 0; i < m_NextLocations.Count; i++)
                {

                    if (m_NextLocations[i] == null) continue;
                    m_WayLines[i] = new Line(new Vector3(0, 0, 0),
                                m_NextLocations[i].transform.position - this.transform.position,
                                lineColor,
                                lineColor,
                                lineWidth + MAX_WIDTH_FACTOR,
                                lineWidth + MAX_WIDTH_FACTOR);
                }
            }
            if (m_WayLines != null && m_WayLines.count > 0)
            {
                m_WayLines.DrawNow(transform.localToWorldMatrix);
            }


        }

        #endregion
        protected override void OnValidate()
        {
            base.OnValidate();

            name = $"LOC[{key}]";

            ableLocations.RemoveAll(x => x == null);
        }


        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            if(m_ParentSimulator == null)
            {
                m_ParentSimulator = GetComponentInParent<UAMSimulator>();
            }

            if(m_ParentLocationControl == null)
            {
                m_ParentLocationControl = GetComponentInParent<LocationControl>();
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if(m_ParentLocationControl != null && m_ParentLocationControl.locations.Contains(this) == false)
            {
                m_ParentLocationControl.locations.Add(this);
            }

            handleVTOLS.CollectionChanged += OnHandleVTOLCollectionChanged;
        }

        private void Update()
        {
            ResizeLines();

            Color lineColor;
            float lineWidth;

            lineColor = m_ParentSimulator?.oneWayLineColor ?? Color.red;
            lineWidth = m_ParentSimulator?.lineWidth ?? DEFAULT_LINE_WIDTH;
            for (int i = 0; i < m_OneSideLocations.Count; i++)
            {

                if (m_OneSideLocations[i] == null) continue;
                m_OneWayLines[i] = new Line(this.transform.position,
                            m_OneSideLocations[i].transform.position,
                            lineColor,
                            lineColor,
                            lineWidth + MAX_WIDTH_FACTOR,
                            lineWidth + MIN_WIDTH_FACTOR);
            }


            if (m_OneWayLines != null && m_OneWayLines.count > 0)
            {
                m_OneWayLines.Draw();
            }

            lineColor = m_ParentSimulator?.lineColor ?? Color.yellow;
            lineWidth = m_ParentSimulator?.lineWidth ?? DEFAULT_LINE_WIDTH;

            for (int i = 0; i < m_NextLocations.Count; i++)
            {
                if (m_NextLocations != null)
                {
                    m_WayLines[i] = new Line(this.transform.position,
                                m_NextLocations[i].transform.position,
                                lineColor,
                                lineColor,
                                lineWidth + MAX_WIDTH_FACTOR,
                                lineWidth + MIN_WIDTH_FACTOR);
                }
            }

            if (m_WayLines != null && m_WayLines.count > 0)
            {
                m_WayLines.Draw();
            }
            
        }

        private void OnHandleVTOLCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach(object item in e.NewItems)
                {
                    var target = item as EVTOL;
                    if (target == null) continue;

                    target.onLocationArrived.Invoke(this);
                }
            }
        }



        [Button]
        public void AddLocation(
            [ValueDropdown("@m_ParentLocationControl?.GetLocationList() ?? new List<string>()")]
            string key)
        {
            if(m_ParentLocationControl == null)
            {
                Debug.LogError($"[{name}] Parent location is not exist", gameObject);
                return;
            }

            var location = m_ParentLocationControl.locations.Find(x => x.key == key);
            if(location != null)
            {
                if(ableLocations.Contains(x => x == location) == true)
                {
                    Debug.LogError($"[{name}] Location is already exist", gameObject);
                    return;
                }

                ableLocations.Add(location);
            }
            else
            {
                Debug.LogError($"[{name}] Location[{key}] is not exist in this instance", gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Hit")
            {
                var vtol = other.GetComponentInParent<EVTOL>();
                if(vtol != null)
                {
                    if(handleVTOLS.Contains(vtol) == false)
                    {
                        handleVTOLS.Add(vtol);
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.tag == "Hit")
            {
                var vtol = other.GetComponentInParent<EVTOL>();
                if(vtol != null)
                {
                    handleVTOLS.Remove(vtol);
                }
            }
        }


        /*
        [Button]
        public void CreateUAMToNex()
        {
            if (nextLocation == null) return;

            if (m_ParentSimulator == null) return;

            var uam = Instantiate(m_UAMPrefab, m_ParentSimulator.uamParent).GetComponent<UAM>();
            uam.transform.position = transform.position;
            uam.SetTargetLocation(nextLocation);

        }
        */
    }


}
