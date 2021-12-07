using Sirenix.OdinInspector.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Alkemic;

namespace Alkemic.UAM
{


    [CanEditMultipleObjects]
    [CustomEditor(typeof(Location))]
    public class LocationEditor : OdinEditor
    {
        private LocationControl parentLocationControl;

        protected new Location target => base.target as Location;

        private GameObject wayPointPrefab;

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

            if(UAMManager.Inst?.WayPointPrefab != null)
            {
                this.wayPointPrefab = UAMManager.Inst.WayPointPrefab;
            }

            SceneView.duringSceneGui -= UpdateSceneView;
            SceneView.duringSceneGui += UpdateSceneView;

            target.CacheComponentInParent(ref parentLocationControl);

        }

        protected override void OnDisable()
        {
            base.OnDisable();

            this.wayPointPrefab = null;

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

            if(Event.current.type == EventType.MouseDown)
            {
                pos = Event.current.mousePosition;
                float ppp = EditorGUIUtility.pixelsPerPoint;
                pos.y = sceneView.camera.pixelHeight - pos.y * ppp;
                pos.x *= ppp;
                ray = sceneView.camera.ScreenPointToRay(pos);

                //Gizmos.DrawRay(ray);
                //Debug.DrawRay(ray.origin, ray.direction * 10000000, Color.cyan);

                //Debug.DrawLine(ray.origin, ray.origin + (ray.direction * Mathf.Infinity), Color.red, 10f);

                /*
                Debug.DrawLine(ray.origin + new Vector3(0, 0, -1000f), ray.origin + new Vector3(0, 0, 1000f)
                    , Color.red, 2f);
                Debug.DrawLine(ray.origin + new Vector3(0, -1000f, 0), ray.origin + new Vector3(0, 1000f, 0)
                    , Color.red, 2f);
                Debug.DrawLine(ray.origin + new Vector3(-1000f, 0, 0), ray.origin + new Vector3(1000f, 0, 0)
                    , Color.red, 2f);
                */

                if(Event.current.button == 0)
                {
                    var raycastResults = Physics.RaycastAll(ray);
                    var hits = raycastResults.Where(x => x.collider.isTrigger == true && x.collider.tag == "Hit").Select(x => x.collider.gameObject);
                    Location location = null;
                    foreach (var hit in hits)
                    {
                        var _location = hit.GetComponentInParent<Location>();
                        if (_location != null
                            && Equals(target, _location) == false
                            && target.PreWays.Contains(x => x.From == _location) == false)
                        {
                            location = _location;
                            break;
                        }
                    }
                    if(location != null)
                    {
                        var controlID = GUIUtility.GetControlID(FocusType.Passive);
                        GUIUtility.hotControl = controlID;
                        Selection.activeGameObject = location.gameObject;
                        Event.current.Use();
                    }
                }
                else if (Event.current.button == 1)
                {
                    switch (target.EditMode)
                    {
                        case LocationEditMode.DrawMode:
                            {

                                Vector3 pointsPos = ray.origin;
                                pointsPos.y = 0;
                                //Todo create object here at pointsPos

                                if (wayPointPrefab == null)
                                {
                                    Debug.LogError($"WayPointPrefab is null");
                                    return;
                                }

                                var newLocation = (PrefabUtility.InstantiatePrefab(wayPointPrefab) as GameObject)?.GetComponent<WayPoint>();
                                newLocation.transform.position = pointsPos;
                                newLocation.transform.parent = parentLocationControl.transform;
                                newLocation.EditMode = LocationEditMode.DrawMode;
                                if (parentLocationControl.Locations.Contains(newLocation) == false)
                                {
                                    parentLocationControl.Locations.Add(newLocation);
                                }
                                target.AddWay(newLocation);
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
                                var hits = raycastResults.Where(x => x.collider.isTrigger == true && x.collider.tag == "Hit").Select(x => x.collider.gameObject);

                                Location location = null;
                                foreach (var hit in hits)
                                {
                                    var _location = hit.GetComponentInParent<Location>();
                                    if (_location != null
                                        && Equals(target, _location) == false
                                        && target.PreWays.Contains(x => x.From == _location) == false)
                                    {
                                        location = _location;
                                        break;
                                    }
                                }

                                if (location != null && target.NextWays.Contains(x => x.To == location) == false)
                                {
                                    if (Event.current.control == true)
                                    {
                                        target.AddWay(location, true);
                                    }
                                    else
                                    {
                                        target.AddWay(location, false);
                                    }
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
