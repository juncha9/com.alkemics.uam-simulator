using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace Alkemic.UAM
{
    [Serializable]
    public class Selector<T> where T : IKeyContainer<string>
    {
        [ValueDropdown("@")]
        private string key;

        [ShowInInspector]
        private T target;

        private IEnumerable<T> items = null;

        public Selector()
        {

        }

        public Selector(IEnumerable<T> items)
        {

        }
    }


}

