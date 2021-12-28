using UnityEngine;

namespace Alkemic.UAM
{
    public static class UAMStatic
    {

        public const float TICK_TIME = 0.05f;

        public static readonly WaitForSeconds TICK = new WaitForSeconds(TICK_TIME);

        public static Color LineColor = ColorDefine.SAFFRON;

        public static Color AltLineColor = ColorDefine.BURNING_ORANGE;

        public static Color VertiPortLineColor = ColorDefine.BLUE_BRIGHT;

        public static float LineWidth = 2f;

        public const float MinVTOLSpeed = 40f;

        public const float MaxVTOLSpeed = 200f;

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

        public static float DefaultAirHeight = 1000f;
        public static float DefaultLandHeight = 0f;
    }
}
