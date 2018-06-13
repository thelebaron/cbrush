using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace Structura
{
    [CustomEditor(typeof(Floor))]
    public class StrucFloorInspector : Editor
    {
        void OnSceneGUI()
        {



            Floor floor = target as Floor;



            Handles.color = Color.magenta;
            Handles.color = Color.red;
            Handles.CircleHandleCap(0,floor.transform.position + new Vector3(5, 0, 0),floor.transform.rotation * Quaternion.LookRotation(new Vector3(1, 0, 0)),1,EventType.Repaint);


            //
            //text
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.green;

            Handles.BeginGUI();
            Vector3 pos = floor.transform.position;
            Vector2 pos2D = HandleUtility.WorldToGUIPoint(pos);
            GUI.Label(new Rect(pos2D.x, pos2D.y, 100, 100), pos.ToString(), style);
            Handles.EndGUI();
            //

            EditorGUI.BeginChangeCheck();
            
            for (int i = 0; i < floor.m_Points.Count; i++)
            {
                Handles.color = Color.magenta;
                floor.m_HandlePoints[i] = Handles.PositionHandle(floor.m_HandlePoints[i], Quaternion.identity);
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
            Floor floor = target as Floor;
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
            Floor floor = target as Floor;
            for (int i = 0; i < floor.Corners; i++)
            {
                //floor.m_Initialised = false;
                //floor.Update();//Refresh in editor view
                //floor.Update();//Refresh in editor view
            }
        }
    }
}