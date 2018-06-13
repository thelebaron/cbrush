using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Canary
{
    [ExecuteInEditMode]
	public class DEBUG_archhits : MonoBehaviour
	{

		void Awake ()
		{
			
		}
		
		void Start ()
		{
			
		}

		void Update ()
		{
            var h = new List<RaycastHit2D>();
            var p = new List<Vector3>();
            var vel = new Vector3(0.5f, 0.5f, 1);
            var acc = new Vector3(0.5f, 0.5f, 1);
            AwesomeExtensions.GetArcHits(out h, out p, 0, transform.position, vel, acc, 0.05f, 10f, false, false);

        }
	}
}
