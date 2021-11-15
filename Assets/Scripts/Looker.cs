using Sirenix.OdinInspector;
using UnityEngine;

namespace UAM
{
    public class Looker : Behavior
    {
        public Transform m_Target = null;
        [ShowInInspector]
        public Transform target
        {
            set => m_Target = value;
            get => m_Target;
        }

        [SerializeField]
        private float m_TurnSpeed = 0.01f;
        public float turnSpeed
        {
            set => m_TurnSpeed = value;
            get => m_TurnSpeed;
        }



        protected override void Awake()
        {
            base.Awake();
            enabled = false;
        }


        private void Update()
        {
            if (target == null) return;

            Vector3 dir = (target.position - transform.position).normalized;
            var goal = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, goal, turnSpeed);
        }
    }


}
