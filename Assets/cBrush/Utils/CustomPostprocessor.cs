using UnityEngine;
using UnityEditor;
/*
 https://docs.unity3d.com/ScriptReference/AssetPostprocessor.html
     https://coderwall.com/p/kycnlw/asset-post-processor-in-unity
     http://answers.unity3d.com/questions/14703/how-to-start-coding-with-assetpostprocessor.html
     */
public class CustomPostprocessor : AssetPostprocessor
{
    // Before the texture is imported
    public void OnPreprocessTexture()
    {
        // Apple settings to all atlases
        if (assetPath.Contains("_atlas"))
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            textureImporter.textureType = TextureImporterType.Default;
            textureImporter.mipmapEnabled = false;
            // All your import settings for atlases
        }
    }

    // After the texture is imported
    public void OnPostprocessTexture(Texture2D texture)
    {
        // Invert colors
        if (assetPath.Contains("_invert_color"))
        {
            for (int m = 0; m < texture.mipmapCount; m++)
            {
                Color[] c = texture.GetPixels(m);
                for (int i = 0; i < c.Length; i++)
                {
                    c[i].r = 1 - c[i].r;
                    c[i].g = 1 - c[i].g;
                    c[i].b = 1 - c[i].b;
                }
                texture.SetPixels(c, m);
            }
        }
    }
}