using Alkemic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UAM
{
    public class Looker : BaseComponent
    {
        
        private Transform target = null;
        [ShowInInspector]
        public Transform Target
        {
            set => target = value;
            get => target;
        }

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

            Vector3 dir = Target.position - transform.position;
            var goal = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, goal, TurnSpeed);
        }
    }


}
