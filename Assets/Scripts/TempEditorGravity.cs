using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TempEditorGravity : MonoBehaviour
{
    public int seconds = 5;
    public Rigidbody rb;

	void Awake ()
	{

	}


    void Start ()
	{
        rb = gameObject.AddComponent<Rigidbody>() as Rigidbody;

        Invoke("DelayDestroy", seconds);
    }

	void Update ()
	{
        Physics.gravity = new Vector3(0, -15.0F, 0);
        Debug.Log("tick");
        seconds -= 1;
    }

    void DelayDestroy()
    {
        Debug.Log("destroy");
        DestroyImmediate(rb);
        DestroyImmediate(this);
    }
}

