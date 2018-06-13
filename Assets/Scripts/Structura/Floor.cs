using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Structura
{
    [ExecuteInEditMode]
    public class Floor : MonoBehaviour
    {
        //test
        public bool m_ClosedStructure;
        public int m_Corners = 4;
        //world points
        public List<Vector3> m_Points;
        //editor handles
        public List<Vector3> m_HandlePoints;



        public bool UnifyHandles
        {
            get { return m_UnifyHandles; }
            set { m_UnifyHandles = value; }
        }
        public bool m_UnifyHandles;
        public int Corners
        {
            get
            {
                return m_Corners;
            }
        }

        //old
        public Vector3[] m_LocalCornerVertices;
        public Vector3[] m_WorldCornerVertices;

        public bool m_Initialised;

        public bool m_Debug;


        public void Start()
        {
            if (!m_Initialised)
            {
                m_Initialised = true;
                transform.position = new Vector3(0, 0, 0);

                //Initialise the arrays
                m_Points = new List<Vector3>();
                m_HandlePoints = new List<Vector3>();

                //Make a square to start with
                m_Points.Add(new Vector3(0, 0, 0));
                m_Points.Add(new Vector3(1, 0, 0));
                m_Points.Add(new Vector3(1, 0, 1));
                m_Points.Add(new Vector3(0, 0, 1));

                m_HandlePoints.Add(new Vector3(0, 0, 0));
                m_HandlePoints.Add(new Vector3(1, 0, 0));
                m_HandlePoints.Add(new Vector3(1, 0, 1));
                m_HandlePoints.Add(new Vector3(0, 0, 1));

                m_LocalCornerVertices = new Vector3[(1) * (m_Corners)];
                m_WorldCornerVertices = new Vector3[(1) * (m_Corners)];

                //Make a square to start with
                m_LocalCornerVertices[0] = new Vector3(0, 0, 0);
                m_LocalCornerVertices[1] = new Vector3(1, 0, 0);
                m_LocalCornerVertices[2] = new Vector3(1, 0, 1);
                m_LocalCornerVertices[3] = new Vector3(0, 0, 1);
            }

        }

        public void Update()
        {
            GenerateFloor();
            if (m_Debug) {
                m_Debug = false;
                //Add debug code
            }
        }
        

        public void GenerateFloor()
        {
            Start();

            if (!m_UnifyHandles)
            {
                for (int i = 0; i < m_Points.Count; i++)
                {
                    var OLDPOS = m_HandlePoints[i];
                    var NEWPOS = OLDPOS + transform.position;
                    m_Points[i] = NEWPOS;

                    //oldcode
                    //var oldPos = m_LocalCornerVertices[i];
                    //var newPos = oldPos + transform.position;
                    //m_WorldCornerVertices[i] = newPos;
                }
            }
            else
            {
                DoUnifyHandles();
            }

        }

        public void DoUnifyHandles()
        {
            m_UnifyHandles = false;
            transform.position = new Vector3(0, 0, 0);

            for (int i = 0; i < m_Points.Count; i++)
            {
                var OLDPOS = m_Points[i];
                var NEWPOS = OLDPOS - transform.position;
                m_HandlePoints[i] = NEWPOS;

                //old
                //var oldPos = m_WorldCornerVertices[i];
                //var newPos = oldPos - transform.position;
                //m_LocalCornerVertices[i] = newPos;
            }
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            for (int i = 0; i < m_Points.Count; i++)
            {

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(m_Points[i], 0.1f);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(m_Points[i], m_Points[GetPartnerPoint(m_Points, i)]);

            }
        }
        
        //Gets the partner point(vector3) in a list. 
        //ie point[0] and point[1] is its pair for a wall segment
        public int GetPartnerPoint(List<Vector3> INPUT, int i)
        {
            //To ensure we arent looking for a phantom integer
            //Subtract 1 because lists and arrays start at [0]
            if (i != (INPUT.Count - 1))
            {
                i = i + 1;
                return i;
            }
            else
            {
                i = 0;
                return i;
            }
        }
    }
}