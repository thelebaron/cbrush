using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace Core
{
    [CustomEditor(typeof(CustomSceneMouseMoverUI))]
    public class CustomSceneMouseMoverUIInspector : Editor
    {

        void OnSceneGUI()
        {
            Event e = Event.current;
            //Debug.Log(e.mousePosition);
            CustomSceneMouseMoverUI t = target as CustomSceneMouseMoverUI;
            //Check the event type and make sure it's left click.
            if (e.type == EventType.MouseDrag || e.type == EventType.MouseDown &&  e.button == 1)
            {

                //if (t.AllowMouseMove)
                //{
                    var c = SceneView.GetAllSceneCameras();
                    Vector3 position = SceneView.lastActiveSceneView.pivot;
                    position.z += 0.01f;
                    SceneView.lastActiveSceneView.pivot = position;
                    SceneView.lastActiveSceneView.Repaint();

                    /* Do stuff
                       * * * *
                     * */
                    //e.Use();  //Eat the event so it doesn't propagate through the editor.
                //}

            }


            //
            Handles.color = Color.magenta;
            //Handles.selectedColor = Color.magenta;

            EditorGUI.BeginChangeCheck();
            //Handles code was here


            if (EditorGUI.EndChangeCheck())
            {


                Undo.RecordObject(target, "Changed Look Target");
                //floor.lookTarget = lookTarget;
                
                //floor.Update();
            }
        }

        public override void OnInspectorGUI()
        {
            CustomSceneMouseMoverUI t = target as CustomSceneMouseMoverUI;

            if (GUILayout.Button("Toggle"))
            {
                t.Toggle();
            }
           
            DrawDefaultInspector();


            

        }


    }
}