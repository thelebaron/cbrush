using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(ObjSpray))]
public class ObjSprayInspector : Editor {

    void OnSceneGUI()
    {
        //EditorGUI.BeginChangeCheck();

        RaycastHit hit;
        //Grab references
        ObjSpray os = target as ObjSpray;


        Event e = Event.current;

        if (Event.current.type == EventType.MouseMove) {
            Vector2 guiPosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    Debug.Log("ClickerEvent");
                }









                //Debug.Log("Found an object - distance: " + hit.distance);
                os.rayPos = hit.point;
            }
        }
            //Physics.Raycast(Event.current.mouseRay);
    }
}
