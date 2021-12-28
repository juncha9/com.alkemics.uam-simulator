using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Alkemic.UAM
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Task))]
    public class TaskEditor : OdinEditor
    {
        private new Task target => base.target as Task;
    }
}
