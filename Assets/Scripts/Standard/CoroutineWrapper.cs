using Sirenix.OdinInspector;
using System.Collections;

namespace UAM
{
    public class CoroutineWrapper : IEnumerator
    {
        private IEnumerator m_Coroutine;
        [ReadOnly, ShowInInspector]
        private bool m_Alive = true;
        public bool IsAlive
        {
            get
            {
                return m_Alive;
            }
        }
        public object Current
        {
            get
            {
                return m_Coroutine.Current;
            }
        }
        public CoroutineWrapper(IEnumerator aCoroutine)
        {
            m_Coroutine = aCoroutine;
        }
        public bool MoveNext()
        {
            m_Alive = m_Coroutine.MoveNext();
            return m_Alive;
        }
        public void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}
