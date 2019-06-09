using UnityEngine;
using System.Collections;
using UnityEditor;

namespace LsSearch
{
    public class EmptyPrefabEditor : LsEditorBase
    {

        public override void OnGUI()
        {
            if (GUILayout.Button("查  找", GUILayout.Height(BtnHeight)))
            {
                ResetGUISender();
                SeachEmptyPrefab();
                Sort();
            }
            SimpleShowInfo();
        }

        private void SeachEmptyPrefab()
        {
            string[] allAssets = AssetDatabase.GetAllAssetPaths();
            int i = 0;
            foreach (string s in allAssets)
            {
                EditorUtility.DisplayProgressBar("查找中", s, (float)i / (float)allAssets.Length);
                if (s.EndsWith(".prefab"))
                {
                    GameObject g = AssetDatabase.LoadAssetAtPath(s, typeof(GameObject)) as GameObject;
                    if (g == null)
                    {
                        AddToShow(s);
                    }
                }
                i++;
            }
            EditorUtility.ClearProgressBar();

            if (ShowArr.Count > 0 && EditorUtility.DisplayDialog("提示", "找到" + ShowArr.Count + "个空预置体，是否删除？！", "删除", "取消"))
            {
                foreach (string path in ShowArr)
                {
                    AssetDatabase.DeleteAsset(path);
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}
