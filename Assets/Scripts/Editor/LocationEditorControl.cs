using Codice.Client.BaseCommands;
using Sirenix.OdinInspector.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

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
            if(target.editMode != Location.EditMode.None)
            {
                target.editMode = Location.EditMode.None;
            }

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
            
            if (s_MainUAMSimulator?.locationControl == null) return;

            Vector2 pos;
            Ray ray;

            if (target.editMode != Location.EditMode.None)
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

                    switch (target.editMode)
                    {
                        case Location.EditMode.DrawMode:
                            {
                                var locationControl = s_MainUAMSimulator.locationControl;

                                Vector3 pointsPos = ray.origin;

                                pointsPos.y = locationControl.transform.position.y;
                                //Todo create object here at pointsPos

                                var newLocation = (PrefabUtility.InstantiatePrefab(s_LocationPrefab) as GameObject)?.GetComponent<Location>();
                                newLocation.transform.position = pointsPos;
                                newLocation.transform.parent = locationControl.locationParent;
                                newLocation.editMode = Location.EditMode.DrawMode;
                                if (locationControl.locations.Contains(newLocation) == false)
                                {
                                    locationControl.locations.Add(newLocation);
                                }
                                target.nextLocations.Add(newLocation);
                                EditorUtility.SetDirty(target);

                                // Avoid the current event being propagated
                                // I'm not sure which of both works better here
                                //Event.current.Use();
                                //Event.current = null;

                                target.editMode = Location.EditMode.None;
                                Selection.activeGameObject = newLocation.gameObject;
                            }
                            break;
                        case Location.EditMode.WayMode:
                            {

                                var raycastResults = Physics.RaycastAll(ray);

                                var hits = raycastResults.Where(x => x.collider.tag == "Hit").Select(x => x.collider.gameObject);
                                
                                Location location = null;
                                foreach (var hit in hits)
                                {
                                    var _location = hit.GetComponentInParent<Location>();
                                    if (_location != null 
                                        && Equals(target, _location) == false
                                        && target.preLocations.Contains(_location) == false)
                                    {
                                        location = _location;
                                        break;
                                    }
                                }

                                if(location != null)
                                {
                                    if (Event.current.control == true)
                                    {
                                        if(target.oneSideLocations.Contains(location) == false)
                                        {
                                            target.oneSideLocations.Add(location);
                                            EditorUtility.SetDirty(target);
                                        }
                                    }
                                    else
                                    {
                                        if (target.nextLocations.Contains(location) == false)
                                        {
                                            target.nextLocations.Add(location);
                                            EditorUtility.SetDirty(target);
                                        }
                                    }
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
