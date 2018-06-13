using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class gui : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnGUI()
	{
		
		 GUI.Box(new Rect(0, 0, 500, 100), "1st position");
		
			GUI.Box(new Rect(0, 50, 500, 100), "2nd position");


	}

}
