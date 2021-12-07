using UnityEditor;

namespace Alkemic.UAM
{
    [CustomEditor(typeof(MoveTask))]
    public class MoveTaskEditor : TaskEditor
    {
        private new MoveTask target => base.target as MoveTask;
    }
}
