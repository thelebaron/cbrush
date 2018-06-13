using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace Core
{
    [CustomEditor(typeof(OldWall))]
    public class NewWallInspector : Editor
    {

        void OnSceneGUI()
        {
            OldWall wall = target as OldWall;

            EditorGUI.BeginChangeCheck();
            wall.WallVectorStartPoint = Handles.PositionHandle(wall.WallVectorStartPoint, Quaternion.identity);
            wall.WallVectorEndPoint = Handles.PositionHandle(wall.WallVectorEndPoint, Quaternion.identity);

            Handles.color = Color.magenta;
 
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Look Target");
                wall.Update();
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            OldWall wall = target as OldWall;

            if (GUILayout.Button("Debug Array"))
            {
                wall.DebugEditorButton();
            }
            

            if (GUILayout.Button("Create Prefab Array"))
            {
                wall.CreatePrefabArray();
            }
            if (GUILayout.Button("Clear Prefab Array"))
            {
                wall.ClearPrefabArray();
            }
        }
    }
}