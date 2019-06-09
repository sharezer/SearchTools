using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LsSearch
{
    class FindReferences : LsEditorBase
    {
        Vector2 scrollPos = Vector2.zero;

        public override void OnGUI()
        {
            if (GUILayout.Button("查找选中资源引用", GUILayout.Height(BtnHeight)))
            {
                ResetGUISender();
                Find();
                Sort();
            }
            SimpleShowInfo();
        }

        void Find()
        {
            EditorSettings.serializationMode = SerializationMode.ForceText;
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!string.IsNullOrEmpty(path))
            {
                string guid = AssetDatabase.AssetPathToGUID(path);
                string withoutExtensions = "*.prefab*.unity*.mat*.asset";
                string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories)
                    .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
                int startIndex = 0;

                EditorApplication.update = delegate ()
                {
                    string file = files[startIndex];

                    bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)files.Length);

                    if (System.Text.RegularExpressions.Regex.IsMatch(File.ReadAllText(file), guid))
                    {
                        AddToShow(GetRelativeAssetsPath(file));
                    }

                    startIndex++;
                    if (isCancel || startIndex >= files.Length)
                    {
                        EditorUtility.ClearProgressBar();
                        EditorApplication.update = null;
                        startIndex = 0;
                        Debug.Log("匹配结束");
                    }

                };
            }
        }

        private string GetRelativeAssetsPath(string path)
        {
            return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
        }

    }
}
