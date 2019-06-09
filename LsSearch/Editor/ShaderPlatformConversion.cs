using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace LsSearch
{
    public class ShaderPlatformConversion : LsEditorBase
    {

        public override void OnGUI()
        {
            if (GUILayout.Button("转为手机适用Shader", GUILayout.Height(BtnHeight)))
            {
                ResetGUISender();
                SearchAndChangeShader(true);
                Sort();
            }

            if (GUILayout.Button("转为PC适用Shader", GUILayout.Height(BtnHeight)))
            {
                ResetGUISender();
                SearchAndChangeShader(false);
                Sort();
            }
            SimpleShowInfo();
        }

        Dictionary<string, string> shaderDic = new Dictionary<string, string>{
    {"Diffuse", "Mobile/Diffuse"},
    {"Bumped Difffuse", "Mobile/Bumped Difffuse"},
    {"Bumped Specular", "Mobile/Bumped Specular"},
    {"Particles/Additive", "Mobile/Particles/Additive"},
    {"Particles/Alpha Blended", "Mobile/Particles/Alpha Blended"},
    {"Particles/Multiply", "Mobile/Particles/Multiply"},
    {"Particles/VertexLit Blended", "Mobile/Particles/VertexLit Blended"},
    {"VertexLit", "Mobile/VertexLit"},
    };

        private void SearchAndChangeShader(bool isMobile)
        {
            string[] allAssets = AssetDatabase.GetAllAssetPaths();

            int i = 0;
            foreach (string s in allAssets)
            {
                EditorUtility.DisplayProgressBar("查找中", s, (float)i / (float)allAssets.Length);
                if (s.EndsWith(".prefab") || s.EndsWith(".fbx") || s.EndsWith(".FBX"))
                {//不考虑fbx|| s.EndsWith(".fbx") || s.EndsWith(".FBX")
                    GameObject g = AssetDatabase.LoadAssetAtPath(s, typeof(GameObject)) as GameObject;
                    if (g == null)
                    {
                        i++;
                        continue;
                    }

                    Renderer[] renderers = g.GetComponentsInChildren<Renderer>(true);
                    if (renderers == null)
                    {
                        i++;
                        continue;
                    }

                    foreach (Renderer r in renderers)
                    {
                        ChangeShader(r, isMobile);
                    }
                }
                i++;
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void ChangeShader(Renderer renderer, bool isMobile)
        {
            if (renderer.sharedMaterial != null)
            {
                if (isMobile)
                {
                    if (shaderDic.ContainsKey(renderer.sharedMaterial.shader.name))
                    {
                        renderer.sharedMaterial.shader = Shader.Find(shaderDic[renderer.sharedMaterial.shader.name]);
                        AddToShow(renderer);
                    }
                }
                else if (shaderDic.ContainsValue(renderer.sharedMaterial.shader.name))
                {
                    string key = shaderDic.FirstOrDefault(q => q.Value == renderer.sharedMaterial.shader.name).Key;
                    renderer.sharedMaterial.shader = Shader.Find(key);
                    AddToShow(renderer);
                }
            }
            //else if (renderer.material != null) {
            //    if (isMobile) {
            //        if (shaderDic.ContainsKey(renderer.material.shader.name)) {
            //            renderer.material.shader = Shader.Find(shaderDic[renderer.material.shader.name]);
            //            AddToShow(renderer);
            //        }
            //    } else if (shaderDic.ContainsValue(renderer.material.shader.name)) {
            //        string key = shaderDic.FirstOrDefault(q => q.Value == renderer.material.shader.name).Key;
            //        renderer.material.shader = Shader.Find(key);
            //        AddToShow(renderer);
            //    }
            //}
        }

    }

}
