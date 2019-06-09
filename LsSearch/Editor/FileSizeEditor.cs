using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.ComponentModel;

namespace LsSearch
{
    public class FileSizeEditor : LsEditorBase
    {
        /// <summary>
        /// 文件结构体
        /// </summary>
        public struct FileAttribute
        {
            public string path;     //路径
            public long size;       //文件大小
            public string sizeStr;  //转成MB后的显示值
        }

        private List<FileAttribute> fileList = new List<FileAttribute>();
        private List<FileAttribute> fbxSortList = new List<FileAttribute>();

        Vector2 scrollPos = Vector2.zero;

        public override void OnGUI()
        {
            if (GUILayout.Button("查  找", GUILayout.Height(BtnHeight)))
            {
                SearchParticle();
            }

            int index = 0;
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            foreach (var item in fbxSortList)
            {
                index++;
                if ((index & 1) == 1)
                    GUI.backgroundColor = Color.green;
                else
                    GUI.backgroundColor = Color.yellow;

                GUILayout.BeginHorizontal("PopupCurveSwatchBackground", GUILayout.Height(RowHegith));
                if (DrawShowBtn())
                    SelectionActive(item.path);

                DrawItemLabel(item.sizeStr);
                DrawLabel(item.path);
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();
        }

        /// <summary>
        /// 搜索文件
        /// </summary>
        void SearchParticle()
        {
            fileList.Clear();
            fbxSortList.Clear();
            var allObjects = AssetDatabase.GetAllAssetPaths();
            int i = 0;
            foreach (var path in allObjects)
            {
                EditorUtility.DisplayProgressBar("查找中", path, (float)i / (float)allObjects.Length);
                System.IO.FileInfo file = new System.IO.FileInfo(path);
                if (file != null && file.Exists)
                {
                    FileAttribute fileAttr;
                    fileAttr.path = path;
                    fileAttr.size = file.Length;

                    float tempFileLength = 0;
                    //转成MB
                    if (file.Length > 1024)
                        tempFileLength = file.Length / 1024.0f / 1024.0f;

                    if (tempFileLength > 0)
                    {
                        int strPos = tempFileLength.ToString().IndexOf(".");
                        fileAttr.sizeStr = tempFileLength.ToString().Substring(0, strPos + 3) + " MB";
                    }
                    else
                    {
                        fileAttr.sizeStr = "0.00 MB";
                    }


                    fileList.Add(fileAttr);
                }
                i++;
            }

            fbxSortList = new List<FileAttribute>(fileList);
            fbxSortList.Sort(SortFileAttrCompare);
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// 排序规则
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        private int SortFileAttrCompare(FileAttribute obj1, FileAttribute obj2)
        {
            int res = 0;
            if (obj1.size > obj2.size)
            {
                res = -1;
            }
            else if (obj1.size < obj2.size)
            {
                res = 1;
            }
            return res;
        }
    }
}