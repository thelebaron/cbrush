using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
namespace Core
{
    [ExecuteInEditMode]
    public class CustomSceneMouseMoverUI : MonoBehaviour
    {
        private bool m_Initialised;
        public bool AllowMouseMove;
        Ray m_Ray;
        Camera m_Camera;

        public void Start()
        {
            m_Camera = GetComponent<Camera>();
        }

        public void Update()
        {
            if (!m_Initialised)
                Start();
        }

        public void Toggle()
        {
            AllowMouseMove = !AllowMouseMove;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            
        }

        public Vector3 GetWorldPointFromMouse(bool plane = true, float planeLevel = 0)
        {
            var groundPlane = new Plane(Vector3.up, new Vector3(0, planeLevel, 0));

            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit rayHit;
            Vector3 hit = new Vector3();
            float dist;

            if (plane)
                if (groundPlane.Raycast(ray, out dist))
                {
                    hit = ray.origin + ray.direction.normalized * dist;
                    Debug.Log("Hit plane");
                }

            if (!plane)
                if (Physics.Raycast(ray, out rayHit))
                {
                    hit = rayHit.point;

                    Debug.Log("Hit something else");
                }

            return hit;
        }
    }
}