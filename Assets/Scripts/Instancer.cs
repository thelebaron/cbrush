using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Instancer : MonoBehaviour {

    public int xSize;
    public int ySize;
    private Vector3[] vertices;
    public int instances;

    public GameObject prefab;

    public bool scaTTER;
    public float xScatterSize = 5f;
    public float yScatterSize = 0.5f;
    public float zScatterSize = 5f;
    public int scatterCount = 12;
    public int scatterRepititions = 1;

	// Use this for initialization
	void Init () {

        vertices = new Vector3[(xSize + 1) * (ySize + 1)];

    }

    void CountInstances()
    {
        //instances = 0;
        instances = transform.childCount;
        /*
        foreach(Transform tr in transform)
        {
            instances++;
        }*/
    }

    void ScatterPrefab()
    {
        for (int i = 0; i < scatterRepititions; i++)
        { 
            for (int j = 0; j < scatterCount; j++)
            {
                var pos = transform.position + new Vector3(Random.Range(-xScatterSize, xScatterSize), Random.Range(-yScatterSize, yScatterSize), Random.Range(-zScatterSize, zScatterSize));
                var newinstance = Instantiate(prefab, pos, transform.rotation);
                newinstance.transform.SetParent(transform);
            }
        }
    }

    void Update () {
        CountInstances();
        if (scaTTER)
        {
            scaTTER = false;
            ScatterPrefab();

        }
    }

    private void OnDrawGizmos()
    {
        var size = new Vector3(0.5f, 0.5f, 0.5f);
        Gizmos.color = Color.yellow;

        if (vertices == null)
            return;

        for (int i = 0; i < vertices.Length; i++)
        {
            //Gizmos.DrawCube(vertices[i], size);
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}
