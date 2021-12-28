using System;

namespace Alkemic.UAM
{
    [Serializable]
    public class VTOLReserveData
    {
        public DateTime Time = new DateTime(0, 0, 0, 0, 0, 0);
        public VTOLType Type = VTOLType.ShortTerm;
        public int Count = 1;
        public string RouteKey = "";
    }


}

