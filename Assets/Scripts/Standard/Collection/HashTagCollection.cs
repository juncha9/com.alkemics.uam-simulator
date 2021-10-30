using System;

namespace UAM
{
    [Serializable]
    public class HashTagCollection : ObservableHashSet<string> 
    {
        public HashTagCollection() : base()
        {

        }
    }
}

