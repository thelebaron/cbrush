using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Core
{

    [ExecuteInEditMode]
    public class Building : MonoBehaviour 
	{
        public BuildingBlock BuildingResourceFile;
        private readonly int Corners = 4;
        public bool m_Initialised;

        [Range(1, 100)]
        public int Rows = 2;
        [Range(1, 100)]
        public int Floors = 2;
        public float FloorHeight = 1;
        public float PrefabZDepth = 0.5f;


        public BuildingFloor m_Floor;
        public BuildingWall[] m_Walls;
        
        public bool Initialised
        { get { return m_Initialised; } }


        public void Initialise()
        {
            m_Initialised = true;

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
                go.name = "Wall"+ number;
                go.transform.SetParent(transform);
                var WallComponent = go.AddComponent<BuildingWall>() as BuildingWall;
                WallComponent.WallNumber = i;
                m_Walls[i] = WallComponent;
            }
        }

        public void PropagateChanges()
        {

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
        }

        public void Clear()
        {
            m_Initialised = false;
            DestroyImmediate(GetComponent<Rigidbody>());
            DestroyImmediate(GetComponent<Animator>());
            DestroyImmediate(GetComponent<CapsuleCollider>());
            DestroyImmediate(GetComponent<BuildingFloor>());
            DestroyImmediate(GetComponent<BuildingWall>());
            DestroyImmediate(GetComponent<BuildingWall>());
            DestroyImmediate(GetComponent<BuildingWall>());
            DestroyImmediate(GetComponent<BuildingWall>());
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

    }
}