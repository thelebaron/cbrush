using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PositionRecorder : MonoBehaviour
{

    public List<Vector3> m_RecordedPosition;
    public bool m_ShowGizmos;

    void Awake ()
	{
        m_RecordedPosition = new List<Vector3>();

    }
		
	void Start ()
	{
        InvokeRepeating("StorePos", 0.25f, 0.25f);
	}
    
    void StorePos() {

        m_RecordedPosition.Add(transform.position);

    }

    private void OnDrawGizmos()
    {

        if (m_ShowGizmos)
        {

            for (int i = 0; i < m_RecordedPosition.Count; i++)
            {
                var from = i;
                var to = m_RecordedPosition.Count;


                Gizmos.DrawLine(m_RecordedPosition[i], m_RecordedPosition[GetNextSequence(i, m_RecordedPosition)]);
            }
        }
    }

    public int GetNextSequence(int i, List<Vector3> list)
    {
        var to = i + 1;
        if (to > list.Count)
        {
            to = 0;
        }

        return to;
    }
}

