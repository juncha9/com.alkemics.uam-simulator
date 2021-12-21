using Linefy.Primitives;
using UnityEngine;

namespace UnityExamples
{
    public class DrawRuntime : MonoBehaviour
    {
        Box box;

        private void Start()
        {
            box = new Box();
        }

        private void Update()
        {
            box.Draw();
        }
    }
}
