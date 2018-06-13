using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace Structura
{
    [CustomEditor(typeof(StructuraCore))]
    public class StructuraCoreInspector : Editor
    {
        void OnSceneGUI()
        {
            EditorGUI.BeginChangeCheck();

            //Grab references
            StructuraCore sc = target as StructuraCore;
            var scf = sc.m_Floor;

            //Because initialise is called in StructuraCoreInspector, the order is a bit funky,
            //Stops an editor error, and just try again on the next update
            if (scf == null)
                return;

            for (int i = 0; i < scf.m_Points.Count; i++)
            {
                Handles.color = Color.magenta;
                scf.m_HandlePoints[i] = Handles.PositionHandle(scf.m_HandlePoints[i], Quaternion.identity);
                scf.Update();
                sc.m_Wall.Update();
            }

            //
            //text
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.green;

            Handles.BeginGUI();
            Vector3 pos = sc.transform.position;
            Vector2 pos2D = HandleUtility.WorldToGUIPoint(pos);
            GUI.Label(new Rect(pos2D.x, pos2D.y, 100, 100), pos.ToString(), style);
            Handles.EndGUI();
            //

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Look Target");
            }
        }



        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            StructuraCore sc = target as StructuraCore;
            //var scf = sc.m_Floor;
            
            //Initialise if not already done
            if (!sc.Initialised)
            {
                sc.m_Initialised = true;
                sc.Initialise();
                SceneView.RepaintAll();
                SceneView.RepaintAll();

            }
            if ((GUILayout.Button("Initialise")) && !sc.Initialised)
            {
                sc.m_Initialised = true;
                sc.Initialise();//Refresh in editor view
            }

            if ((GUILayout.Button("Add Wall")))
            {
                sc.AddWall();//Refresh in editor view
                sc.m_Wall.Reset();
            }
            if ((GUILayout.Button("Remove Wall")))
            {
                sc.RemoveWall();//Refresh in editor view
            }
            if ((GUILayout.Button("Toggle material visiblity")))
            {
                //sc.//
            }



            if (GUILayout.Button("Regenerate"))
            {
                sc.PropagateChanges();//
            }
            if (GUILayout.Button("Clear Building"))
            {
                sc.m_Initialised = false;
                sc.Clear();//Refresh in editor view
            }

            SceneView.RepaintAll();
        }

    }
}