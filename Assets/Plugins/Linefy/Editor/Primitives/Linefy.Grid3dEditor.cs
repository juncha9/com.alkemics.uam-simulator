using Linefy.Primitives;
using UnityEditor;
using UnityEngine;

namespace Linefy.Editors
{
    [CustomEditor(typeof(LinefyGrid3d))]
    public class Grid3dEditor : MonoBehaviourEditorsBase
    {

        [MenuItem("GameObject/3D Object/Linefy/Primitives/Grid3d", false, 4)]
        public static void Create(MenuCommand menuCommand)
        {
            GameObject go = LinefyGrid3d.CreateInstance().gameObject;
            postCreate(go, menuCommand);
        }
    }
}
