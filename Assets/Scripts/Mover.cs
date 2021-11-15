using Sirenix.OdinInspector;
using UnityEngine;

namespace UAM
{ 
    public class Mover : Behavior
    {

        private float m_TargetSpeed;
        [ShowInInspector]
        public float targetSpeed
        {
            protected set
            {
                if(value < 0) m_TargetSpeed = 0f;
                else m_TargetSpeed = value;
            }
            get => m_TargetSpeed;
        }

        private float m_Speed;
        [ShowInInspector]
        protected float speed
        {
            private set
            {
                if (value < 0) m_Speed = 0f;
                else m_Speed = value;
            }
            get => m_Speed;
        }

        private Vector3? m_Direction;
        public Vector3? direction
        {
            set => m_Direction = value;
            get => m_Direction;
        }

        protected override void Awake()
        {
            base.Awake();

            enabled = false;
        }


        private void FixedUpdate()
        {
            if (enabled == false) return;

            speed = Mathf.Lerp(speed, targetSpeed, 2f * Time.unscaledTime);
            if(direction.HasValue == true)
            {
                transform.Translate(direction * speed * Time.unscaledTime);
            }
            else
            {
                transform.Translate(transform.forward * speed * Time.unscaledTime);
            }
            
        }

    }


}
