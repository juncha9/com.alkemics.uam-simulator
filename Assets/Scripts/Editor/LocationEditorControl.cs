using Sirenix.OdinInspector.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UAM
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(WayPoint))]
    public class WayPointEditor : LocationEditor
    {

    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Station))]
    public class StationEditor : LocationEditor
    {

    }


    [CanEditMultipleObjects]
    [CustomEditor(typeof(Location))]
    public class LocationEditor : OdinEditor
    {
        private new Location target => base.target as Location;

        private static Object s_LocationPrefab;

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

        protected override void OnEnable()
        {
            base.OnEnable();
            s_LocationPrefab = Resources.Load("Location");

            SceneView.duringSceneGui -= UpdateSceneView;
            SceneView.duringSceneGui += UpdateSceneView;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if(target.EditMode != LocationEditMode.None)
            {
                target.EditMode = LocationEditMode.None;
            }

            SceneView.duringSceneGui -= UpdateSceneView;

        }

        private void UpdateSceneView(SceneView sceneView)
        {
            Vector2 pos;
            Ray ray;

            if (target.EditMode != LocationEditMode.None)
            {
                
                if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                {
                    pos = Event.current.mousePosition;
                    float ppp = EditorGUIUtility.pixelsPerPoint;
                    pos.y = sceneView.camera.pixelHeight - pos.y * ppp;
                    pos.x *= ppp;
                    ray = sceneView.camera.ScreenPointToRay(pos);

                    Debug.DrawLine(ray.origin + new Vector3(0, 0, -1000f), ray.origin + new Vector3(0, 0, 1000f)
                        , Color.red, 10f);
                    Debug.DrawLine(ray.origin + new Vector3(0, -1000f, 0), ray.origin + new Vector3(0, 1000f, 0)
                        , Color.red, 10f);
                    Debug.DrawLine(ray.origin + new Vector3(-1000f, 0, 0), ray.origin + new Vector3(1000f, 0, 0)
                        , Color.red, 10f);
                    Debug.DrawLine(ray.origin, ray.origin + (ray.direction * -1000f)
                        , Color.red, 10f);

                    switch (target.EditMode)
                    {
                        case LocationEditMode.DrawMode:
                            {
                                var locationControl = target.ParentLocationControl;

                                Vector3 pointsPos = ray.origin;

                                pointsPos.y = locationControl.transform.position.y;
                                //Todo create object here at pointsPos

                                var newLocation = (PrefabUtility.InstantiatePrefab(s_LocationPrefab) as GameObject)?.GetComponent<WayPoint>();
                                newLocation.transform.position = pointsPos;
                                newLocation.transform.parent = locationControl.locationParent;
                                newLocation.EditMode = LocationEditMode.DrawMode;
                                if (locationControl.locations.Contains(newLocation) == false)
                                {
                                    locationControl.locations.Add(newLocation);
                                }
                                Way way = target.gameObject.AddComponent<Way>();
                                way.Setup(target, newLocation);
                                target.NextWays.Add(way);
                                EditorUtility.SetDirty(target);

                                // Avoid the current event being propagated
                                // I'm not sure which of both works better here
                                //Event.current.Use();
                                //Event.current = null;

                                target.EditMode = LocationEditMode.None;
                                Selection.activeGameObject = newLocation.gameObject;
                            }
                            break;
                        case LocationEditMode.WayMode:
                            {

                                var raycastResults = Physics.RaycastAll(ray);

                                var hits = raycastResults.Where(x => x.collider.tag == "Hit").Select(x => x.collider.gameObject);
                                
                                WayPoint location = null;
                                foreach (var hit in hits)
                                {
                                    var _location = hit.GetComponentInParent<WayPoint>();
                                    if (_location != null 
                                        && Equals(target, _location) == false
                                        && target.PreWays.Contains(x => x.From ==_location) == false)
                                    {
                                        location = _location;
                                        break;
                                    }
                                }

                                if(location != null && target.NextWays.Contains(x => x.To == location) == false)
                                {
                                    Way way = target.gameObject.AddComponent<Way>();
                                    if (Event.current.control == true)
                                    {
                                        way.Setup(target, location, true);
                                    }
                                    else
                                    {
                                        way.Setup(target, location, false);
                                    }
                                    target.NextWays.Add(way);
                                    EditorUtility.SetDirty(target);
                                }
                            }
                            break;
                    }

                }

            }
               

                
                
                // Keep the created object in focus
                //Selection.activeGameObject = target.gameObject;

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
