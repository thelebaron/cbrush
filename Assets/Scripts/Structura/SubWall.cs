using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Structura
{
    public class SubWall : MonoBehaviour
    {
        public bool Locked;
        public int Rows;
        public int Floors;
        public float RowWidth;
        public float FloorHeight;
        //start and end floor points needed for editor specific handles
        public int WallIndexStart;
        public int WallIndexEnd;
        public List<Vector3> m_Vertices;
        public List<GameObject> m_Prefabs;
        public Mesh m_Mesh;
        public Wall m_Wall;
        public Floor m_Floor;

        public void SplitWall()
        {
            
            //
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            if (m_Vertices == null)
                return;

            for (int i = 0; i < m_Vertices.Count; i++)
            {
                Gizmos.DrawSphere(m_Vertices[i], 0.025f);
            }
        }
    }
}