using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LsSearch
{
    public class MaterialHelper : LsEditorBase
    {
        bool isShowEmpth = true;

        public override void OnGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("查看可能为空的材质", GUILayout.Height(BtnHeight)))
            {
                ResetGUISender();
                SearchEmpty();
                Sort();
                isShowEmpth = true;
            }

            if (GUILayout.Button("查看相同材质", GUILayout.Height(BtnHeight)))
            {
                ResetGUISender();
                SearchCompare();
                Sort();
                isShowEmpth = false;
            }
            GUILayout.EndHorizontal();
            if (isShowEmpth)
                SimpleShowInfo();
            else
                ShowCompareInfo();

        }

        public void ShowCompareInfo()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            foreach (var item in compareDict)
            {
                GUI.backgroundColor = Color.green;
                GUILayout.BeginHorizontal("PopupCurveSwatchBackground", GUILayout.Height(RowHegith));
                if (DrawShowBtn())
                    SelectionActive(item.Key);
                DrawItemLabel(item.Key);
                GUILayout.EndHorizontal();

                GUI.backgroundColor = Color.yellow;
                foreach (var compareItem in item.Value)
                {
                    GUILayout.BeginHorizontal("PopupCurveSwatchBackground", GUILayout.Height(RowHegith));
                    if (DrawShowBtn())
                        SelectionActive(compareItem);
                    DrawItemLabel(compareItem);
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
        }

        void SearchEmpty()
        {
            string[] allAssets = AssetDatabase.GetAllAssetPaths();

            int i = 0;
            foreach (string path in allAssets)
            {
                EditorUtility.DisplayProgressBar("查找中", path, (float)i / (float)allAssets.Length);
                if (path.EndsWith(".mat"))
                {
                    Material mat = AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material;
                    if (mat == null)
                    {
                        i++;
                        continue;
                    }

                    if (mat.mainTexture == null && GetFileReferences(path) == 0)
                    {
                        AddToShow(path);
                    }

                }
                i++;
            }
            EditorUtility.ClearProgressBar();
        }

        //Key 路径， Value 每个相同的材质的路径
        Dictionary<string, List<string>> compareDict = new Dictionary<string, List<string>>();
        void SearchCompare()
        {
            compareDict.Clear();

            //所有材质
            List<Material> allMat = GetAllMaterial();

            int i = 0;
            //用于与所有材质进行比较进使用，动态删除已经相同的
            List<Material> searchMats = new List<Material>(allMat);
            foreach (Material mat in allMat)
            {
                EditorUtility.DisplayProgressBar("查找中", mat.name, (float)i / (float)allMat.Count);
                if (searchMats.Contains(mat))
                {
                    searchMats.Remove(mat);
                    //临时比较列表
                    List<Material> tempList = new List<Material>(searchMats);
                    //存储相同的材质列表
                    List<string> compareList = new List<string>();
                    foreach (Material mat2 in tempList)
                    {
                        if (mat != mat2 && Compare(mat, mat2))
                        {
                            searchMats.Remove(mat2);
                            compareList.Add(AssetDatabase.GetAssetPath(mat2));
                        }
                    }

                    if (compareList.Count > 0)
                        compareDict.Add(AssetDatabase.GetAssetPath(mat), compareList);
                }

                i++;
            }
            EditorUtility.ClearProgressBar();
        }

        List<Material> GetAllMaterial()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            List<Material> list = new List<Material>();
            string[] allAssets = AssetDatabase.GetAllAssetPaths();
            foreach (string path in allAssets)
            {
                if (path.EndsWith(".mat"))
                {
                    Material mat = AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material;
                    list.Add(mat);
                }
            }
            return list;
        }

        bool Compare(Material m1, Material m2)
        {
            EditorSettings.serializationMode = SerializationMode.ForceText;
            string m1Path = AssetDatabase.GetAssetPath(m1);
            string m2Path = AssetDatabase.GetAssetPath(m2);

            if (!string.IsNullOrEmpty(m1Path) && !string.IsNullOrEmpty(m2Path))
            {
                string rootPath = Directory.GetCurrentDirectory();
                m1Path = Path.Combine(rootPath, m1Path);
                m2Path = Path.Combine(rootPath, m2Path);

                string text1 = File.ReadAllText(m1Path).Replace(" m_Name: " + m1.name, "");
                string text2 = File.ReadAllText(m2Path).Replace(" m_Name: " + m2.name, "");
                return (text1 == text2);
            }
            return false;
        }

    }
}