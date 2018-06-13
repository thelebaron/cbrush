using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

public class GameObjectEditorWindow : EditorWindow
{
    private static Camera cam;
    private static GameObject camGo;
    private static GameObject tmpGo;

    int width = 400;
    int height = 400;
    GameObject gameObject;
    public Mesh mesh;
    public Material material;
    Editor gameObjectEditor;



    [MenuItem("Window/GameObject")]
    static void ShowWindow()
    {
        GetWindow<GameObjectEditorWindow>("GameObject Editor");
       
    }

    void Init() {

        var sqr = width;

        gameObject = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/fern.prefab", typeof(GameObject));

        //set game object's attribs
        //====================================================================
        if(tmpGo==null)
            tmpGo = Instantiate(gameObject) as GameObject;
        tmpGo.name = "fern_ins";

        if (camGo == null)
            camGo = new GameObject();
        camGo.name = "camgo";


        camGo.transform.position = new Vector3(0.0f, 0.0f, -5f);
        tmpGo.transform.position = new Vector3(0f,-0.5f,0f);
        tmpGo.hideFlags = HideFlags.DontSave;
        camGo.hideFlags = HideFlags.DontSave;


        cam = camGo.GetComponent<Camera>();
        if(cam == null)
            cam = camGo.AddComponent<Camera>();

        cam.clearFlags = CameraClearFlags.Color;
        cam.backgroundColor = Color.grey;
        cam.enabled = false;
        cam.orthographic = false;
        cam.fieldOfView = 15;
        cam.orthographicSize = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().orthographicSize;
        cam.targetTexture = new RenderTexture((int)width, (int)height, 24, RenderTextureFormat.ARGB32);
        cam.Render();
        RenderTexture.active = cam.targetTexture;
        
        Texture2D scr = new Texture2D(sqr, sqr, TextureFormat.RGB24, false);
        // false, meaning no need for mipmaps
        scr.ReadPixels(new Rect(0, 0, sqr, sqr), 0, 0);
        RenderTexture.active = null; //can help avoid errors 
        cam.targetTexture = null;
        // consider ... Destroy(tempRT);

        byte[] bytes;
        bytes = scr.EncodeToPNG();

        File.WriteAllBytes(Application.dataPath + "/Scr.png", bytes);

        DestroyImmediate(camGo);
        DestroyImmediate(tmpGo);
    }

    void OnGUI()
    {

        Init();

        gameObject = (GameObject)EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

        if (gameObject != null)
        {
            if (gameObjectEditor == null)
                gameObjectEditor = Editor.CreateEditor(gameObject);

            //gameObjectEditor.OnPreviewGUI(GUILayoutUtility.GetRect(400, 400), EditorStyles.whiteLabel);
            




            //GUI.DrawTexture(new Rect(0, 0, position.width, position.height), cam.targetTexture);

            
        }
        
    }


}