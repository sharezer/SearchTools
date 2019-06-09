using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace LsSearch
{
    public class MissingScriptEditor : LsEditorBase
    {

        public override void OnGUI()
        {
            if (GUILayout.Button("查  找", GUILayout.Height(BtnHeight)))
            {
                ResetGUISender();
                FindMissSript();
                Sort();
            }
            SimpleShowInfo();

            if(delList.Count > 0 && GUILayout.Button("删  除", GUILayout.Height(BtnHeight)))
            {
                DeleteMissing();
            }

        }

        List<GameObject> delList = new List<GameObject>();
        private void FindMissSript()
        {
            string[] allAssets = AssetDatabase.GetAllAssetPaths();

            int i = 0;
            foreach (string s in allAssets)
            {
                EditorUtility.DisplayProgressBar("查找中", s, i / (float)allAssets.Length);
                if (s.EndsWith(".prefab"))
                {
                    GameObject g = AssetDatabase.LoadAssetAtPath(s, typeof(GameObject)) as GameObject;
                    if (g == null)
                    {
                        Debug.Log("empty gameObject: ", g);
                        i++;
                        continue;
                    }

                    Component[] components = g.GetComponentsInChildren<MonoBehaviour>(true);
                    foreach (Component child in components)
                    {
                        if (child == null)
                        {
                            string str = AssetDatabase.GetAssetPath(g);
                            AddToShow(str);
                            //if (isDelete && !delList.Contains(g))
                            if (!delList.Contains(g))
                                delList.Add(g);
                        }

                    }
                }
                i++;
            }
            EditorUtility.ClearProgressBar();
            Selection.objects = delList.ToArray();
        }

        void DeleteMissing()
        {
            int i = 0;
            foreach (GameObject item in delList)
            {
                ClearMissComponent(item);
                //CleanMissingScript(item);
                EditorUtility.DisplayProgressBar("删除中", item.name, (float)i / (float)delList.Count);
                i++;
            }
            EditorUtility.ClearProgressBar();
        }

        private void ClearMissComponent(GameObject go)
        {
            if (go == null)
            {
                return;
            }
            var components = go.GetComponents<Component>();

            SerializedObject serializedObject = new SerializedObject(go);
            SerializedProperty prop = serializedObject.FindProperty("m_Component");

            int r = 0;
            for (int j = 0; j < components.Length; j++)
            {
                if (components[j] == null)
                {
                    prop.DeleteArrayElementAtIndex(j - r);
                    r++;
                    Debug.Log("移除丢失组件: " + go.name, go);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}