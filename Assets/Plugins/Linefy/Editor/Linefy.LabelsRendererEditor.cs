using UnityEditor;
using UnityEngine;

namespace Linefy.Editors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LinefyLabelsRenderer))]
    public class LinefyLabelsRenderEditor : MonoBehaviourEditorsBase
    {
        [MenuItem("GameObject/3D Object/Linefy/LabelsRenderer", false, 1)]
        public static void Create(MenuCommand menuCommand)
        {
            GameObject go = LinefyLabelsRenderer.CreateInstance().gameObject;
            postCreate(go, menuCommand);
        }
    }
}
