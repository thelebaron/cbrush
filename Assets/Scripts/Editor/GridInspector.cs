using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace Core
{
    [CustomEditor(typeof(Grid))]
    public class GridInspector : Editor
    {

        void OnSceneGUI()
        {
            Grid grid = target as Grid;

            EditorGUI.BeginChangeCheck();
            //Vector3 p0 = Handles.PositionHandle(grid.p0, Quaternion.identity);
            grid.p0 = Handles.PositionHandle(grid.p0, Quaternion.identity);
            grid.p1 = Handles.PositionHandle(grid.p1, Quaternion.identity);

            Handles.color = Color.magenta;
 
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Look Target");
                //grid.lookTarget = lookTarget;
                
                grid.Update();
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Grid grid = target as Grid;
            if (GUILayout.Button("Create Prefab Array"))
            {
                grid.CreatePrefabArray();
            }
            if (GUILayout.Button("Clear Prefab Array"))
            {
                grid.ClearPrefabArray();
            }
        }
    }
}