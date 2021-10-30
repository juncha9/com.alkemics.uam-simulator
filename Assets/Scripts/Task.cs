using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using UnityEngine;

namespace UAM
{

    [Serializable]
    public class EVTOL_MoveTask : Task
    {
        [ReadOnly, ShowInInspector]
        private EVTOL m_Target;
        public EVTOL target => m_Target;

        [SerializeField]
        private Location m_DestLocation;
        public Location destLocation
        {
            private set => m_DestLocation = value;
            get => m_DestLocation;
        }

        protected override void Awake()
        {
            base.Awake();

            if (m_Target == null)
            {
                m_Target = parentTaskControl.target.GetComponent<EVTOL>();
            }

        }

        public void SetLocation(Location location)
        {
            this.m_DestLocation = location;
        }

        public override IEnumerator Logic()
        {
            if (this.m_DestLocation != this.destLocation)
            {
                this.m_DestLocation = this.destLocation;
            }

            yield return new WaitUntil(() => target.curLocation == this.destLocation);
        }

    }


    [Serializable]
    public class Task : Behavior
    {
        [ReadOnly, ShowInInspector]
        private TaskControl m_ParentTaskControl;
        public TaskControl parentTaskControl => m_ParentTaskControl;

        [ReadOnly, ShowInInspector]
        private bool m_IsCompleted = false;
        public bool isCompleted
        {
            private set => m_IsCompleted = value;
            get => m_IsCompleted;
        }

        [ReadOnly, ShowInInspector]
        private Func<Task, IEnumerator> m_Logic = null;
        public Func<Task, IEnumerator> logic => m_Logic;

        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            if(m_ParentTaskControl == null)
            {
                m_ParentTaskControl = GetComponentInParent<TaskControl>();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(m_ParentTaskControl != null, $"[{name}] {nameof(m_ParentTaskControl)} is null", gameObject);
        }

        public virtual IEnumerator Logic()
        {
            yield return null;
        }

        public void Complete()
        {
            this.isCompleted = true;
        }

        public Task()
        {

        }
    }


}
