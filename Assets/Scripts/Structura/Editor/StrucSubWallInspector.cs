using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace Structura
{
    [CustomEditor(typeof(SubWall))]
    public class StrucSubWallInspector : Editor
    {
        void OnSceneGUI()
        {
            SubWall sw = target as SubWall;

            EditorGUI.BeginChangeCheck();

            sw.m_Floor.m_HandlePoints[sw.WallIndexStart] = Handles.PositionHandle(sw.m_Floor.m_HandlePoints[sw.WallIndexStart], Quaternion.identity);
            sw.m_Floor.m_HandlePoints[sw.WallIndexEnd] = Handles.PositionHandle(sw.m_Floor.m_HandlePoints[sw.WallIndexEnd], Quaternion.identity);
            sw.m_Floor.Update();
            sw.m_Wall.Update();

            Handles.color = Color.magenta;
 
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Look Target");
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            SubWall sw = target as SubWall;

            //Add row
            //Subtract row
            //Add floor
            //Subtract floor
            if (GUILayout.Button("Split Wall"))
            {
                var newAverage = (sw.m_Floor.m_Points[sw.WallIndexStart] + sw.m_Floor.m_Points[sw.WallIndexEnd]);
                newAverage = newAverage / 2;
                sw.m_Floor.m_Points.Insert(sw.WallIndexEnd, newAverage);
                sw.m_Floor.m_HandlePoints.Insert(sw.WallIndexEnd, newAverage);

                sw.m_Floor.Update();
                sw.m_Wall.Reset();

                GameObject[] newSelection = new GameObject[1];
                newSelection[0] = sw.m_Wall.m_SubWall[sw.WallIndexEnd].gameObject;
                Selection.objects = newSelection;
                //Debug.Log("Added wall at " + newAverage);
                //sw.Split();
                /* List<T> someList = new List();
 someList.Add(x)        // Adds x to the end of the list
 someList.Insert(0, x)  // Adds x at the given index
 someList.Remove(x)     // Removes the first x observed
 someList.RemoveAt(0)   // Removes the item at the given index
 someList.Count()       // Always good to know how many elements you have!*/
            }
            
            if (GUILayout.Button("Remove Wall"))
            {
                //sw.ClearPrefabArray();
            }
        }
    }
}