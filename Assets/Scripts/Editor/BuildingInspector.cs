using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace Core
{
    [CustomEditor(typeof(Building))]
    public class BuildingInspector : Editor
    {

        void OnSceneGUI()
        {
            EditorGUI.BeginChangeCheck();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Look Target");
            }
        }

        public override void OnInspectorGUI()
        {
            Building Building = target as Building;
            if ((GUILayout.Button("Make Art")) && !Building.Initialised)
            {
                Building.m_Initialised = true;
                Building.Initialise();//Refresh in editor view
            }
            DrawDefaultInspector();
            if (GUILayout.Button("Propagate Changes"))
            {
                Building.PropagateChanges();//
            }
            if (GUILayout.Button("Clear Building"))
            {
                Building.m_Initialised = false;
                Building.Clear();//Refresh in editor view
            }
        }
        
    }
}