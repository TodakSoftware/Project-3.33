 using UnityEngine;
 using UnityEditor;
 using System.Collections.Generic;
 using System.IO;
public class MaterialsUpdate : EditorWindow
     {
         [MenuItem("Tools/Shader Occurence and Update")]
         public static void Open() { GetWindow<MaterialsUpdate>(); }
         Shader shader;
         Shader shaderChangeTo;
         List<string> materials = new List<string>();
         Vector2 scroll;
         void UpdateScrollView()
         {
             // Find materials by shader type selected by user in field "Shader to find"
             string shaderPath = AssetDatabase.GetAssetPath(shader);
             string[] allMaterials = AssetDatabase.FindAssets("t:Material");
             materials.Clear();
             for (int i = 0; i < allMaterials.Length; i++)
             {
                 allMaterials[i] = AssetDatabase.GUIDToAssetPath(allMaterials[i]);
                 var material = AssetDatabase.LoadAssetAtPath<Material>(allMaterials[i]);
                 if (material.shader == shader) materials.Add(allMaterials[i]);
             }
             // Put materials paths to scroll view
             scroll = GUILayout.BeginScrollView(scroll);
             {
                 for (int i = 0; i < materials.Count; i++)
                 {
                     GUILayout.BeginHorizontal();
                     {
                         GUILayout.Label(Path.GetFileNameWithoutExtension(materials[i]));
                         GUILayout.FlexibleSpace();
                         if (GUILayout.Button("Show"))
                             EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(materials[i], typeof(Material)));
                     }
                     GUILayout.EndHorizontal();
                 }
             }
             GUILayout.EndScrollView();
         }
         void OnGUI()
         {
             Shader prev = shader;
             GUILayout.Label("Shader to find");
             shader = EditorGUILayout.ObjectField(shader, typeof(Shader), false) as Shader;
             GUILayout.Label("Shader change to");
             shaderChangeTo = EditorGUILayout.ObjectField(shaderChangeTo, typeof(Shader), false) as Shader;
 
             if (shader != prev)
                 UpdateScrollView();
 
             // Shange material shader to selected by user in field "Shader change to"
             if (GUILayout.Button("Change All"))
             {
                 if (shaderChangeTo && shaderChangeTo != shader)
                 {
                     materials.ForEach(delegate (string material)
                    {
                        Material materialAsset = AssetDatabase.LoadAssetAtPath<Material>(material);
                        materialAsset.shader = shaderChangeTo;
                    });
 
                     UpdateScrollView();
                 }
                 else
                 {
                     Debug.Log("WARNING: MaterialsUpdate - No valid shader change to");
                 }
             }
         }
 }