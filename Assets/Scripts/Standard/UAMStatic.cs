using UnityEngine;

namespace UAM
{
    public static class UAMStatic
    {

        public const float TIME_TICK = 0.1f;

        public static readonly WaitForSeconds TICK = new WaitForSeconds(TIME_TICK);

        public static Color LineColor = Color.yellow;

        public static Color AltLineColor = Color.red;

        public static float lineWidth = 2f;


        public static float knotPHour2Speed
        {
            get
            {
                return 1000f / 60f;
            }
        }

        public static float speed2KnotPHour
        {
            get
            {
                return (60f / 1000f);
            }
        }
    }
}
