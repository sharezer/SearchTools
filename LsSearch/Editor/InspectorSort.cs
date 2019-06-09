using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace LsSearch
{
    public class InspectorSort : LsEditorBase
    {
        List<string> componentList = new List<string>();
        GameObject target;

        public override void OnGUI()
        {

            if (GUILayout.Button("开  始", GUILayout.Height(BtnHeight)))
            {
                RefreshComponentList(Selection.activeGameObject);
            }

            if (target != null)
                GUILayout.Label("Target: " + AssetDatabase.GetAssetPath(target), GUILayout.Height(RowHegith));


            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            DrawComponentList();
            EditorGUILayout.EndScrollView();
        }

        private void DrawComponentList()
        {
            for(int i = 1; i < componentList.Count; i++)
            {
                if ((i & 1) == 1)
                    GUI.backgroundColor = Color.green;
                else
                    GUI.backgroundColor = Color.yellow;

                GUILayout.BeginHorizontal("PopupCurveSwatchBackground", GUILayout.Height(RowHegith));
                DrawLabel(componentList[i]);
                if(DrawItemBtn("Up"))
                {
                    MoveComponent(i, i - 1);
                    RefreshComponentList(target);
                }
                if (DrawItemBtn("Down"))
                {
                    MoveComponent(i, i + 1);
                    RefreshComponentList(target);
                }
                if (DrawItemBtn("Top"))
                {
                    MoveComponent(i, 1);
                    RefreshComponentList(target);
                }
                if (DrawItemBtn("Buttom"))
                {
                    MoveComponent(i, componentList.Count);
                    RefreshComponentList(target);
                }
                GUILayout.EndHorizontal();
            }
        }


        public void RefreshComponentList(GameObject go)
        {
            if (go == null)
                return;
            target = go;
            componentList.Clear();

            SerializedObject serializedObject = new SerializedObject(target);
            var components = target.GetComponents<Component>();
            
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] != null)
                {
                    componentList.Add(components[i].GetType().ToString());
                }
            } 
        }

        public void MoveComponent(int from, int to)
        {
            if (from == to)
                return;

            to = Mathf.Clamp(to, 1, componentList.Count - 1);
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty prop = serializedObject.FindProperty("m_Component");
            prop.MoveArrayElement(from, to);
            serializedObject.ApplyModifiedProperties();
        }

    }
}
