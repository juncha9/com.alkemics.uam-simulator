using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Alkemic.UAM
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Way))]
    public class WayEditor : OdinEditor
    {

        protected new Way target => base.target as Way;

        protected override void OnEnable()
        {
            base.OnEnable();
            //parentLocation = target?.ParentLocation;
        }


        protected virtual void OnDestroy()
        {

        }

    }
}
