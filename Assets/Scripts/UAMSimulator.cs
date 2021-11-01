using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UAM
{
    public class UAMSimulator : Behavior
    {
        [SerializeField]
        private bool m_IsMain = false;
        public bool isMain => m_IsMain;

        [SerializeField]
        private GameObject m_EVTOLPrefab;
        public GameObject EVTOLPrefab => m_EVTOLPrefab;

        [SerializeField]
        private Transform m_EVTOLParent;
        public Transform EVTOLParent => m_EVTOLParent;


        [SerializeField]
        private Color m_LineColor = Color.yellow;
        public Color lineColor => m_LineColor;

        private Color m_OneDirLineColor = Color.red;
        public Color oneWayLineColor => m_OneDirLineColor;

        [Range(2f, 10f)]
        [SerializeField]
        private float m_LineWidth = 2f;
        public float lineWidth => m_LineWidth;

        [ReadOnly, ShowInInspector]
        private LocationControl m_LocationControl;
        public LocationControl locationControl => m_LocationControl;

        [SerializeField]
        private int m_UAMCount = 50;
        public int UAMCount => m_UAMCount;

        [ReadOnly, ShowInInspector]
        private List<EVTOL> m_EVTOLs = new List<EVTOL>();
        public List<EVTOL> EVTOLs => m_EVTOLs;

        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            if(m_LocationControl == null)
            {
                m_LocationControl = GetComponentInChildren<LocationControl>();
            }
        }

        protected override void Start()
        {
            base.Start();

            StartAutoCoroutine(CheckEVTOLRoutine());
        }

        [Button]
        private void StartSim()
        {

            for (int i =0; i< UAMCount; i++)
            {
                var evtol = Instantiate(m_EVTOLPrefab, m_EVTOLParent)?.GetComponent<EVTOL>();
                evtol.name = $"EVTOL [{i + 1}]";
                evtol.speed = 1000f + (Random.Range(-1f, 1f) * 500f);
                
                this.EVTOLs.Add(evtol);

                var locations = locationControl.locations;
                Location randomLocation = locations[Random.Range(0, locations.Count)];
                if(randomLocation != null)
                {
                    evtol.transform.position = randomLocation.transform.position;
                }
                else
                {
                    Debug.LogError($"[{name}] Error on create evtol", gameObject);
                }
                
            }
        }


        private IEnumerator CheckEVTOLRoutine()
        {
            WaitForSeconds delay = new WaitForSeconds(0.1f);
            while(true)
            {
                foreach(var evtol in EVTOLs)
                {
                    if (evtol.isTasking == true) continue;
                    if (evtol.curLocation == null) continue;
                    var locations = evtol.curLocation.ableLocations;
                    var targetLoc = locations[Random.Range(0, locations.Count)];
                    if(targetLoc != null)
                    {
                        evtol.taskControl.AddTask<EVTOL_MoveTask>(
                            (task) =>
                            {
                                task.destLocation = targetLoc;
                            });
                    }
                }
                yield return delay;
            }
        }



    }


}
