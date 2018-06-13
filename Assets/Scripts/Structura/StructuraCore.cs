using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Structura
{
    [ExecuteInEditMode]
    public class StructuraCore : MonoBehaviour
    {
        [LargeHeader("Core functions")]
        //public BuildingBlock BuildingResourceFile;
        //private readonly int Corners = 4;
        public bool m_Initialised;
        public string m_StructureName = "New Structure";

        [Range(1, 100)]
        public int Rows = 2;
        [Range(1, 100)]
        public int Floors = 2;
        public float FloorHeight = 1;
        public float PrefabZDepth = 0.5f;


        public Floor m_Floor;
        public Wall m_Wall;

        public bool Initialised
        { get { return m_Initialised; } }


        public void Initialise()
        {
            m_Initialised = true;
            name = m_StructureName;
            m_Floor = gameObject.AddComponent<Floor>() as Floor;
            m_Wall = gameObject.AddComponent<Wall>() as Wall;
            m_Floor.Update();
            m_Wall.Update();
            if (m_Floor.m_Initialised)
            {
                //AddComponentMenu wa;l
            }
            /*
            //Will Need to comment out for release or figure out workaround, dont want default always when initialising building
            BuildingResourceFile = Resources.Load("BuildingBlocks/DefaultBuildingResource", typeof(BuildingBlock)) as BuildingBlock;

            Rows = 2;
            Floors = 2;
            m_Walls = new BuildingWall[Corners];
            this.gameObject.name = "Building";

            m_Floor = gameObject.AddComponent<BuildingFloor>() as BuildingFloor;

            for (int i = 0; i < Corners; i++)
            {
                var go = new GameObject();
                string number = i.ToString();
                go.name = "Wall" + number;
                go.transform.SetParent(transform);
                var WallComponent = go.AddComponent<BuildingWall>() as BuildingWall;
                WallComponent.WallNumber = i;
                m_Walls[i] = WallComponent;
            }
            */
        }

        public void AddWall()
        {
            var newAverage = (m_Floor.m_Points[m_Floor.m_Points.Count - 1] + m_Floor.m_Points[0]);
            newAverage = newAverage / 2;
            m_Floor.m_Points.Add(newAverage);
            m_Floor.m_HandlePoints.Add(newAverage);

            m_Floor.Update();

            //Debug.Log("Added wall at "+ newAverage);
        }

        public void RemoveWall()
        {
            var segmentToRemove = (m_Floor.m_Points.Count - 1);
            if (segmentToRemove > 0)
            {
                DestroyImmediate(m_Wall.m_SubWall[segmentToRemove].gameObject);
                m_Floor.m_Points.Remove(m_Floor.m_Points[segmentToRemove]);
            }
            else
                return;

            m_Floor.Update();
            //Debug.Log("Removed wall at " + segmentToRemove);
        }

        public void PropagateChanges()
        {
            m_Wall.Rows = Rows;
            m_Wall.Floors = Floors;
            m_Wall.DefaultFloorHeight = FloorHeight;
            m_Wall.RowSize = new Vector3(0, 0, PrefabZDepth);
            m_Wall.DefaultRowDepth = PrefabZDepth;
            m_Wall.Update();

            /*
            for (int i = 0; i < m_Walls.Length; i++)
            {
                var component = m_Walls[i].transform.GetComponent<BuildingWall>();
                component.WallVectorStartPoint = m_Floor.m_LocalCornerVertices[i];

                var o = i + 1;//calculate end number
                if (o > (m_Walls.Length - 1))
                { o = 0; }

                component.WallVectorEndPoint = m_Floor.m_LocalCornerVertices[o];
                //var WallComponent = w.transform.GetComponent<BuildingWall>();
                component.Rows = Rows;
                component.Floors = Floors;
                component.floorHeight = FloorHeight;
                component.RowSize = new Vector3(0, 0, PrefabZDepth);
                component.Update();
            }

            foreach (BuildingWall w in m_Walls)
            {

            }
            */
        }

        public void Clear()
        {
            m_Initialised = false;
            DestroyImmediate(GetComponent<Rigidbody>());
            DestroyImmediate(GetComponent<Animator>());
            DestroyImmediate(GetComponent<CapsuleCollider>());
            DestroyImmediate(GetComponent<Floor>());
            DestroyImmediate(GetComponent<Wall>());
            
            DestroyImmediate(GetComponent<MeshRenderer>());
            DestroyImmediate(GetComponent<MeshFilter>());
            DestroyImmediate(GetComponent<MeshRenderer>());
            DestroyImmediate(GetComponent<MeshFilter>());

            var children = new List<GameObject>();
            foreach (Transform child in transform) children.Add(child.gameObject);
            children.ForEach(child => DestroyImmediate(child));
        }

        public void Update()
        {

        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            //Because initialise is called in StructuraCoreInspector, the order is a bit funky,
            //Stops an editor error, and just try again on the next update
            if (m_Floor == null)
                return;

            for (int i = 0; i < m_Floor.m_Points.Count; i++)
            {

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(m_Floor.m_Points[i], 0.1f);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(m_Floor.m_Points[i], m_Floor.m_Points[m_Floor.GetPartnerPoint(m_Floor.m_Points, i)]);

            }
        }

    }
}