using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LsSearch
{
    public class LsEditorBase
    {
        public static float BtnHeight = 30.0f;
        public static float BtnWidth = 150.0f;

        public static float RowHegith = 20.0f;
        public static float RowWidth = 100;
        public static float ItemBtnWidth = 50.0f;

        public List<string> ShowArr = new List<string>();
        public Vector2 scrollPosition = Vector2.zero;

        public enum ComponentEnum
        {
            Animation = 0,
            Animator,
            AudioListener,
            AudioSource,
            BoxCollider,
            BoxCollider2D,
            Camera,
            Canvas,
            Collider2D,
            Collider,
            Font,
            Joint2D,
            Joint,
            Light,
            Material,
            Mesh,
            MeshCollider,
            MeshRenderer,
            ParticleEmitter,
            ParticleSystem,
            Renderer,
            Rigidbody,
            Rigidbody2D,
            Skybox,
            SpriteRenderer,
            Terrain,
        }

        public virtual void OnGUI()
        {

        }

        #region GetType
        public Type GetType(string TypeName)
        {
            Type type = Type.GetType(TypeName);

            if (type == null)
                type = GetExecutingType(TypeName);

            if (type == null)
                type = GetUnityType(TypeName);

            if (type == null)
                type = Types.GetType(TypeName, "Assembly-CSharp");

            if (type == null)
                type = Types.GetType(TypeName, "Assembly-CSharp-Editor");

            return type;
        }

        //从当前执行的程序集中获取Type
        Type GetExecutingType(string TypeName)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();
            return assembly.GetType(TypeName);
        }

        //从UnityEngin中获取
        Type GetUnityType(string TypeName)
        {
            string namespaceStr = "UnityEngine";
            if (!TypeName.Contains(namespaceStr))
                TypeName = namespaceStr + "." + TypeName;

            return Types.GetType(TypeName, namespaceStr);
        }
        #endregion

        /// <summary>
        /// 得到物体的树结构
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string GetCompleteTree(GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }

        /// <summary>
        /// 添加到GUI显示
        /// </summary>
        /// <param name="obj"></param>
        public void AddToShow(UnityEngine.Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            AddToShow(path);
        }
        public void AddToShow(string str)
        {
            if (!ShowArr.Contains(str))
                ShowArr.Add(str);
        }

        public void ResetGUISender()
        {
            scrollPosition = Vector2.zero;
            ShowArr.Clear();
        }

        public void Sort()
        {
            ShowArr.Sort();
        }

        /// <summary>
        /// 通用显示搜索列表
        /// </summary>
        public void SimpleShowInfo()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            int index = 0;
            foreach (string item in ShowArr)
            {
                index++;
                if ((index & 1) == 1)
                    GUI.backgroundColor = Color.green;
                else
                    GUI.backgroundColor = Color.yellow;

                GUILayout.BeginHorizontal("PopupCurveSwatchBackground", GUILayout.Height(RowHegith));
                if (DrawShowBtn())
                    SelectionActive(item);

                DrawLabel(item);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }

        public void SelectionActive(string path)
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object)) as UnityEngine.Object;
            SelectionActive(obj);
        }

        public void SelectionActive(UnityEngine.Object obj)
        {
            if (obj)
            {
                Selection.activeObject = obj;
                //把名字存储在剪粘板
                EditorGUIUtility.systemCopyBuffer = AssetDatabase.GetAssetPath(obj);
            }

        }

        public int GetFileReferences(string path)
        {
            EditorSettings.serializationMode = SerializationMode.ForceText;
            if (!string.IsNullOrEmpty(path))
            {
                string guid = AssetDatabase.AssetPathToGUID(path);
                string withoutExtensions = "*.prefab*.unity*.mat*.asset";
                string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories)
                    .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
                int referencesCount = 0;


                for (int i = 0; i < files.Length; i++)
                {
                    string file = files[i];
                    if (System.Text.RegularExpressions.Regex.IsMatch(File.ReadAllText(file), guid))
                    {
                        referencesCount++;
                    }
                }
                return referencesCount;
            }
            return 0;
        }

        private string GetRelativeAssetsPath(string path)
        {
            return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
        }

        #region DrawHelper

        public bool DrawShowBtn()
        {
            return DrawItemBtn("Show");
        }

        public bool DrawItemBtn(string name)
        {
            return GUILayout.Button(name, GUILayout.Width(ItemBtnWidth), GUILayout.Height(RowHegith));
        }

        public void DrawItemTitle(string text)
        {
            GUILayout.Label(text, GUI.skin.textField, GUILayout.Width(RowWidth), GUILayout.Height(RowHegith));
        }

        public void DrawLabel(string text)
        {
            DrawLabel(text, GUILayout.Height(RowHegith));
        }

        public void DrawLabel(string text, params GUILayoutOption[] option)
        {
            GUILayout.Label(text, option);
        }

        public void DrawItemLabel(string text)
        {
            DrawItemLabel(text, GUI.skin.textArea);
        }

        public void DrawItemLabel(string text, GUIStyle style)
        {
            DrawItemLabel(text, style, GUILayout.Width(RowWidth), GUILayout.Height(RowHegith));
        }
        public void DrawItemLabel(string text, params GUILayoutOption[] options)
        {
            DrawItemLabel(text, GUI.skin.textArea, options);
        }

        public void DrawItemLabel(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.Label(text, style, options);
        }

        #endregion
    }
}
