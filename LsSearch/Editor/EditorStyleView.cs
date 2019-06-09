using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace LsSearch
{
    public class EditorStyleView : LsEditorBase
    {
        private string search = string.Empty;
        public override void OnGUI()
        {
            GUILayout.BeginHorizontal("HelpBox");
            GUILayout.Label("单击示例将复制其名到剪贴板", "label");
            GUILayout.FlexibleSpace();
            GUILayout.Label("查找:");
            search = EditorGUILayout.TextField(search);
            GUILayout.EndHorizontal();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            foreach (GUIStyle style in GUI.skin)
            {
                if (style.name.ToLower().Contains(search.ToLower()))
                {
                    GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
                    GUILayout.Space(7);
                    if (GUILayout.Button(style.name, style))
                    {
                        EditorGUIUtility.systemCopyBuffer = "\"" + style.name + "\"";
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.SelectableLabel("\"" + style.name + "\"");
                    GUILayout.EndHorizontal();
                    GUILayout.Space(11);
                }
            }

            GUILayout.EndScrollView();
        }
    }
}