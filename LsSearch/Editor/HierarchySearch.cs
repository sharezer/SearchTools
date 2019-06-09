using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LsSearch
{
    public class HierarchySearch : LsEditorBase
    {
        string search = string.Empty;
        ComponentEnum selectComponent;
        Dictionary<string, GameObject> showDict = new Dictionary<string, GameObject>();

        //false 使用方法1
        //true 使用方法2
        bool ToggleBool = false;

        public override void OnGUI()
        {
            GUILayout.BeginVertical(GUILayout.Height(BtnHeight));
            //方法1 通过选择框查找
            GUILayout.BeginHorizontal();
            ToggleBool = !EditorGUILayout.Toggle(!ToggleBool, GUILayout.Width(ItemBtnWidth));
            GUI.enabled = !ToggleBool;
            selectComponent = (ComponentEnum)EditorGUILayout.EnumPopup("搜索类型:", selectComponent);
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            //方法2 手动输入类型来查找
            GUILayout.BeginHorizontal();
            ToggleBool = EditorGUILayout.Toggle(ToggleBool, GUILayout.Width(ItemBtnWidth));
            GUI.enabled = ToggleBool;
            GUILayout.Label("查找:");
            search = EditorGUILayout.TextField(search);
            GUI.enabled = true;

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (GUILayout.Button("查  找", GUILayout.Height(BtnHeight)))
            {
                showDict.Clear();

                string typeName = string.Empty;
                if(!ToggleBool) //方法1
                    typeName = selectComponent.ToString();
                else //方法2
                {
                    if (string.IsNullOrEmpty(search))
                        return;
                    typeName = search;
                }

                Debug.Log(typeName);

                Type type = GetType(typeName);
                if (type != null)
                    SearchHierachy(type);
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            int index = 0;
            foreach (string item in showDict.Keys)
            {
                index++;
                if ((index & 1) == 1)
                    GUI.backgroundColor = Color.green;
                else
                    GUI.backgroundColor = Color.yellow;

                GUILayout.BeginHorizontal("PopupCurveSwatchBackground", GUILayout.Height(RowHegith));
                if (DrawShowBtn())
                    SelectionActive(showDict[item]);

                DrawLabel(item);
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        void SearchHierachy(Type t)
        {
            var allObjects = SearchTools.FindObjects(t);
            int i = 0;
            foreach (Component item in allObjects)
            {
                string path = GetCompleteTree(item.gameObject);
                EditorUtility.DisplayProgressBar("查找中", path, (float)i / (float)allObjects.Length);

                if (item.gameObject && !showDict.ContainsKey(path))
                {
                    showDict.Add(path, item.gameObject);
                }
                i++;
            }
            EditorUtility.ClearProgressBar();
        }
    }
}
