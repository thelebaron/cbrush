using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MeshInstances : MonoBehaviour {

    public Mesh m_Mesh;
    public Material m_Material;
    public Matrix4x4[] m_Matrix4;
    public MaterialPropertyBlock m_MaterialPropertyBlock;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        Graphics.DrawMeshInstanced(m_Mesh, 0, m_Material, m_Matrix4, 1, m_MaterialPropertyBlock, ShadowCastingMode.On, true, 0);

    }
}
