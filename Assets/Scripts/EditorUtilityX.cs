using UnityEditor;
using UnityEngine;
namespace Core
{
    public class EditorUtilityX
    {
        public static Vector3 GetWorldPointFromMouse(bool plane = true, float planeLevel = 0)
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