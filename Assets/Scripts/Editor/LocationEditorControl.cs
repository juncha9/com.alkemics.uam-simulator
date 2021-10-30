using Sirenix.OdinInspector.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UAM
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Location))]
    public class LocationCreateControl : OdinEditor
    {
        private static bool s_IsCreateMode;

        private static UAMSimulator s_MainUAMSimulator;

        private new Location target => base.target as Location;

        private static Object s_LocationPrefab;

        private static Location s_LastCreatedLocation;

        /*
        [MenuItem("UAM/EnableCreateMode")]
        private static void EnableCreateMode()
        {
            Debug.Log("CreateMode enabled");

            FindUAM();

            s_IsCreateMode = true;
            // Add a callback for SceneView update

            SceneView.duringSceneGui -= UpdateSceneView;
            SceneView.duringSceneGui += UpdateSceneView;
        }

        [MenuItem("UAM/DisableCreateMode")]
        private static void DisableCreateMode()
        {
            Debug.Log("CreateMode disabled");

            s_IsCreateMode = false;

            SceneView.duringSceneGui -= UpdateSceneView;
        }
        */

        private void OnEnable()
        {
            s_LocationPrefab = Resources.Load("Location");

            FindUAM();

            SceneView.duringSceneGui -= UpdateSceneView;
            SceneView.duringSceneGui += UpdateSceneView;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= UpdateSceneView;
        }

        private static void FindUAM()
        {
            var mainSimulator = GameObject.FindObjectsOfType<UAMSimulator>()
                           .Where(x => x.isMain == true)
                           .FirstOrDefault();

            s_MainUAMSimulator = mainSimulator;
        }

        private void UpdateSceneView(SceneView sceneView)
        {
            if (target.useDrawMode == false) return;
            if (s_MainUAMSimulator?.locationControl == null) return;

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                var locationControl = s_MainUAMSimulator.locationControl;

                var pos = Event.current.mousePosition;
                float ppp = EditorGUIUtility.pixelsPerPoint;
                pos.y = sceneView.camera.pixelHeight - pos.y * ppp;
                pos.x *= ppp;

                var ray = sceneView.camera.ScreenPointToRay(pos);

                Debug.DrawLine(ray.origin + new Vector3(0,0, -1000f), ray.origin + new Vector3(0,0,1000f)
                    , Color.red, 10f);
                Debug.DrawLine(ray.origin + new Vector3(0, -1000f, 0), ray.origin + new Vector3(0, 1000f, 0)
                    , Color.red, 10f);
                Debug.DrawLine(ray.origin + new Vector3(-1000f, 0, 0), ray.origin + new Vector3(1000f, 0, 0)
                    , Color.red, 10f);
                Debug.DrawLine(ray.origin, ray.origin + (ray.direction * -1000f)
                    , Color.red, 10f);

                Vector3 pointsPos = ray.origin;
                
                pointsPos.y = locationControl.transform.position.y;
                //Todo create object here at pointsPos

                var newLocation = (PrefabUtility.InstantiatePrefab(s_LocationPrefab) as GameObject)?.GetComponent<Location>();
                newLocation.transform.position = pointsPos;
                newLocation.transform.parent = locationControl.locationParent;
                newLocation.useDrawMode = true;
                if (locationControl.locations.Contains(newLocation) == false)
                {
                    locationControl.locations.Add(newLocation);
                }
                target.nextLocations.Add(newLocation);

                // Avoid the current event being propagated
                // I'm not sure which of both works better here
                //Event.current.Use();
                //Event.current = null;

                target.useDrawMode = false;
                Selection.activeGameObject = newLocation.gameObject;
                
                // Keep the created object in focus
                //Selection.activeGameObject = target.gameObject;

                // exit creation mode
            }

            /*
            if (isCreating)
            {
                
            }
            else
            {
                // Skip if event is Layout or Repaint
                
                if (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint)
                {
                    Selection.activeGameObject = lastCreated;
                    return;
                }

                // Prevent Propagation
                Event.current.Use();
                Event.current = null;
                Selection.activeGameObject = lastCreated;

                // Remove the callback
                SceneView.duringSceneGui -= UpdateSceneView;
            }
            */
        }
    }
}
