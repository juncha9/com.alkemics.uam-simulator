using JetBrains.Annotations;
using Linefy;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UAM;
using System.Collections.Specialized;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UAM
{
    
    public class Location : Behavior
    {
        private const float MAX_WIDTH_FACTOR = 3f;
        private const float MIN_WIDTH_FACTOR = -1f;
        private const float DEFAULT_LINE_WIDTH = 2f;

        private bool m_UseDrawMode = false;
        public bool useDrawMode
        {
            set => m_UseDrawMode = value;
            get =>m_UseDrawMode;
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
        private Lines m_Lines = null;

        [ListDrawerSettings(HideAddButton = true)]
        [SerializeField]
        private List<Location> m_NextLocations = new List<Location>();
        public List<Location> nextLocations => m_NextLocations;

        [ReadOnly, ShowInInspector]
        private DestroyableList<EVTOL> m_HandleVTOLS = new DestroyableList<EVTOL>();
        public DestroyableList<EVTOL> handleVTOLS => m_HandleVTOLS;

        [GUIColor("@useDrawMode == true ? Color.yellow : Color.white")]
        [Button]
        private void DrawMode()
        {
            useDrawMode = !useDrawMode;
        }

        [Button]
        private void GenerateKey()
        {
            this.key = Guid.NewGuid().ToString().Split('-')[0];
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            name = $"LOC[{key}]";

            nextLocations.RemoveAll(x => x == null);
        }

        private void OnDrawGizmos()
        {
            ResizeLines();

            var lineColor = m_ParentSimulator?.lineColor ?? Color.yellow;
            var lineWidth = m_ParentSimulator?.lineWidth ?? DEFAULT_LINE_WIDTH;

            for (int i = 0; i < m_NextLocations.Count; i++)
            {
                if (m_NextLocations != null)
                {
                    if (m_NextLocations[i] == null) continue;
                    m_Lines[i] = new Line(new Vector3(0, 0, 0),
                                m_NextLocations[i].transform.position - this.transform.position,
                                lineColor,
                                lineColor,
                                lineWidth + MAX_WIDTH_FACTOR,
                                lineWidth + MIN_WIDTH_FACTOR);
                }
            }

            if (m_Lines != null && m_Lines.count > 0)
            {
                m_Lines.DrawNow(transform.localToWorldMatrix);
            }
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

            var lineColor = m_ParentSimulator?.lineColor ?? Color.yellow;
            var lineWidth = m_ParentSimulator?.lineWidth ?? DEFAULT_LINE_WIDTH;

            for (int i = 0; i < m_NextLocations.Count; i++)
            {
                if (m_NextLocations != null)
                {
                    m_Lines[i] = new Line(this.transform.position,
                                m_NextLocations[i].transform.position,
                                lineColor,
                                lineColor,
                                lineWidth + MAX_WIDTH_FACTOR,
                                lineWidth + MIN_WIDTH_FACTOR);
                }
            }

            if (m_Lines != null && m_Lines.count > 0)
            {
                m_Lines.Draw();
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

        private void ResizeLines()
        {
            if (m_Lines != null && (m_NextLocations.Count == m_Lines.count))
            {
                return;
            }
            m_Lines = new Lines(m_NextLocations.Count);
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
                if(nextLocations.Contains(x => x == location) == true)
                {
                    Debug.LogError($"[{name}] Location is already exist", gameObject);
                    return;
                }

                nextLocations.Add(location);
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
