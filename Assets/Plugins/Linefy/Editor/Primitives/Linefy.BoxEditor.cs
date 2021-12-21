using Linefy.Primitives;
using UnityEditor;
using UnityEngine;

namespace Linefy.Editors
{
    [CustomEditor(typeof(LinefyBox))]
    public class BoxEditor : MonoBehaviourEditorsBase
    {
        [MenuItem("GameObject/3D Object/Linefy/Primitives/Box", false, 1)]
        public static void Create(MenuCommand menuCommand)
        {
            GameObject go = LinefyBox.CreateInstance().gameObject;
            postCreate(go, menuCommand);
        }
    }
}
