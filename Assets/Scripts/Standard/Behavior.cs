using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace UAM
{
    [ShowOdinSerializedPropertiesInInspector]
    public class Behavior : MonoBehaviour, IDebugable, IDestroyable, IStartHandler
    {
       
        [ShowIf("@useDebug == true")]
        [ReadOnly, ShowInInspector]
        private List<CoroutineWrapper> m_Coroutines = new List<CoroutineWrapper>();

        private IDestroyable.DestroyEvent m_OnDestroy = new IDestroyable.DestroyEvent();
        public IDestroyable.DestroyEvent onDestroy => m_OnDestroy;

        private IStartHandler.StartEvent m_OnStart = new IStartHandler.StartEvent();
        public IStartHandler.StartEvent onStart => m_OnStart;

        [ToggleLeft]
        [OnValueChanged("set_"+nameof(useDebug))]
        [SerializeField]
        private bool m_UseDebug = false;
        public bool useDebug
        {
            set => m_UseDebug = value;
            get => m_UseDebug;
        }

        /// <summary>
        /// 컴포넌트에 필요한 다른 컴포넌트를 찾아서 캐싱할때 이 메서드 안에서 구현
        /// * Awake, OnValidate 내부에서 실행됨
        /// </summary>
        protected virtual void OnPreAwake()
        {

        }

        /// <summary>
        /// 컴포넌트에 필요한 다른 컴포넌트를 찾아서 캐싱할때 이 메서드 안에서 구현
        /// * FindCache 메서드 보다 늦게 Start 내부에서 실행됨
        /// </summary>
        protected virtual void OnPreStart()
        {
            if (useDebug == true)
            {
                Debug.Log($"[{name}] On pre start", gameObject);
            }
        }

        protected virtual void Awake()
        {
            OnPreAwake();
            if (useDebug == true)
            {
                Debug.Log($"[{name}] On awake", gameObject);
            }

        }

        protected virtual void Start()
        {
            if(useDebug == true)
            {
                Debug.Log($"[{name}] On start", gameObject);
            }

            OnPreStart();
            onStart.Invoke();
        }

        protected virtual void OnValidate()
        {
            OnPreAwake();

           
            this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic).ForEach(fieldInfo =>
            {
                var fieldType = fieldInfo.FieldType;

                if (typeof(GameObjectAttachable).IsAssignableFrom(fieldInfo.FieldType) == true)
                {
                    var goAttacher = fieldInfo.GetValue(this) as GameObjectAttachable;
                    if (goAttacher != null)
                    {
                        goAttacher.gameObject = this.gameObject;
                    }
                }
                else if (typeof(IEnumerable<GameObjectAttachable>).IsAssignableFrom(fieldInfo.FieldType) == true)
                {
                    (fieldInfo.GetValue(this) as IEnumerable<GameObjectAttachable>)?
                    .ForEach(goAttacher =>
                    {
                        if (goAttacher != null)
                        {
                            goAttacher.gameObject = this.gameObject;
                        }
                    });
                }
            });
        }

        protected virtual void OnEnable()
        {
            if(useDebug == true)
            {
                Debug.Log($"[{name}] On enable", gameObject);
            }

            ClearDeadCoroutines();
            for (int i = 0; i < m_Coroutines.Count; i++)
            {
                if (m_Coroutines[i].IsAlive == true)
                {
                    StartCoroutine(m_Coroutines[i]);
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (useDebug == true)
            {
                Debug.Log($"[{name}] On disable", gameObject);
            }

            for (int i = 0; i < m_Coroutines.Count; i++)
            {
                StopCoroutine(m_Coroutines[i]);
            }
        }

        protected virtual void OnDestroy()
        {
            onDestroy.Invoke(this);
        }

        public void DestroyThis()
        {
            Destroy(gameObject);
        }

        private void ClearDeadCoroutines()
        {
            for (int i = m_Coroutines.Count - 1; i >= 0; i--)
            {
                if (m_Coroutines[i].IsAlive == false)
                    m_Coroutines.RemoveAt(i);
            }
        }

        public void StartAutoCoroutine(IEnumerator coroutine)
        {
            var instance = new CoroutineWrapper(coroutine);
            m_Coroutines.Add(instance);
            StartCoroutine(instance);
            ClearDeadCoroutines();
        }

    }
}