using UnityEngine;
using Alkemic.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System;

namespace Alkemic.UAM
{
    [Serializable]
    public class MapPair
    {
        [SerializeField]
        public string key;
        [SerializeField]
        public string name;

        [SerializeField]
        public Graphic graphic;
    }

    public class MapSelector : LeadComponent
    {
        [CacheComponent]
        private Picker picker;
        public Picker Picker => picker;

        [SerializeField]
        private List<MapPair> maps = new List<MapPair>();
        public List<MapPair> Maps => maps;

        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            this.CacheComponent(ref picker);
        }

        protected override void Awake()
        {
            base.Awake();

            picker.useSinglePick = true;
            picker.nameSelector = (x) =>
            {
                var mapPair = x as MapPair;
                if (mapPair != null)
                {
                    return mapPair.name;
                }
                else
                {
                    return "";
                }
            };
            picker.onItemPicked.AddListener(OnItemPicked);
        }

        protected override void Start()
        {
            base.Start();

            picker.SetOptions(Maps, new List<MapPair> { maps.FirstOrDefault() });
        }

        private void OnItemPicked(object target, bool value)
        {
            var pair = target as MapPair;
            if (pair == null) return;
            if(value == true)
            {
                pair.graphic.enabled = true;

            }
            if(value == false)
            {
                pair.graphic.enabled = false;
            }
        }




    }


}
