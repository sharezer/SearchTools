using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace LsSearch
{
    public class EmptyAnim : LsEditorBase
    {
        public override void OnGUI()
        {
            if (GUILayout.Button("查  找", GUILayout.Height(BtnHeight)))
            {
                ResetGUISender();
                SearchEmptyAnim();
                Sort();
            }
            SimpleShowInfo();
        }

        private void SearchEmptyAnim()
        {
            List<Animation> deleteList = new List<Animation>();
            deleteList.Clear();
            string[] allAssets = AssetDatabase.GetAllAssetPaths();

            int i = 0;
            foreach (string s in allAssets)
            {
                EditorUtility.DisplayProgressBar("查找中", s, (float)i / (float)allAssets.Length);
                if (s.EndsWith(".prefab"))
                {//不考虑fbx|| s.EndsWith(".fbx") || s.EndsWith(".FBX")
                    GameObject g = AssetDatabase.LoadAssetAtPath(s, typeof(GameObject)) as GameObject;
                    if (g == null)
                    {
                        Debug.Log("empty gameObject: ", g);
                        i++;
                        continue;
                    }

                    Animation[] anims = g.GetComponentsInChildren<Animation>(true);
                    if (anims == null)
                    {
                        i++;
                        continue;
                    }

                    //删除没有用的动画组件
                    foreach (Animation animation in anims)
                    {
                        if (animation.clip == null)
                        {
                            AddToShow(s);
                            deleteList.Add(animation);
                            //DestroyImmediate(animation, true);
                        }
                    }
                }
                i++;
            }
            EditorUtility.ClearProgressBar();


            if (deleteList.Count > 0 && EditorUtility.DisplayDialog("提示", "找到" + deleteList.Count + "个空动画，是否删除？！", "删除", "取消"))
            {
                foreach (Animation animation in deleteList)
                {
                    //TODO : Destory
                    //DestroyImmediate(animation, true);
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}