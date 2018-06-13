// C#
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TransformSceneHelper : MonoBehaviour
{
    public Vector3 StoredPosition;
    public bool LockEnabled;

    void Update()
    {

#if UNITY_EDITOR

        if(LockEnabled)
            transform.position = StoredPosition;
#endif

    }
}