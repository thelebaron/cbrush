using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Core
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    [ExecuteInEditMode]
    public class BuildingWall : MonoBehaviour 
	{
        public bool m_Initialised;
        public int WallNumber;

        //BuildingWall
        public int Rows = 2;
        public int Floors = 2;
        private readonly int Sides = 4;
        public float floorHeight = 1;
        public float chunkWidth = 1;
        private int xSize, ySize; //modified by floors and rows
        public Vector3 WallVectorStartPoint;
        public Vector3 WallVectorEndPoint;
        private Vector3 m_XStartPos; //modified by wallvector points
        private Vector3 m_XEndPos;
        private Vector3[] m_Vertices;
        private Mesh mesh;

        public bool m_EditorHandleInit;
        public float m_EditorHandleDistance;

        public bool m_ChunkModelInit;
        public GameObject FacadePrefab;
        public MeshFilter facadeMesh;
        public MeshRenderer facadeMeshRenderer;
        public Vector3 RowSize;
        private Vector3 RowBounds_Readonly;
        private Vector3 RowBoundsPost_Readonly;
        public float scale = 1;
        private Vector3[] originalVertices;
        private Vector3[] _chunkVertices;
        private int[] sortVerts;
        private List<int> sortedVerts;
        //old code
        public GameObject[] prefabArray;
        public bool m_ArrayEmpty;

        //Replacement for prefab usage
        public Block[] BlockArray;

        public void Start()
        {
            //Initial setup
            m_Initialised = true;
            WallVectorEndPoint = new Vector3(1, 0, 0);
            RowSize = new Vector3(0f, 0f, 0.5f);
            MeshRenderer mr = transform.GetComponent<MeshRenderer>();
            //mr.enabled = false;

            // old code
            mr.material = Resources.Load("BuildingBlocks/TransparentBuildingMaterial", typeof(Material)) as Material;
            FacadePrefab = Resources.Load("BuildingBlocks/FacadePrefab", typeof(GameObject)) as GameObject;
            prefabArray = new GameObject[Rows * Floors]; 

            //Replacement allows for changing prefab
            BlockArray = new Block[Rows * Floors];



            //reference
            //m_LocalCornerVertices[0] = new Vector3(0, 0, 0);
            //m_LocalCornerVertices[1] = new Vector3(1, 0, 0);
            //m_LocalCornerVertices[2] = new Vector3(1, 0, 1);
            //m_LocalCornerVertices[3] = new Vector3(0, 0, 1);     

            if (WallNumber == 0)
            {
                WallVectorStartPoint = new Vector3(0, 0, 0);
                WallVectorEndPoint = new Vector3(1, 0, 0);
            }

            if (WallNumber == 1)
            {
                WallVectorStartPoint = new Vector3(1, 0, 0);
                WallVectorEndPoint = new Vector3(1, 0, 1);
            }

            if (WallNumber == 2)
            {
                WallVectorStartPoint = new Vector3(1, 0, 1);
                WallVectorEndPoint = new Vector3(0, 0, 1);
            }

            if (WallNumber == 3)
            {
                WallVectorStartPoint = new Vector3(0, 0, 1);
                WallVectorEndPoint = new Vector3(0, 0, 0);
            }


        }

        public void Update()
        {
            OnEditorHelpers();
            Generate(Sides);
            UpdatePrefabArray();
        }

        private void OnEditorHelpers()
        {
            //Start helper position
            var mS = (Rows + 1) * 0;
            //End helper position
            var mE = mS + Rows;

            m_XStartPos.x = WallVectorStartPoint.x;
            m_XStartPos.y = WallVectorStartPoint.y;
            m_XStartPos.z = WallVectorStartPoint.z;
            m_XEndPos.x = WallVectorEndPoint.x;
            m_XEndPos.y = WallVectorEndPoint.y;
            m_XEndPos.z = WallVectorEndPoint.z;

            m_EditorHandleDistance = Vector3.Distance(WallVectorStartPoint, WallVectorEndPoint);
        }

        private void Generate(int sides)
        {
            if (!m_Initialised)
            {
                Start();
            }

            ySize = Floors;
            xSize = Rows;
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            mesh.name = "Procedural BuildingWall";

            chunkWidth = m_EditorHandleDistance / Rows;

            m_Vertices = new Vector3[(xSize + 1) * (ySize + 1)];

            for (int i = 0, y = 0; y <= ySize; y++)
            {
                for (int x = 0; x <= xSize; x++, i++)
                {
                    m_Vertices[i] = new Vector3(x * chunkWidth, y * floorHeight);
                }
            }


            for (int i = 0; i <= Floors; i++)
            {
                //start modifier
                var pStart = (Rows + 1) * i;
                m_Vertices[pStart].x = transform.position.x + m_XStartPos.x;
                m_Vertices[pStart].z = transform.position.z + m_XStartPos.z;
                //end modifier
                var pEnd = pStart + Rows;
                m_Vertices[pEnd].x = transform.position.x + m_XEndPos.x;
                m_Vertices[pEnd].z = transform.position.z + m_XEndPos.z;
                
                //calculate in between start and end rows
                for (int r = 0; r < Rows; r++)
                {
                    if (r == pStart || r == pEnd)
                    continue;
                    

                    //find where we are in the rows
                    float f = ((float)r / (float)Rows); //cast is not redundant contrary to what VS says

                    //affect other rows
                    m_Vertices[r + pStart].x = Mathf.Lerp(m_Vertices[pStart].x, m_Vertices[pEnd].x, f * 1f);
                    m_Vertices[r + pStart].z = Mathf.Lerp(m_Vertices[pStart].z, m_Vertices[pEnd].z, f * 1f);

                }

            }
            
            mesh.vertices = m_Vertices;

            //does not take transform position of root object into account
            int[] triangles = new int[xSize * ySize * 6];
            for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
            {
                for (int x = 0; x < xSize; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                    triangles[ti + 5] = vi + xSize + 2;
                }

            }

            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            
        }
        

        public void CreatePrefabArray()
        {
            ClearPrefabArray();
            //EditPrefabArray();
        }

        private void UpdatePrefabArray() {
            ClearPrefabArray();

            //if (FacadePrefab == null)
                //return;

            var t = Rows * Floors;
            for (var r = 0; r < t; r++)
            {
                if (m_ArrayEmpty)
                {
                    var go = Instantiate((Resources.Load("BuildingBlocks/FacadePrefab", typeof(GameObject))) as GameObject, new Vector3(1 * 2.0f, 0, 0), Quaternion.identity);



                    go.transform.SetParent(transform);
                    go.name = "instantiated wall facade prefab";
                    var goMR = go.transform.GetComponent<MeshRenderer>();
                    var goMF = go.transform.GetComponent<MeshFilter>();
                    RowBounds_Readonly = goMR.bounds.size;
                    Mesh meshCopy = Mesh.Instantiate(goMF.sharedMesh) as Mesh;
                    goMF.mesh = meshCopy;

                    var origVerts = new Vector3[meshCopy.vertices.Length];
                    var instanceVerts = new Vector3[meshCopy.vertices.Length];
                    origVerts = meshCopy.vertices;
                    instanceVerts = meshCopy.vertices;

                    //var chunkvertices = new Vector3[_chunkVertices.Length];
                    for (var i = 0; i < instanceVerts.Length; i++)
                    {
                        RowSize.x = (chunkWidth / RowBounds_Readonly.x) * 1.0f;//2017 was 1.2
                        RowSize.y = floorHeight / RowBounds_Readonly.y;

                        var vertex = instanceVerts[i];
                        var refVertex = origVerts[i];
                        vertex.x = refVertex.x * RowSize.x;
                        vertex.y = refVertex.y * RowSize.y;
                        vertex.z = refVertex.z * RowSize.z;
                        instanceVerts[i] = vertex;
                        //chunkvertices[i] = originalVertices[i] * scale;
                    }
                    goMF.sharedMesh.vertices = instanceVerts;

                    goMF.sharedMesh.RecalculateNormals();
                    goMF.sharedMesh.RecalculateBounds();

                    go.transform.rotation = Quaternion.LookRotation(mesh.normals[0]);

                    ////////////////

                    //start modifier
                    var pStart = (Rows + 1) * r;
                    //end modifier
                    var pEnd = pStart + Rows;
                    
                    var r2 = r + 1;
                    var variable = Rows + 1;

                    var vert1 = r;
                    var vert2 = r + 1;


                    go.transform.position = (mesh.vertices[r] + mesh.vertices[r2]) / 2;
                    
                    

                    BoxCollider bc = go.AddComponent(typeof(BoxCollider)) as BoxCollider;

                    RowBoundsPost_Readonly = goMR.bounds.size;

                    prefabArray[r] = go;
                    
                }
            }

            sortVerts = new int[m_Vertices.Length];
            sortedVerts = new List<int>(m_Vertices.Length);
            for (int i = 0; i < m_Vertices.Length; i++)
            {
                sortVerts[i] = i;
            }
            for (int i = 0; i < m_Vertices.Length; i++)
            {
                for (int x = 0; x < m_Vertices.Length; x++)
                {
                    var pStart = (Rows + 1) * i;
                    var pEnd = pStart + Rows;
                    var pPoint = 1 * i;
                    if (x.CompareTo(pEnd) == 0)
                    {
                        sortVerts[x] = -1;
                    }
                }
            }
            for (int x = 0; x < sortVerts.Length; x++)
            {
                sortedVerts.Add(sortVerts[x]);
            }

            for (int i = sortedVerts.Count - 1; i >= 0; i--)
            {
                if (sortedVerts[i].CompareTo(-1) == 0)
                    sortedVerts.RemoveAt(i);
            }
            var chopExcess = sortedVerts.Count - prefabArray.Length;

            for (int i = sortedVerts.Count - 1; i >= prefabArray.Length; i--)
            {
                sortedVerts.RemoveAt(i);
            }

            for (int i = 0; i < prefabArray.Length; i++)
            {
                var vert1 = sortedVerts[i];
                var vert2 = sortedVerts[i] + 1;

                prefabArray[i].transform.position = (mesh.vertices[vert1] + mesh.vertices[vert2]) / 2;
                
            }
            
            m_ArrayEmpty = false;

        }

        private void EditPrefabArray()
        {
            //prefabArray;
            if (m_ArrayEmpty)
            {
                //if (FacadePrefab == null)
                    //return;

                var go = Instantiate((Resources.Load("FacadePrefab", typeof(GameObject))) as GameObject, new Vector3(1 * 2.0f, 0, 0), Quaternion.identity);
                go.transform.SetParent(transform);
                go.name = "instantiated wall facade prefab";
                var goMR = go.transform.GetComponent<MeshRenderer>();
                var goMF = go.transform.GetComponent<MeshFilter>();
                RowBounds_Readonly = goMR.bounds.size;
                Mesh meshCopy = Mesh.Instantiate(goMF.sharedMesh) as Mesh;
                goMF.mesh = meshCopy;
                
                var origVerts = new Vector3[meshCopy.vertices.Length];
                var instanceVerts = new Vector3[meshCopy.vertices.Length];
                origVerts = meshCopy.vertices;
                instanceVerts = meshCopy.vertices;
                
                for (var i = 0; i < instanceVerts.Length; i++)
                {
                    RowSize.x = (chunkWidth / RowBounds_Readonly.x)*1.0f;//2017 was 1.2
                    RowSize.y = floorHeight / RowBounds_Readonly.y;

                    var vertex = instanceVerts[i];
                    var refVertex = origVerts[i];
                    vertex.x = refVertex.x * RowSize.x;
                    vertex.y = refVertex.y * RowSize.y;
                    vertex.z = refVertex.z * RowSize.z;
                    instanceVerts[i] = vertex;
                }
                goMF.sharedMesh.vertices = instanceVerts;

                goMF.sharedMesh.RecalculateNormals();
                goMF.sharedMesh.RecalculateBounds();

                go.transform.rotation = Quaternion.LookRotation(mesh.normals[0]);
                go.transform.position = (mesh.vertices[0] + mesh.vertices[1]) / 2;
                BoxCollider bc = go.AddComponent(typeof(BoxCollider)) as BoxCollider;
                RowBoundsPost_Readonly = goMR.bounds.size;

                prefabArray[0] = go;


                m_ArrayEmpty = false;
            }

        }

        public void ClearPrefabArray()
        {

            //old
            for (int i = 0; i < prefabArray.Length; i++)
            {
                if (prefabArray.Length < 1)
                    break;
                if (prefabArray[i] != null)
                    DestroyImmediate(prefabArray[i].gameObject); 
            }
            m_ArrayEmpty = true;

            prefabArray = new GameObject[Rows*Floors];

            //new
            for (int i = 0; i < BlockArray.Length; i++)
            {
                if (BlockArray.Length < 1)
                    break;
                if (BlockArray[i] != null)
                    DestroyImmediate(BlockArray[i].gameObject);
            }
            m_ArrayEmpty = true;
            BlockArray = new Block[Rows * Floors];

        }

        private void OnDrawGizmos()
        {
            
            Gizmos.color = Color.green;

            if (m_Vertices == null)
                return;

            for (int i = 0; i < m_Vertices.Length; i++)
            {
                Gizmos.DrawSphere(m_Vertices[i], 0.025f);
            }
        }
        //Helper functions

        public static Vector3 GetPoint(Vector3 WallVectorStartPoint, Vector3 WallVectorEndPoint, Vector3 p2, float t)
        {
            return Vector3.Lerp(Vector3.Lerp(WallVectorStartPoint, WallVectorEndPoint, t), Vector3.Lerp(WallVectorEndPoint, p2, t), t);
        }

        public static float GetPointF(float WallVectorStartPoint, float WallVectorEndPoint, float p2, float t)
        {
            return Mathf.Lerp(Mathf.Lerp(WallVectorStartPoint, WallVectorEndPoint, t), Mathf.Lerp(WallVectorEndPoint, p2, t), t);
        }

        public bool ArrayIsEmpty(object[] arr, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (arr[i] != null)
                    return false;
            }
            return true;
        }
    }
}