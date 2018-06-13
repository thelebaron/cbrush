using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace Structura
{
    [CustomEditor(typeof(Wall))]
    public class StrucWallInspector : Editor
    {
        void OnSceneGUI()
        {
            Wall wall = target as Wall;

            EditorGUI.BeginChangeCheck();
            //wall.WallVectorStartPoint = Handles.PositionHandle(wall.WallVectorStartPoint, Quaternion.identity);
            //wall.WallVectorEndPoint = Handles.PositionHandle(wall.WallVectorEndPoint, Quaternion.identity);

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

            Wall wall = target as Wall;
            if (GUILayout.Button("Reset"))
            {
                wall.Reset();
            }
            if (GUILayout.Button("Clear Prefab Array"))
            {
                wall.ClearPrefabArray();
            }
        }
    }
}