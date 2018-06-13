using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace Core
{
    [CustomEditor(typeof(BuildingWall))]
    public class WallInspector : Editor
    {

        void OnSceneGUI()
        {
            BuildingWall wall = target as BuildingWall;

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

            BuildingWall wall = target as BuildingWall;
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