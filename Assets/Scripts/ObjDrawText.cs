using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ObjDrawText : MonoBehaviour
{
    public static void Draw(Transform child, float distance, Color c)
    {
        GUIStyle style = new GUIStyle();
        Vector3 oncam = Camera.current.WorldToScreenPoint(child.position);
        /*
        if ((oncam.x >= 0)(oncam.x <= Camera.current.pixelWidth)(oncam.y >= 0)(oncam.y <= Camera.current.pixelHeight)(oncam.z > 0)(oncam.z < distance))
        {
            style.normal.textColor = c;
            Handles.Label(child.position, child.name, style);
        }*/
    }
}
/*Transform[] allChildren = GetComponentsInChildren<Transform>();
foreach (Transform child in allChildren) {
    #if UNITY_EDITOR
        ObjDrawText.Draw(child,500,Color.red);
    #endif
}
 **/
