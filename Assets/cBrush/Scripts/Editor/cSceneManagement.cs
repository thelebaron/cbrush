using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public static class cSceneManagement
{
    public static string WorkingSceneName;
    public static Scene WorkingScene;

    public static void NewScene()
    {
        string[] path = EditorSceneManager.GetActiveScene().path.Split(char.Parse("/"));
        path[path.Length - 1] = "" + path[path.Length - 1];
        bool saveOK = EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), string.Join("/", path));
        Debug.Log("Saved Scene " + (saveOK ? "OK" : "Error!"));
        //OnInspectorUpdate();

        WorkingScene = SceneManager.GetActiveScene();
        WorkingSceneName = WorkingScene.path;
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
    }
    public static void NewSceneNosave()
    {
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
    }
    public static void ReloadWorkingScene()
    {
        EditorSceneManager.OpenScene(WorkingSceneName, OpenSceneMode.Single);
    }
}