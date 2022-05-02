using Alkemic.Doings;
using UnityEditor;

namespace Alkemic.UAM
{
    [CustomEditor(typeof(MoveTask))]
    public class MoveTaskEditor : DoingEditor
    {
        private new MoveTask target => base.target as MoveTask;
    }
}
