using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Alkemic.UAM
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Way))]
    public class WayEditor : OdinEditor
    {
        private Location parentLocation;

        protected new Way target => base.target as Way;

        protected override void OnEnable()
        {
            base.OnEnable();
            parentLocation = target?.ParentLocation;
        }


        protected virtual void OnDestroy()
        {
            target.ParentLocation.NextWays.RemoveAll(x => x == target);
            //parentLocation.NextWays.RemoveAll(x => x == null);
        }

    }
}
