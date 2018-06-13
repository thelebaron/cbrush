using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public static class AwesomeExtensions
{


    public static void SetObjectPosition(Transform go, bool set, float min, float max)
    {
        var maxtilt = UnityEngine.Random.Range(-15f, 15f);
        var maxtilty = UnityEngine.Random.Range(0f, 360f);
        go.transform.rotation = Quaternion.Euler(go.transform.eulerAngles.x, maxtilty, go.transform.eulerAngles.z);

    }

    public static void SetObjectRotation(Transform go, bool set, float x, float y, float z)
    {
        if (!set)
            return;

        var new_x = UnityEngine.Random.Range(0f, x);
        var new_y = UnityEngine.Random.Range(0f, y);
        var new_z = UnityEngine.Random.Range(0f, z);

        
        if (IsZero(x))
            new_x = go.transform.eulerAngles.x;
        if (IsZero(y))
            new_y = go.transform.eulerAngles.y;
        if (IsZero(z))
            new_z = go.transform.eulerAngles.z;
        
        go.transform.rotation = Quaternion.Euler(new_x, new_y, new_z);

    }

    public static void SetObjectScale(Transform go, bool set, float min, float max)
    {
        if (!set)
            return;
        var SIZE = UnityEngine.Random.Range(min, max);
        go.transform.localScale = new Vector3(SIZE, SIZE, SIZE);
    }

    //Positive or negative helpers

    public static bool IsPositive(this int number)
    {
        return number > 0;
    }

    public static bool IsNegative(this int number)
    {
        return number < 0;
    }

    public static bool IsZero(this int number)
    {
        return number == 0;
    }

    public static bool IsAwesome(this int number)
    {
        return IsNegative(number) && IsPositive(number) && IsZero(number);
    }

    public static bool IsPositive(this float number)
    {
        return number > 0;
    }

    public static bool IsNegative(this float number)
    {
        return number < 0;
    }

    public static bool IsZero(this float number)
    {
        return number == 0;
    }


    public static void GetArcHits(out List<RaycastHit2D> Hits, out List<Vector3> Points, int iLayerMask, Vector3 vStart, Vector3 vVelocity, Vector3 vAcceleration, float fTimeStep = 0.05f, float fMaxtime = 10f, bool bIncludeUnits = false, bool bDebugDraw = false)
    {
        Hits = new List<RaycastHit2D>();
        Points = new List<Vector3>();

        Vector3 prev = vStart;
        Points.Add(vStart);

        for (int i = 1; ; i++)
        {
            float t = fTimeStep * i;
            if (t > fMaxtime) break;
            Vector3 pos = PlotTrajectoryAtTime(vStart, vVelocity, vAcceleration, t);

            var result = Physics2D.Linecast(prev, pos, iLayerMask);
            if (result.collider != null)
            {
                Hits.Add(result);
                Points.Add(pos);
                break;
            }
            else
            {
                Points.Add(pos);

            }
            Debug.DrawLine(prev, pos, Color.Lerp(Color.yellow, Color.red, 0.35f), 0.5f);

            prev = pos;
        }
    }

    public static Vector3 PlotTrajectoryAtTime(Vector3 start, Vector3 startVelocity, Vector3 acceleration, float fTimeSinceStart)
    {
        return start + startVelocity * fTimeSinceStart + acceleration * fTimeSinceStart * fTimeSinceStart * 0.5f;
    }

}

