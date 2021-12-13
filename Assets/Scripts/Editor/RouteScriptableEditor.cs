using Alkemic.UAM;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Alkemic.Editors
{

    /*
    [CustomEditor(typeof(RouteScriptable))]
    public class RouteScriptableEditor : CategorizedEditor<RouteScriptableEditor.Category>
    {
        public enum Category
        {
            Normal = 0,
            EditRoute,
        }

        protected new RouteScriptable target => base.target as RouteScriptable;

        //[ShowIf("@EditingRoute != null")]

        private UAMSimulator simulator = null;

        private EditRoute editingRoute = null;
        protected EditRoute EditingRoute
        {
            set
            {
                if (editingRoute == value) return;
                if (editingRoute != null)
                {
                    ScriptableObject.DestroyImmediate(editingRoute);
                }
                editingRoute = value;
            }
            get => editingRoute;
        }

        private RouteData selectRouteData = null;

        public override void OnInspectorGUI()
        {
            using (var hori = new GUILayout.HorizontalScope())
            {




            }

            base.OnInspectorGUI();
        }

        private void ShowEditingRoute(EditRoute editingRoute)
        {
            using (var v = new GUILayout.VerticalScope(GUI.skin.box))
            {

                EditorGUILayout.LabelField("Editing Route");
                EditorGUI.indentLevel++;
                var e = Editor.CreateEditor(editingRoute);
                e.DrawDefaultInspector();
                using (var h = new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Confirm") == true)
                    {
                        //확인작업 수행
                        //this.selectRouteData.UpdateBy(editingRoute);
                        this.editingRoute = null;

                    }
                    if (GUILayout.Button("Cancel") == true)
                    {
                        this.editingRoute = null;
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

        protected override void OnCategoryInspector(Category category)
        {
            base.OnCategoryInspector(category);

            if(category == Category.EditRoute)
            {

                using (var verti = new GUILayout.VerticalScope(GUI.skin.box))
                {
                    this.simulator = (UAMSimulator)EditorGUILayout.ObjectField("Simulator", this.simulator, typeof(UAMSimulator));   
                }

                if (editingRoute != null)
                {
                    
                }

                foreach (var routeData in target.RouteDatas)
                {
                    using (var h = new GUILayout.HorizontalScope(GUI.skin.box))
                    {
                        EditorGUILayout.LabelField(routeData.Key);
                        if (this.simulator != null)
                        {
                            if (GUILayout.Button("Edit") == true)
                            {
                                editingRoute = EditRoute.CreateFrom(simulator, routeData);
                            }
                        }
                    }
                    //if(selectRouteData == this)

                    EditorGUI.indentLevel++;
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("VertiPort");
                    EditorGUILayout.LabelField(routeData.VertiPort);
                    GUILayout.EndHorizontal();
                    int index = 1;
                    foreach (var way in routeData.Ways)
                    {
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(index.ToString());
                        EditorGUILayout.LabelField(way);
                        GUILayout.EndHorizontal();
                        index++;
                    }
                    EditorGUI.indentLevel--;
                }
               
            }

        }

    }
    */
}
