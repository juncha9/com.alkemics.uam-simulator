#if UNITY_EDITOR
#endif

namespace Alkemic.UAM
{
    public class WayPoint : Location
    {

        protected override void SetupName()
        {
            base.SetupName();
            DataCache.Set(DataCache.Path.NAME, $"WP");
        }

    }
}
