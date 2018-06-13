using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Structura
{
    [ExecuteInEditMode]
    public class Wall : MonoBehaviour 
	{
        [LargeHeader("Wall functions")]
        public bool m_Initialised;
        public bool m_AutoResize = true;
        public Floor m_Floor;
        //public int m_PendingWalls;
        public List<Transform> m_Walls;//the subwall transforms
        public List<SubWall> m_SubWall;
        public List<Vector3> m_WallStartPos; 
        public List<Vector3> m_WallEndPos;
        
        public int Rows = 2;
        public int Floors = 2;

        public float DefaultFloorHeight = 1;
        public float DefaultRowWidth = 1; //this gets changed, more read only sorta old code
        public float DefaultRowDepth = 1;


        #region old vars



        public int WallNumber;
        
        public Vector3 WallVectorStartPoint;
        public Vector3 WallVectorEndPoint;
        private Vector3 m_XStartPos; //modified by wallvector points
        private Vector3 m_XEndPos;
        private Vector3[] m_Vertices;
        private Mesh mesh;
        
        public float m_EditorHandleDistance;

        public bool m_ChunkModelInit;
        public GameObject FacadePrefab;
        public Vector3 RowSize;
        public Vector3 RowBounds_Readonly;
        public Vector3 RowBoundsPost_Readonly;
        public float scale = 1;
        private int[] sortVerts;
        private List<int> sortedVerts;
        //old code
        public GameObject[] prefabArray;
        public bool m_ArrayEmpty;

        //Replacement for prefab usage
        public Block[] BlockArray;


        public MeshRenderer meshRenderer;
        public MeshFilter meshFilter;
#endregion
        public void Start()
        {
            m_Initialised = true;
            //Initialise the lists
            m_Walls = new List<Transform>();
            m_WallStartPos = new List<Vector3>();
            m_WallEndPos = new List<Vector3>();
            m_SubWall = new List<SubWall>();

            WallVectorEndPoint = new Vector3(1, 0, 0);
            RowSize = new Vector3(0f, 0f, 0.5f);

            if (m_Floor == null)
            {
                //m_Floor = gameObject.AddComponent<Floor>() as Floor;
                m_Floor = transform.GetComponent<Floor>();
            }
            //Create a wall for each floor point
            for (int w = 0; w < m_Floor.m_Points.Count; w++)
            {
                GameObject g = new GameObject();
                g.transform.SetParent(transform);
                g.name = "StructuraWall";
                m_Walls.Add(g.transform);

                //Get positions of the floor and set them into our list for the wall start and end points
                m_WallStartPos.Add(m_Floor.m_Points[w]);
                var PartnerPoint = m_Floor.GetPartnerPoint(m_Floor.m_Points, w);
                m_WallEndPos.Add(m_Floor.m_Points[PartnerPoint]);

                //Add subwall component and add it to the list, easier than always getting component from transform of subwalls list
                var SUBWALL = g.AddComponent<SubWall>() as SubWall;
                m_SubWall.Add(SUBWALL);
                SUBWALL.m_Vertices = new List<Vector3>();

                //Pass the index points so we can access floor point handles from the sub script
                m_SubWall[w].m_Wall = this;
                m_SubWall[w].m_Floor = m_Floor;
                m_SubWall[w].WallIndexStart = w;
                m_SubWall[w].WallIndexEnd = PartnerPoint;

                //Add mesh renderers and filters for each child
                var subWallMeshRenderer = g.AddComponent<MeshRenderer>() as MeshRenderer;
                subWallMeshRenderer.material = Resources.Load("BuildingBlocks/TransparentBuildingMaterial", typeof(Material)) as Material;
                var subWallMeshFilter = g.AddComponent<MeshFilter>() as MeshFilter;

                GenerateSubWall(w); //make the wall mesh for each subtransform
            }

            //Replacement allows for changing prefab
            BlockArray = new Block[Rows * Floors];
            // old code
            FacadePrefab = Resources.Load("BuildingBlocks/FacadePrefab", typeof(GameObject)) as GameObject;
            prefabArray = new GameObject[Rows * Floors]; 

        }

        public void Update()
        {
            WallUpdater();
        }



        private void OnEditorHelpers()
        {
            //Uh what does this do?
            //Start helper position
            //var mS = (Rows + 1) * 0;
            //End helper position
            //var mE = mS + Rows;
            //delete soon oldcode
            m_XStartPos.x = WallVectorStartPoint.x;
            m_XStartPos.y = WallVectorStartPoint.y;
            m_XStartPos.z = WallVectorStartPoint.z;
            m_XEndPos.x = WallVectorEndPoint.x;
            m_XEndPos.y = WallVectorEndPoint.y;
            m_XEndPos.z = WallVectorEndPoint.z;

            m_EditorHandleDistance = Vector3.Distance(WallVectorStartPoint, WallVectorEndPoint);
        }

        //Creates the subwall
        private void GenerateSubWall(int t) //t is what index we are for the relevant lists
        {
            /*
            m_Walls = new List<Transform>();
            m_WallStartPos = new List<Vector3>();
            m_WallEndPos = new List<Vector3>();
            */
            m_SubWall[t].Rows = Rows;
            m_SubWall[t].Floors = Floors;

            var SUBMESH = m_Walls[t].GetComponent<MeshFilter>().sharedMesh;
            SUBMESH = new Mesh();
            m_Walls[t].GetComponent<MeshFilter>().mesh = SUBMESH;

            var NAMEMODIFIER = t.ToString();
            var NAME = "Wall_";
            SUBMESH.name = NAME+NAMEMODIFIER;
            m_Walls[t].name = NAME + NAMEMODIFIER;
            // rearranging m_SubWall[t].m_Vertices = new List<Vector3>();

            
            var EDITORHANDLEDISTANCE = Vector3.Distance(m_WallStartPos[t], m_WallEndPos[t]);
            //old
            DefaultRowWidth = EDITORHANDLEDISTANCE / Rows;

            //old array
            //m_Vertices = new Vector3[(Rows + 1) * (Floors + 1)];

            m_SubWall[t].m_Vertices = new List<Vector3>();
            for (int i = 0, y = 0; y <= Floors; y++)
            {
                for (int x = 0; x <= Rows; x++, i++)
                {
                    m_SubWall[t].m_Vertices.Add(new Vector3(x * DefaultRowWidth, y * DefaultFloorHeight));
                    //m_Vertices[i] = new Vector3(x * DefaultRowWidth, y * DefaultFloorHeight);
                }
            }

            //I think this loop places the start and end vertices 
            for (int i = 0; i <= Floors; i++)
            {
                //Starting Vector3 position Index
                var pStart = (Rows + 1) * i;

                //todo replace all xstartpos and xendpos
                var START = m_WallStartPos[t];
                var END = m_WallEndPos[t];
                //Cant access variables directly so get it, modify in tempvar, replace entirely with new variable
                var TEMP_REPLACE_X = m_SubWall[t].m_Vertices[pStart];
                TEMP_REPLACE_X.x = transform.position.x + START.x;
                m_SubWall[t].m_Vertices[pStart] = TEMP_REPLACE_X;
                
                var TEMP_REPLACE_Z = m_SubWall[t].m_Vertices[pStart];
                TEMP_REPLACE_Z.z = transform.position.z + START.z;
                m_SubWall[t].m_Vertices[pStart] = TEMP_REPLACE_Z;

                //old
                //m_Vertices[pStart].x = transform.position.x + START.x;
                //m_Vertices[pStart].z = transform.position.z + START.z;

                //Ending Vector3 position Index
                var pEnd = pStart + Rows;

                TEMP_REPLACE_X = m_SubWall[t].m_Vertices[pEnd];
                TEMP_REPLACE_X.x = transform.position.x + END.x;
                m_SubWall[t].m_Vertices[pEnd] = TEMP_REPLACE_X;

                TEMP_REPLACE_Z = m_SubWall[t].m_Vertices[pEnd];
                TEMP_REPLACE_Z.z = transform.position.z + END.z;
                m_SubWall[t].m_Vertices[pEnd] = TEMP_REPLACE_Z;

                //old
                //m_Vertices[pEnd].x = transform.position.x + END.x;
                //m_Vertices[pEnd].z = transform.position.z + END.z;

                //This lerps the position of other vertices in line with the start and end points
                //calculate in between start and end rows
                for (int r = 0; r < Rows; r++)
                {
                    if (r == pStart || r == pEnd)
                        continue;


                    //find where we are in the rows
                    float f = ((float)r / (float)Rows); //cast is not redundant contrary to what VS says
                    
                    //affect other rows
                    TEMP_REPLACE_X = m_SubWall[t].m_Vertices[r + pStart];
                    TEMP_REPLACE_X.x = Mathf.Lerp(m_SubWall[t].m_Vertices[pStart].x, m_SubWall[t].m_Vertices[pEnd].x, f * 1f);
                    m_SubWall[t].m_Vertices[r + pStart] = TEMP_REPLACE_X;
                    TEMP_REPLACE_Z = m_SubWall[t].m_Vertices[r + pStart];
                    TEMP_REPLACE_Z.z = Mathf.Lerp(m_SubWall[t].m_Vertices[pStart].z, m_SubWall[t].m_Vertices[pEnd].z, f * 1f);
                    m_SubWall[t].m_Vertices[r + pStart] = TEMP_REPLACE_Z;

                    //old
                    //m_Vertices[r + pStart].x = Mathf.Lerp(m_Vertices[pStart].x, m_Vertices[pEnd].x, f * 1f);
                    //m_Vertices[r + pStart].z = Mathf.Lerp(m_Vertices[pStart].z, m_Vertices[pEnd].z, f * 1f);

                }

            }

            //Debug.Log(triangles.Length);
            SUBMESH.vertices = m_SubWall[t].m_Vertices.ToArray();
            //SUBMESH.vertices = m_Vertices;
            
            //does not take transform position of root object into account
            int[] triangles = new int[Rows * Floors * 6];
            for (int ti = 0, vi = 0, y = 0; y < Floors; y++, vi++)
            {
                for (int x = 0; x < Rows; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + Rows + 1;
                    triangles[ti + 5] = vi + Rows + 2;
                }

            }
            //Debug.Log(triangles.Length);
            SUBMESH.triangles = triangles;
            SUBMESH.RecalculateNormals();

            m_SubWall[t].m_Mesh = SUBMESH; //cache the above
        }

        //Note this is called in StructuraCoreInspector editor handles section
        public void WallUpdater()
        {
            if (!m_AutoResize)
            {
                return;
            }
            for (int w = 0; w < m_Floor.m_Points.Count; w++)
            {
                //Get positions of the floor and set them into our list for the wall start and end points
                m_WallStartPos[w] = m_Floor.m_Points[w];
                var PartnerPoint = m_Floor.GetPartnerPoint(m_Floor.m_Points, w);
                m_WallEndPos[w] = m_Floor.m_Points[PartnerPoint];
            }
            for (int i = 0; i< m_Walls.Count; i++)
            {
                UpdateWall(i);
                //UpdatePrefabArray(i);
            }
        }



        private void UpdateWall(int t) //t is what index we are for the relevant lists
        {
            //Check For Rows And Floors Changes
            //Make a new list, apply the rows/floors formula to check vertice total of what is currently requested and what we actually have

            var ROWSX = 0; var FLOORSY = 0;

            //IF the subwall is locked, read the rows/floors from there directly. Otherwise just go off of the main script
            if (!m_SubWall[t].Locked)
            {
                ROWSX = Rows;
                FLOORSY = Floors;

                //Might be redundant but update just to keep up appearances
                m_SubWall[t].Rows = Rows;
                m_SubWall[t].Floors = Floors;
            }
            if (m_SubWall[t].Locked)
            {
                ROWSX = m_SubWall[t].Rows;
                FLOORSY = m_SubWall[t].Floors;
            }



          
            var VERTCHECK = new List<int>();
            for (int i = 0, y = 0; y <= FLOORSY; y++)
            {
                for (int x = 0; x <= ROWSX; x++, i++)
                {
                    int DUMMY = 0;
                    VERTCHECK.Add(DUMMY);
                    //m_SubWall[t].m_Vertices.Add(new Vector3(x * DefaultRowWidth, y * DefaultFloorHeight));
                }
            }



            if (m_SubWall[t].m_Vertices.Count != VERTCHECK.Count)
            {
                Debug.Log("no match");
                m_SubWall[t].m_Vertices = new List<Vector3>();
                for (int i = 0, y = 0; y <= FLOORSY; y++)
                {
                    for (int x = 0; x <= ROWSX; x++, i++)
                    {
                        m_SubWall[t].m_Vertices.Add(new Vector3(x * DefaultRowWidth, y * DefaultFloorHeight));
                        //m_Vertices[i] = new Vector3(x * DefaultRowWidth, y * DefaultFloorHeight);
                    }
                }

            }



            //Rescale x and y and z, for now only y works
            for (int i = 0, y = 0; y <= FLOORSY; y++)
            {
                for (int x = 0; x <= ROWSX; x++, i++)
                {
                    Vector3 REPLACEMENT = m_SubWall[t].m_Vertices[i];
                    REPLACEMENT.x = x * DefaultRowWidth;
                    REPLACEMENT.y = y * DefaultFloorHeight;
                    REPLACEMENT.z = 1 * DefaultRowDepth;
                    m_SubWall[t].m_Vertices[i] = REPLACEMENT;
                    //m_SubWall[t].m_Vertices.Add(new Vector3(x * DefaultRowWidth, y * DefaultFloorHeight));
                }
            }

            if (m_SubWall[t].m_Vertices.Count < VERTCHECK.Count)
            {
                m_SubWall[t].m_Vertices.Add(new Vector3(DefaultRowWidth, DefaultFloorHeight));
                return;
            }
            if (m_SubWall[t].m_Vertices.Count > VERTCHECK.Count)
            {
                var segmentToRemove = (m_SubWall[t].m_Vertices.Count - 1);
                m_SubWall[t].m_Vertices.Remove(m_SubWall[t].m_Vertices[segmentToRemove]);
                return;
            }

    

            //
            // Update wall vertice positions
            //
            var SUBMESH = m_SubWall[t].m_Mesh;

            //I think this loop places the start and end vertices 
            for (int i = 0; i <= FLOORSY; i++)
            {
                //Starting Vector3 position Index
                var pStart = (ROWSX + 1) * i;

                //todo replace all xstartpos and xendpos
                var START = m_WallStartPos[t];
                var END = m_WallEndPos[t];
                //Cant access variables directly so get it, modify in tempvar, replace entirely with new variable
                var TEMP_REPLACE_X = m_SubWall[t].m_Vertices[pStart];
                TEMP_REPLACE_X.x = transform.position.x + START.x;
                m_SubWall[t].m_Vertices[pStart] = TEMP_REPLACE_X;

                var TEMP_REPLACE_Z = m_SubWall[t].m_Vertices[pStart];
                TEMP_REPLACE_Z.z = transform.position.z + START.z;
                m_SubWall[t].m_Vertices[pStart] = TEMP_REPLACE_Z;
                
                //Ending Vector3 position Index
                var pEnd = pStart + ROWSX;

                TEMP_REPLACE_X = m_SubWall[t].m_Vertices[pEnd];
                TEMP_REPLACE_X.x = transform.position.x + END.x;
                m_SubWall[t].m_Vertices[pEnd] = TEMP_REPLACE_X;

                TEMP_REPLACE_Z = m_SubWall[t].m_Vertices[pEnd];
                TEMP_REPLACE_Z.z = transform.position.z + END.z;
                m_SubWall[t].m_Vertices[pEnd] = TEMP_REPLACE_Z;
                
                //This lerps the position of other vertices in line with the start and end points
                //calculate in between start and end rows
                for (int r = 0; r < ROWSX; r++)
                {
                    if (r == pStart || r == pEnd)
                        continue;


                    //find where we are in the rows
                    float f = ((float)r / (float)ROWSX); //cast is not redundant contrary to what VS says

                    //affect other rows
                    TEMP_REPLACE_X = m_SubWall[t].m_Vertices[r + pStart];
                    TEMP_REPLACE_X.x = Mathf.Lerp(m_SubWall[t].m_Vertices[pStart].x, m_SubWall[t].m_Vertices[pEnd].x, f * 1f);
                    m_SubWall[t].m_Vertices[r + pStart] = TEMP_REPLACE_X;
                    TEMP_REPLACE_Z = m_SubWall[t].m_Vertices[r + pStart];
                    TEMP_REPLACE_Z.z = Mathf.Lerp(m_SubWall[t].m_Vertices[pStart].z, m_SubWall[t].m_Vertices[pEnd].z, f * 1f);
                    m_SubWall[t].m_Vertices[r + pStart] = TEMP_REPLACE_Z;
                    

                }

            }
            
            SUBMESH.vertices = m_SubWall[t].m_Vertices.ToArray();

            //does not take transform position of root object into account
            int[] triangles = new int[ROWSX * FLOORSY * 6];
            for (int ti = 0, vi = 0, y = 0; y < FLOORSY; y++, vi++)
            {
                for (int x = 0; x < ROWSX; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + ROWSX + 1;
                    triangles[ti + 5] = vi + ROWSX + 2;
                }

            }

            SUBMESH.triangles = triangles;
            SUBMESH.RecalculateNormals();

        }
        
        public void UpdatePrefabArray(int t) {

            //old
            for (int i = 0; i < prefabArray.Length; i++)
            {
                if (prefabArray.Length < 1)
                    break;
                if (prefabArray[i] != null)
                    DestroyImmediate(prefabArray[i].gameObject);
            }
            m_ArrayEmpty = true;

            prefabArray = new GameObject[Rows * Floors];


            var ROWSX = 0; var FLOORSY = 0;
            var ISLISTEMPTY = false;

            //IF the subwall is locked, read the rows/floors from there directly. Otherwise just go off of the main script
            if (!m_SubWall[t].Locked)
            {
                ROWSX = Rows;
                FLOORSY = Floors;

                //Update just to keep up appearances / but it doesnt actually affect since locked
                m_SubWall[t].Rows = Rows;
                m_SubWall[t].Floors = Floors;
            }
            if (m_SubWall[t].Locked)
            {
                ROWSX = m_SubWall[t].Rows;
                FLOORSY = m_SubWall[t].Floors;
            }

            //Create list if it doesnt exist, will need code for clearing it upon reset
            if (m_SubWall[t].m_Prefabs == null)
                m_SubWall[t].m_Prefabs = new List<GameObject>();

            //Check if we need to instantiate prefabs
            if (m_SubWall[t].m_Prefabs.Count <= 0)
            {
                ISLISTEMPTY = true;
            }
            else
            {
                ISLISTEMPTY = false;
            }

            var TOTALPREFABS = ROWSX * FLOORSY;
            for (var r = 0; r < TOTALPREFABS; r++)
            {
                if (ISLISTEMPTY || (ISLISTEMPTY && !m_SubWall[t].Locked))
                {
                    var go = Instantiate((Resources.Load("BuildingBlocks/FacadePrefab", typeof(GameObject))) as GameObject, new Vector3(1 * 2.0f, 0, 0), Quaternion.identity);

                    go.transform.SetParent(m_SubWall[t].transform);
                    go.name = "Prefab";
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
                        RowSize.x = (DefaultRowWidth / RowBounds_Readonly.x) * 1.0f;//2017 was 1.2
                        RowSize.y = DefaultFloorHeight / RowBounds_Readonly.y;
                        RowSize.z = DefaultRowDepth;

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

                    go.transform.rotation = Quaternion.LookRotation(m_SubWall[t].m_Mesh.normals[0]);

                    ////////////////

                    //start modifier
                    var pStart = (ROWSX + 1) * r;
                    //end modifier
                    var pEnd = pStart + ROWSX;

                    var r2 = r + 1;
                    var variable = ROWSX + 1;

                    var vert1 = r;
                    var vert2 = r + 1;


                    go.transform.position = (m_SubWall[t].m_Mesh.vertices[r] + m_SubWall[t].m_Mesh.vertices[r2]) / 2;



                    BoxCollider bc = go.AddComponent(typeof(BoxCollider)) as BoxCollider;

                    RowBoundsPost_Readonly = goMR.bounds.size;

                    m_SubWall[t].m_Prefabs.Add(go);
                    //prefabArray[r] = go;

                }
                
            }

            if (!ISLISTEMPTY)
            {
                for (int p = 0; p < m_SubWall[t].m_Prefabs.Count; p++)
                {
                    m_SubWall[t].m_Prefabs[p].transform.rotation = Quaternion.LookRotation(m_SubWall[t].m_Mesh.normals[0]);
                }
            }


            if (!m_SubWall[t].Locked)
            {
                for (int p = 0; p < m_SubWall[t].m_Prefabs.Count; p++)
                {
                    //RowSize.z = DefaultRowDepth;
                    //set width and height
                    //m_SubWall[t].m_Prefabs[p].transform.rotation = Quaternion.LookRotation(m_SubWall[t].m_Mesh.normals[0]);
                }
            }


            var SORTVERTICES = new List<int>();

            //old
            //sortVerts = new int[m_Vertices.Length];
            sortedVerts = new List<int>(m_SubWall[t].m_Vertices.Count);
            for (int i = 0; i < m_SubWall[t].m_Vertices.Count; i++)
            {
                //sortVerts[i] = i;
                SORTVERTICES.Add(i);
            }
            for (int i = 0; i < m_SubWall[t].m_Vertices.Count; i++)
            {
                for (int x = 0; x < m_SubWall[t].m_Vertices.Count; x++)
                {
                    var pStart = (ROWSX + 1) * i;
                    var pEnd = pStart + ROWSX;
                    var pPoint = 1 * i;
                    if (x.CompareTo(pEnd) == 0)
                    {
                        //sortVerts[x] = -1;
                        SORTVERTICES[x] = -1;
                    }
                }
            }

            for (int x = 0; x < SORTVERTICES.Count; x++)
            {
                sortedVerts.Add(SORTVERTICES[x]);
            }


            for (int i = sortedVerts.Count - 1; i >= 0; i--)
            {
                if (sortedVerts[i].CompareTo(-1) == 0)
                    sortedVerts.RemoveAt(i);
            }
            var chopExcess = sortedVerts.Count - m_SubWall[t].m_Prefabs.Count;

            for (int i = sortedVerts.Count - 1; i >= m_SubWall[t].m_Prefabs.Count; i--)
            {
                sortedVerts.RemoveAt(i);
            }

            for (int i = 0; i < m_SubWall[t].m_Prefabs.Count; i++)
            {
                var vert1 = sortedVerts[i];
                var vert2 = sortedVerts[i] + 1;

                m_SubWall[t].m_Prefabs[i].transform.position = (m_SubWall[t].m_Mesh.vertices[vert1] + m_SubWall[t].m_Mesh.vertices[vert2]) / 2;
                
            }
            
            m_ArrayEmpty = false;

        }

        private void OLDUpdatePrefabArray()
        {
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
                        RowSize.x = (DefaultRowWidth / RowBounds_Readonly.x) * 1.0f;//2017 was 1.2
                        RowSize.y = DefaultFloorHeight / RowBounds_Readonly.y;

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
                    RowSize.x = (DefaultRowWidth / RowBounds_Readonly.x)*1.0f;//2017 was 1.2
                    RowSize.y = DefaultFloorHeight / RowBounds_Readonly.y;

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

        public void Reset()
        {
            m_Walls = null;
            m_WallStartPos = null;
            m_WallEndPos = null;
            m_Floor = null;
            meshRenderer = null;
            meshFilter = null;
            m_SubWall = null;

            var children = new List<GameObject>();
            foreach (Transform child in transform) children.Add(child.gameObject);
            children.ForEach(child => DestroyImmediate(child));
            m_Initialised = false;
            Start();
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

        //unused but might be useful later
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