using Linefy;
using UnityEngine;

namespace Alkemic
{
    [ExecuteInEditMode]
    public class TestScript : BaseComponent
    {

        [SerializeField]
        Vector3 altPos;

        Lines line = new Lines(1);

        private void Update()
        {
            line[0] = new Line(transform.position, altPos, Color.red, 100f);

            line.Draw();
        }

    }
}
