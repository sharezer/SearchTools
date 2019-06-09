using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace LsSearch
{
    public class FbxInfoEditor : LsEditorBase
    {
        public struct FbxAttribute
        {
            public string Name;
            public int vertexCount;
            public int trianglesCount;
            public string path;
        }
        private int faxMaxVerOrTriCount = 3000;
        private List<FbxAttribute> fbxOriginalList = new List<FbxAttribute>();
        private List<FbxAttribute> fbxSortList = new List<FbxAttribute>();

        public override void OnGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("查看选中对象面数（可多选）", GUILayout.Height(BtnHeight)))
            {
                SelectionFbxInfo();
            }

            if (GUILayout.Button("查看所有资源面数", GUILayout.Height(BtnHeight)))
            {
                AllFbxInfo();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("HelpBox", GUILayout.Height(RowHegith));
            GUILayout.Space(ItemBtnWidth + 2);
            DrawItemTitle("名称");
            DrawItemTitle("顶点数");
            DrawItemTitle("面数");
            DrawItemTitle("路径");

            isUseSort = GUILayout.Toggle(isUseSort, "排序(面数)", GUILayout.Width(RowWidth), GUILayout.Height(RowHegith));
            List<FbxAttribute> fbxList = isUseSort ? fbxSortList : fbxOriginalList;
            GUILayout.EndHorizontal();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            int index = 0;
            GUIStyle fbxAttrStyle = new GUIStyle(GUI.skin.textArea);
            fbxAttrStyle.normal.textColor = Color.red;
            foreach (FbxAttribute attr in fbxList)
            {
                index++;
                if((index & 1) == 1)
                    GUI.backgroundColor = Color.green;
                else
                    GUI.backgroundColor = Color.yellow;

                GUIStyle tempStyle;

                if ((attr.vertexCount > faxMaxVerOrTriCount) || (attr.trianglesCount > faxMaxVerOrTriCount))
                    tempStyle = fbxAttrStyle;
                else
                    tempStyle = GUI.skin.textArea;

                GUILayout.BeginHorizontal("PopupCurveSwatchBackground", GUILayout.Height(RowHegith));
                if (DrawShowBtn())
                {
                    SelectionActive(attr.path);
                }

                DrawItemLabel(attr.Name, GUI.skin.textArea);
                DrawItemLabel(attr.vertexCount.ToString(), tempStyle);
                DrawItemLabel(attr.trianglesCount.ToString(), tempStyle);
                DrawLabel(attr.path);

                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }

        bool isUseSort = false;
        //所有Fbx资源信息
        private void AllFbxInfo()
        {
            fbxOriginalList.Clear();
            fbxSortList.Clear();
            string[] allAssets = AssetDatabase.GetAllAssetPaths();
            int i = 0;
            foreach (string s in allAssets)
            {
                EditorUtility.DisplayProgressBar("查找中", s, (float)i / (float)allAssets.Length);
                if (s.EndsWith(".fbx") || s.EndsWith(".FBX"))
                {
                    GameObject g = AssetDatabase.LoadAssetAtPath(s, typeof(GameObject)) as GameObject;
                    GetFbxInfo(g);
                }
                i++;
            }
            fbxSortList = new List<FbxAttribute>(fbxOriginalList);
            fbxSortList.Sort(SortFbxAttrCompare);
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// 选中的Fbx资源信息
        /// </summary>
        private void SelectionFbxInfo()
        {
            fbxOriginalList.Clear();
            fbxSortList.Clear();

            GameObject[] go = Selection.gameObjects;

            int i = 0;
            foreach (GameObject g in go)
            {
                EditorUtility.DisplayProgressBar("查找中", g.name, (float)i / (float)go.Length);
                GetFbxInfo(g);
            }
            EditorUtility.ClearProgressBar();

            fbxSortList = new List<FbxAttribute>(fbxOriginalList);
            fbxSortList.Sort(SortFbxAttrCompare);
        }

        /// <summary>
        /// 由大到小排序
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        private int SortFbxAttrCompare(FbxAttribute obj1, FbxAttribute obj2)
        {
            int res = 0;
            if (obj1.trianglesCount > obj2.trianglesCount)
            {
                res = -1;
            }
            else if (obj1.trianglesCount < obj2.trianglesCount)
            {
                res = 1;
            }
            return res;
        }

        /// <summary>
        /// 获取Fbx文件信息
        /// </summary>
        /// <param name="g"></param>
        private void GetFbxInfo(GameObject g)
        {
            if (g == null)
                return;
            int vertexCount = 0;
            int trianglesCount = 0;

            MeshFilter[] mfs = g.GetComponentsInChildren<MeshFilter>(true);
            SkinnedMeshRenderer[] smrs = g.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (MeshFilter child in mfs)
            {
                if (child.sharedMesh != null)
                {
                    vertexCount += child.sharedMesh.vertexCount;
                    trianglesCount += child.sharedMesh.triangles.Length / 3;
                }
                else if (child.mesh != null)
                {
                    vertexCount += child.mesh.vertexCount;
                    trianglesCount += child.mesh.triangles.Length / 3;
                }
            }
            foreach (SkinnedMeshRenderer child in smrs)
            {
                if (child.sharedMesh != null)
                {
                    vertexCount += child.sharedMesh.vertexCount;
                    trianglesCount += child.sharedMesh.triangles.Length / 3;
                }
            }

            FbxAttribute fbxAttr;
            fbxAttr.Name = g.name;
            fbxAttr.vertexCount = vertexCount;
            fbxAttr.trianglesCount = trianglesCount;
            fbxAttr.path = AssetDatabase.GetAssetPath(g);

            fbxOriginalList.Add(fbxAttr);
        }
    }
}
