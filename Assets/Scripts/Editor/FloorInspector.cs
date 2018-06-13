using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace Core
{
    [CustomEditor(typeof(BuildingFloor))]
    public class FloorInspector : Editor
    {

        void OnSceneGUI()
        {
            BuildingFloor floor = target as BuildingFloor;
            Handles.color = Color.magenta;

            EditorGUI.BeginChangeCheck();
            
            for (int i = 0; i < floor.Corners; i++)
            {
                floor.m_LocalCornerVertices[i] = Handles.PositionHandle(floor.m_LocalCornerVertices[i], Quaternion.identity);
                floor.Update();
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Look Target");
                //floor.lookTarget = lookTarget;
                
            }
        }

        public override void OnInspectorGUI()
        {
            BuildingFloor floor = target as BuildingFloor;
            if (GUILayout.Button("Unify Handles"))
            {
                floor.UnifyHandles = true;
                floor.Update();//Refresh in editor view
            }
            DrawDefaultInspector();

            if (GUILayout.Button("Reset"))
            {
                Reset();
            }


        }

        public void Reset()
        {
            BuildingFloor floor = target as BuildingFloor;
            for (int i = 0; i < floor.Corners; i++)
            {
                //floor.m_Initialised = false;
                //floor.Update();//Refresh in editor view
                //floor.Update();//Refresh in editor view
            }
        }
    }
}