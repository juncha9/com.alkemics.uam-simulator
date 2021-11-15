#if UNITY_EDITOR
#endif

using Linefy.Primitives;

namespace UAM
{
    public class Station : Location
    {

        private CircularPolyline circular;


        private void OnDrawGizmos()
        {



            circular = new CircularPolyline(64, 10f);
            circular.DrawNow(transform.localToWorldMatrix);
            



        }


    }


}
