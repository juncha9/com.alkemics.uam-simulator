using UnityEngine;

namespace Alkemic.UAM
{
    public class Looker : BaseComponent
    {

        [PropertyGroup]
        [ShowOnly]
        private Transform target = null;
        //[ShowInInspector]
        public Transform Target
        {
            set => target = value;
            get => target;
        }

        [OptionGroup]
        [Range(0.01f, 1f)]
        [SerializeField]
        private float turnSpeed = 0.01f;
        public float TurnSpeed
        {
            set => turnSpeed = value;
            get => turnSpeed;
        }

        protected override void Awake()
        {
            base.Awake();
            enabled = false;
        }

        private void Update()
        {
            if (Target == null) return;
            if (IsDebug == true)
            {
                Debug.DrawLine(this.transform.position, Target.position, Color.green, 1f);
            }
            Vector3 dir = (Target.position - transform.position).normalized;
            var goal = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, goal, TurnSpeed);


        }
    }


}
