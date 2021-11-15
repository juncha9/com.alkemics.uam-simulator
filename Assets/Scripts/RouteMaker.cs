using System.Collections.Generic;
using UnityEngine;

namespace UAM
{
    public class RouteMaker : Behavior
    {

        [SerializeField]
        private List<Way> ways = new List<Way>();
        public List<Way> Ways => ways;

    }


}

