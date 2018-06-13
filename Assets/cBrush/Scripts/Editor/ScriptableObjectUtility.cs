using UnityEngine;
using UnityEditor;
using System.IO;

public static class ScriptableObjectUtility
{
    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary>
    // by digi from youtube https://www.youtube.com/watch?v=O8BMCQnjUe4

    public static void CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    public static void CreateAssetPath<T>(string custompath) where T : ScriptableObject
    {
        //Must always have assets in path to work
        T asset = ScriptableObject.CreateInstance<T>();
        
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(custompath + "/Default" + typeof(T).ToString() + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [MenuItem("BigBrush/Create Session")]
    public static void CreateBigBrush()
    {
        //ScriptableObjectUtility.CreateAssetPath<BigBrushSessionRecorder>("Assets/Resources/");
    }
    [MenuItem("BigBrush/Settings")]
    public static void CreateBigBrushSettings()
    {
        //ScriptableObjectUtility.CreateAssetPath<BigBrushSettings>("Assets/Resources/");
    }
}