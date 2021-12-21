using Linefy.Primitives;
using UnityEditor;
using UnityEngine;

namespace Linefy.Editors
{
    [CustomEditor(typeof(LinefyCone))]
    public class ConeEditor : MonoBehaviourEditorsBase
    {

        [MenuItem("GameObject/3D Object/Linefy/Primitives/Cone", false, 2)]
        public static void Create(MenuCommand menuCommand)
        {
            GameObject go = LinefyCone.CreateInstance().gameObject;
            postCreate(go, menuCommand);
        }
    }
}
