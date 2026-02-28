using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class FixSpriteSkinGPUShader
{
    [MenuItem("Tools/2D Animation/Fix SpriteSkin GPU Shader")]
    public static void ReplaceCustomShader()
    {
        string customShaderName = "Custom/URP_Sprite_3D_Lit_Final";
        Shader custom = Shader.Find(customShaderName);
        if (custom == null)
        {
            Debug.LogWarning($"Custom shader '{customShaderName}' not found in project. Aborting.");
            return;
        }

        // Prefer URP 2D Sprite-Lit shader, fallback to Sprite/Default
        Shader target = Shader.Find("Universal Render Pipeline/2D/Sprite-Lit");
        if (target == null)
            target = Shader.Find("Sprites/Default");

        if (target == null)
        {
            Debug.LogError("Couldn't find a suitable target sprite shader (URP 2D Sprite-Lit or Sprites/Default).");
            return;
        }

        string[] materialGuids = AssetDatabase.FindAssets("t:Material");
        var modified = new List<string>();

        for (int i = 0; i < materialGuids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(materialGuids[i]);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) continue;
            if (mat.shader == custom)
            {
                Undo.RecordObject(mat, "Replace sprite shader");
                mat.shader = target;
                EditorUtility.SetDirty(mat);
                modified.Add(path);
            }
        }

        if (modified.Count > 0)
        {
            AssetDatabase.SaveAssets();
            Debug.Log($"Replaced shader on {modified.Count} material(s). Example: {modified[0]}");
        }
        else
        {
            Debug.Log("No materials using the custom shader were found.");
        }
    }
}
