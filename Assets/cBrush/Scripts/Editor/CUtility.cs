using UnityEngine;
using System;
using System.Reflection;

public static class CUtility
{
    private static Type _annotationUtility;
    private static PropertyInfo _showGridProperty;

    private static PropertyInfo ShowGridProperty
    {
        get
        {
            if (_showGridProperty == null)
            {
                _annotationUtility = Type.GetType("UnityEditor.AnnotationUtility,UnityEditor.dll");
                if (_annotationUtility != null)
                    _showGridProperty = _annotationUtility.GetProperty("showGrid", BindingFlags.Static | BindingFlags.NonPublic);
            }
            return _showGridProperty;
        }
    }

    public static bool ShowGrid
    {
        get
        {
            return (bool)ShowGridProperty.GetValue(null, null);
        }
        set
        {
            ShowGridProperty.SetValue(null, value, null);
        }
    }

    public static bool ShowShadows
    {
        get
        {
            if (QualitySettings.shadows == ShadowQuality.Disable)
                return false;
            if (QualitySettings.shadows == ShadowQuality.All || QualitySettings.shadows == ShadowQuality.HardOnly)
                return true;
            else return true;
        }
        set
        {
            if (QualitySettings.shadows == ShadowQuality.Disable)
                QualitySettings.shadows = ShadowQuality.All;
            if (QualitySettings.shadows == ShadowQuality.All || QualitySettings.shadows == ShadowQuality.HardOnly )
                QualitySettings.shadows = ShadowQuality.Disable;
        }

    }
    
    

}

public class TestShowGrid : MonoBehaviour
{
    [ContextMenu("Toggle Grid")]
    public void ToggleGrid()
    {
        CUtility.ShowGrid = !CUtility.ShowGrid;
    }
}