using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjSpray : MonoBehaviour {
    [Header("Transform")]
    public Vector3 rayPos;
    [Space]
    public Vector3 emptyDebug;
    public const int ARCTIC = 8;
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(rayPos, 0.1f);
            Gizmos.color = Color.blue;
            //Gizmos.DrawLine(m_Floor.m_Points[i], m_Floor.m_Points[m_Floor.GetPartnerPoint(m_Floor.m_Points, i)]);
        
    }
}
