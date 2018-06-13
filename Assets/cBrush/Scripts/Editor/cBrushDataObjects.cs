using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Collections.Generic;

public class cBrushDataObjects
{
    public Transform m_CurrentObject = null;
    public List<Transform> m_ObjectList = null;
    public List<Texture2D> m_ObjectListThumb = null;

    public void Reset()
    {
        m_CurrentObject = null;
        m_ObjectList = new List<Transform>();
        m_ObjectListThumb = new List<Texture2D>();

    }

}