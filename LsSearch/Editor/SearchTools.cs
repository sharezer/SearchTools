using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LsSearch
{
    public class SearchTools : EditorWindow
    {
        static Vector2 windowSize = new Vector2(800.0f, 600.0f);

        [MenuItem("Tools/LsTools %T")]
        public static void ShowWindow()
        {

            SearchTools window = EditorWindow.GetWindow<SearchTools>(true);
            window.minSize = windowSize;
            window.maxSize = windowSize;
            window.titleContent = new GUIContent("Ls工具箱");
            window.Init();
        }

        float toolsWidth = 160.0f;

        Vector2 toolsSVPos = Vector2.zero;
        Vector2 subEditorSVPos = Vector2.zero;
        public LsEditorBase subEditor = null;
        private int clickBtnIndex = 0;

        Dictionary<string, Type> toolsDict = new Dictionary<string, Type>{
            {"模型信息", typeof(FbxInfoEditor)},
            {"脚本丢失", typeof(MissingScriptEditor)},
            {"空Animation", typeof(EmptyAnim)},
            {"空预置体", typeof(EmptyPrefabEditor)},
            {"空材质", typeof(MaterialHelper)},
            {"Shader平台转换", typeof(ShaderPlatformConversion)},
            {"Hierarchy中的组件", typeof(HierarchySearch)},
            {"组件排序",typeof(InspectorSort) },
            {"文件大小", typeof(FileSizeEditor)},
            {"资源引用", typeof(FindReferences)},
            {"GUI样式", typeof(EditorStyleView)},

        };


        void Init()
        {
            isRegister = true;
            ClickBtnByIndex(0);

            //if (RegisterHelper.isRegister())
            //{
            //    isRegister = true;
            //    ClickBtnByIndex(0);
            //}
            //else
            //    EditorUtility.DisplayDialog("提示", "工具未注册", "确定");

        }

        private bool isRegister = false;

        void OnGUI()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log("A");
            }
            if (isRegister)
            {
                GUILayout.Space(5);

                GUIStyle bgStyle = new GUIStyle(GUI.skin.box);
                bgStyle.padding = new RectOffset();
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(bgStyle, GUILayout.ExpandHeight(true));
                ShowTools();
                GUILayout.EndVertical();
                GUILayout.BeginVertical(bgStyle);
                if (subEditor != null)
                    subEditor.OnGUI();

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            SetKeyEvent();

        }

        string keyValue;
        /// <summary> 设置快捷操作 </summary>
        void SetKeyEvent()
        {
            Event e = Event.current;
            if (e.isKey)
            {
                if (e.type == EventType.keyUp)
                {
                    if (e.keyCode == KeyCode.Space)
                        keyValue = string.Empty;
                    else
                    keyValue += e.keyCode;

                    if(keyValue.Equals("SHAREZER"))
                    {
                        RegisterHelper.DoRegister();
                        ShowWindow();
                    }
                }
                else if (e.type == EventType.keyDown)
                {

                }
            }
        }

        void ShowTools()
        {
            int index = 0;
            foreach (string btnName in toolsDict.Keys)
            {
                if (index == clickBtnIndex)
                    GUI.backgroundColor = Color.green;

                if (IsClickBtn(btnName))
                {
                    ClickBtnByIndex(index);
                }
                GUI.backgroundColor = Color.white;

                index++;
            }

            if (IsClickBtn("关闭"))
            {
                Close();
            }
        }

        bool IsClickBtn(string btnName)
        {
            return GUILayout.Button(btnName, GUILayout.Height(LsEditorBase.BtnHeight), GUILayout.Width(LsEditorBase.BtnWidth));
        }

        void ClickBtnByIndex(int index)
        {
            int i = 0;
            foreach (string btnName in toolsDict.Keys)
            {
                if (i == index)
                {
                    Type type = toolsDict[btnName];
                    subEditor = Activator.CreateInstance(type) as LsEditorBase;
                    clickBtnIndex = index;
                    break;
                }
                i++;
            }
        }

        public static UnityEngine.Object[] FindObjects(Type t)
        {
            return FindObjectsOfType(t);
        }

        public static void DoDestory(UnityEngine.Object obj)
        {
            DestroyImmediate(obj, true);
        }
    }
}
