using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Core
{
    [ExecuteInEditMode]
    public class BuildingFloor : MonoBehaviour
    {
        //test
        public bool UnifyHandles
        {
            get { return m_UnifyHandles; }
            set { m_UnifyHandles = value; }
        }
        private bool m_UnifyHandles;
        public int Corners
        {
            get
            {
                return m_Corners;
            }
        }
        private int m_Corners = 4;
        public Vector3[] m_LocalCornerVertices;
        public Vector3[] m_WorldCornerVertices;

        public bool m_Initialised;
        

        public void Start()
        {
            if (!m_Initialised)
            {
                m_Initialised = true;
                transform.position = new Vector3(0, 0, 0);

                //Initialise the arrays
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
        }

        public void GenerateFloor()
        {
            Start();
            
            if (!m_UnifyHandles)
            {
                for (int i = 0; i < Corners; i++)
                {
                    var oldPos = m_LocalCornerVertices[i];
                    var newPos = oldPos + transform.position;
                    m_WorldCornerVertices[i] = newPos;
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

            for (int i = 0; i < Corners; i++)
            {
                var oldPos = m_WorldCornerVertices[i];
                var newPos = oldPos - transform.position;
                m_LocalCornerVertices[i] = newPos;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            
            for (int i = 0; i < m_WorldCornerVertices.Length; i++)
            {
                Gizmos.DrawSphere(m_WorldCornerVertices[i], 0.1f);
            }
        }


        //Helper functions
        /*
        public void CheckTransformPosChange()
        {

            Vector3 offset = transform.position - m_LastPos;
            if (offset.x > m_PosChangeThreshold)
            {
                m_LastPos = transform.position;
                // update lastPos 
                // code to execute when X is getting bigger 

                Debug.Log("Transform changed");
                //UpdateLocalPos();
            }
            else if (offset.x < -m_PosChangeThreshold)
            {
                m_LastPos = transform.position;
                // update lastPos 
                // code to execute when X is getting smaller 

                //Generate(m_Corners);
            }
        }
        */
        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
        }

        public static float GetPointF(float p0, float p1, float p2, float t)
        {
            return Mathf.Lerp(Mathf.Lerp(p0, p1, t), Mathf.Lerp(p1, p2, t), t);
        }
    }
}