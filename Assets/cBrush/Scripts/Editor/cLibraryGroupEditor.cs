using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace cBrush
{

    [CustomEditor(typeof(cLibraryGroup))]
    public class cLibraryGroupEditor : Editor
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
            cLibraryGroup lg = target as cLibraryGroup;
            if ((GUILayout.Button("Make Thumbnails")))
            {
                lg.MakeThumbnails();
            }

            if ((GUILayout.Button("SetupDictionary")))
            {
                lg.SetupDic();
            }

            DrawDefaultInspector();
            if ((GUILayout.Button("Add")))
            {
                lg.Add();
            }

            if ((GUILayout.Button("Clear")))
            {
                lg.Reset();
            }

            if ((GUILayout.Button("Parse Directory")))
            {
                lg.ParseDirectory();
            }
        }

    }
    
}