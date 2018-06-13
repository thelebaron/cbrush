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
    public class Grid : MonoBehaviour 
	{
       
        //Grid
        public int Rows = 2;
        public int Floors = 2;
        public int Sides = 4;
        public float floorHeight = 1;
        public float chunkWidth = 1;
        public int xSize, ySize; //make private
        public Vector3 p0;
        public Vector3 p1;
        public Vector3 xStartPos;
        public Vector3 xEndPos;
        private Vector3[] vertices;
        private Mesh mesh;

        public bool m_EditorHandleInit;
        public float m_EditorHandleDistance;

        public bool m_ChunkModelInit;
        public GameObject FacadePrefab;
        public MeshFilter facadeMesh;
        public MeshRenderer facadeMeshRenderer;
        public Vector3 RowSize;
        public Vector3 RowBounds_Readonly;
        public Vector3 RowBoundsPost_Readonly;
        public float scale = 1;
        private Vector3[] originalVertices;
        private Vector3[] _chunkVertices;
        public int[] sortVerts;
        public List<int> sortedVerts;

        public GameObject[] prefabArray;
        public bool ArrayEmpty;



        public void Update()
        {
            OnEditorHelpers();
            Generate(Sides);

            UpdatePrefabArray();
        }

        private void Generate(int sides)
        {


            ySize = Floors;
            xSize = Rows;
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            mesh.name = "Procedural Grid";

            chunkWidth = m_EditorHandleDistance / Rows;

            vertices = new Vector3[(xSize + 1) * (ySize + 1)];

            for (int i = 0, y = 0; y <= ySize; y++)
            {
                for (int x = 0; x <= xSize; x++, i++)
                {
                    vertices[i] = new Vector3(x * chunkWidth, y * floorHeight);
                }
            }


            for (int i = 0; i <= Floors; i++)
            {
                //start modifier
                var pStart = (Rows + 1) * i;
                vertices[pStart].x = transform.position.x + xStartPos.x;
                vertices[pStart].z = transform.position.z + xStartPos.z;
                //end modifier
                var pEnd = pStart + Rows;
                vertices[pEnd].x = transform.position.x + xEndPos.x;
                vertices[pEnd].z = transform.position.z + xEndPos.z;
                
                //calculate in between start and end rows
                for (int r = 0; r < Rows; r++)
                {
                    if (r == pStart || r == pEnd)
                    continue;
                    

                    //find where we are in the rows
                    float f = ((float)r / (float)Rows); //cast is not redundant contrary to what VS says

                    //affect other rows
                    vertices[r + pStart].x = Mathf.Lerp(vertices[pStart].x, vertices[pEnd].x, f * 1f);
                    vertices[r + pStart].z = Mathf.Lerp(vertices[pStart].z, vertices[pEnd].z, f * 1f);

                }

            }
            
            mesh.vertices = vertices;

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

            /*
            int[] tris = new int[6];

            tris[0] = 0;
            tris[1] = xSize + 1;
            tris[2] = 1;
            //second tri
            tris[3] = 1;
            tris[4] = xSize + 1;
            tris[5] = xSize + 2;
            */
            /*
             1 4----5
             | \\   |
             |  \\  |
             |   \\ |           
             0----2 3
             */
        }

        private void OnDrawGizmos()
        {
            //var size = new Vector3(0.5f, 0.5f, 0.5f);
            Gizmos.color = Color.yellow;

            if (vertices == null)
                return;

            for (int i = 0; i < vertices.Length; i++)
            {
                //Gizmos.DrawCube(vertices[i], size);
                Gizmos.DrawSphere(vertices[i], 0.1f);
            }
            /*
            if(m_EditorHandleStart != null)
                Gizmos.DrawCube(m_EditorHandleStart.position, size);
            if (m_EditorHandleEnd != null)
                Gizmos.DrawCube(m_EditorHandleEnd.position, size);
                */
        }

        private void OnEditorHelpers()
        {
            //Start helper position
            var mS = (Rows + 1) * 0;
            //vertices[mS].x = xStartPos.x;
            //vertices[mS].z = xStartPos.z;
            //End helper position
            //var mE = mS + Rows;

            xStartPos.x = p0.x;
            xStartPos.y = p0.y;
            xStartPos.z = p0.z;
            xEndPos.x = p1.x;
            xEndPos.y = p1.y;
            xEndPos.z = p1.z;
            //p1.y = 0;
            //p0.y = 0;

            m_EditorHandleDistance = Vector3.Distance(p0, p1);
        }

        public void CreatePrefabArray()
        {
            ClearPrefabArray();
            EditPrefabArray();
        }

        private void UpdatePrefabArray() {
            ClearPrefabArray();
            var t = Rows * Floors;
            for (var r = 0; r < t; r++)
            {
                if (ArrayEmpty)
                {
                    var go = Instantiate(FacadePrefab, new Vector3(1 * 2.0f, 0, 0), Quaternion.identity);
                    go.name = "instantiated grid facade prefab";
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
                        RowSize.x = (chunkWidth / RowBounds_Readonly.x) * 1.2f;
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

            sortVerts = new int[vertices.Length];
            sortedVerts = new List<int>(vertices.Length);
            for (int i = 0; i < vertices.Length; i++)
            {
                sortVerts[i] = i;
            }
            for (int i = 0; i < vertices.Length; i++)
            {
                for (int x = 0; x < vertices.Length; x++)
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


                //prefabArray[i].transform.position = (sortedVerts[i] + sortedVerts[(i2)]) / 2;
            }



            for (int i = 0, c = (Rows * Floors); i < vertices.Length; i++, c--)
            {
                //Debug.Log("C= " + c);
               // if (c ==0)
                //{ break; }

                var pStart = (Rows + 1) * i;
                var pEnd = pStart + Rows;

                //Debug.Log("uhhh"+pEnd);
                if (i == pEnd)
                {
                    //Debug.Log("uhhh");
                }
                if (i != pEnd)
                {
                    //Debug.Log(i);
                    //continue;
                }
                //Debug.Log(pEnd);

                //var pUnused = 

                var r2 = i + 1;
                var variable = Rows + 1;

                var vert1 = c;
                var vert2 = c + 1;

                //Debug.Log(i);
                //prefabArray[i].transform.position = (mesh.vertices[i] + mesh.vertices[r2]) / 2;

            }

            ArrayEmpty = false;

        }

        private void EditPrefabArray()
        {
            //prefabArray;
            if (ArrayEmpty)
            {
                var go = Instantiate(FacadePrefab, new Vector3(1 * 2.0f, 0, 0), Quaternion.identity);
                go.name = "instantiated grid facade prefab";
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
                    RowSize.x = (chunkWidth / RowBounds_Readonly.x)*1.2f ;
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
                go.transform.position = (mesh.vertices[0] + mesh.vertices[1]) / 2;
                BoxCollider bc = go.AddComponent(typeof(BoxCollider)) as BoxCollider;
                RowBoundsPost_Readonly = goMR.bounds.size;

                prefabArray[0] = go;


                ArrayEmpty = false;
            }

        }

        public void ClearPrefabArray()
        {
            for (int i = 0; i < prefabArray.Length; i++)
            {
                if(prefabArray[i]!=null)
                    DestroyImmediate(prefabArray[i].gameObject);
            }
            ArrayEmpty = true;

            prefabArray = new GameObject[Rows*Floors];
        }

 

        //Helper functions

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