using Linefy.Primitives;
using UnityEditor;
using UnityEngine;

namespace Linefy.Editors
{
    [CustomEditor(typeof(LinefyCylinder))]
    public class CylinderEditor : MonoBehaviourEditorsBase
    {

        [MenuItem("GameObject/3D Object/Linefy/Primitives/Cylinder", false, 2)]
        public static void Create(MenuCommand menuCommand)
        {
            GameObject go = LinefyCylinder.CreateInstance().gameObject;
            postCreate(go, menuCommand);
        }
    }
}
