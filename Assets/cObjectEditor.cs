using UnityEditor;
using UnityEngine;

public class SceneGUI2 : EditorWindow
{
    public static Texture2D assetIcon;
    public static bool showWindow;
    public static GameObject gameObject;
    public static Editor gameObjectEditor;
    public static Vector2 scrollPos;
    public static string t = "This is a string inside a Scroll view!";
    private static GameObject m_GameObject;

    [MenuItem("Tools/Object Editor")]
    static void ShowWindow()
    {
        GetWindow<SceneGUI2>("Object Editor");
    }

    [MenuItem("Tools/Enable %h")]
    public static void Enable()
    {
        if (showWindow)
            return;
        else
            showWindow = !showWindow;

        assetIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/cBrush/Resources/ThumbnailsGenerated/fern.png", typeof(Texture2D));

        SceneView.onSceneGUIDelegate += OnScene;
        Debug.Log("Scene GUI : Enabled");
        
    }

    [MenuItem("Tools/Disable #h")]
    public static void Disable()
    {
        if (showWindow)
            showWindow = !showWindow;

        SceneView.onSceneGUIDelegate -= OnScene;
        Debug.Log("Scene GUI : Disabled");
    }

    private static void OnScene(SceneView sceneview)
    {
        Handles.BeginGUI();

        //GUI.Box(new Rect(0, 0, 500, 100), "1st position");
  

        gameObject = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/fern.prefab", typeof(GameObject));

        if (GUI.Button(new Rect(20, 100, 50, 50), assetIcon))
        {

        }

        if (GUILayout.Button("Press Me", GUILayout.Width(100), GUILayout.Height(100))) {

            SceneGUI2.Disable();
        }

        m_GameObject = (GameObject)EditorGUILayout.ObjectField("", m_GameObject, typeof(GameObject), true, GUILayout.MaxWidth(90));

        GUILayout.Box("This is an sized label");



        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MaxWidth(1000), GUILayout.Height(300));
        GUILayout.BeginVertical(); // container for rows
        GUILayout.BeginHorizontal(); // container for row 1
        // draw first row components
        if (GUILayout.Button(assetIcon, GUILayout.MaxWidth(60), GUILayout.MaxHeight(60)))
        {

        }
        if (GUILayout.Button(assetIcon, GUILayout.MaxWidth(60), GUILayout.MaxHeight(60)))
        {

        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(); // container for row 2
        // draw second row components
        if (GUILayout.Button(assetIcon, GUILayout.MaxWidth(60), GUILayout.MaxHeight(60)))
        {

        }
        if (GUILayout.Button(assetIcon, GUILayout.MaxWidth(60), GUILayout.MaxHeight(60)))
        {

        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();


        GUILayout.Label(t);

        if (GUILayout.Button("Press Me"))
        {

            t += " \nAnd this is more text!";
        }

        if (GUILayout.Button(assetIcon, GUILayout.MaxWidth(60), GUILayout.MaxHeight(60)))
        {

        }
        
        EditorGUILayout.EndScrollView();
        Handles.EndGUI();
    }
}