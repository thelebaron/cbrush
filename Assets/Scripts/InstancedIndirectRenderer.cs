using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstancedIndirectRenderer : MonoBehaviour
{

    public Material m_instancedIndirectMaterial;
    public Mesh m_cubeMesh;
    public int m_numMeshes;
    public Bounds m_bounds;

    private ComputeBuffer m_bufferWithArgs;
    private Vector3[] m_positionArray;
    private ComputeBuffer m_positionBuffer;

    void Start()
    {
        // Setup buffer with args
        uint indexCountPerInstance = m_cubeMesh.GetIndexCount(0);
        uint instanceCount = (uint)m_numMeshes;
        uint startIndexLocation = 0;
        uint baseVertexLocation = 0;
        uint startInstanceLocation = 0;
        uint[] args = new uint[] { indexCountPerInstance, instanceCount, startIndexLocation, baseVertexLocation, startInstanceLocation };

        m_bufferWithArgs = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        m_bufferWithArgs.SetData(args);

        // Setup positions
        m_positionArray = new Vector3[m_numMeshes];
        for (int i = 0; i < m_numMeshes; i++)
        {
            m_positionArray[i] = new Vector3(i, 0, 0) + transform.position;
        }
        m_positionBuffer = new ComputeBuffer(m_numMeshes, sizeof(float) * 3);
        m_positionBuffer.SetData(m_positionArray);
        m_instancedIndirectMaterial.SetBuffer("positions", m_positionBuffer);
    }


    void Update()
    {
        Graphics.DrawMeshInstancedIndirect(m_cubeMesh, 0, m_instancedIndirectMaterial, m_bounds, m_bufferWithArgs, 0, null, UnityEngine.Rendering.ShadowCastingMode.On, true, 0, Camera.main);
    }

    void OnDestroy()
    {
        m_bufferWithArgs.Dispose();
        m_bufferWithArgs = null;

        m_positionBuffer.Dispose();
        m_positionBuffer = null;
    }
}
