using UnityEngine;
using UnityEditor;
using SpiderWeb;
using System;

/// <summary>
/// for this editor script to work points should be named like this:
/// Point_1
/// Point_2
/// Point_3
/// .
/// .
/// .
/// </summary>

[InitializeOnLoad]
public class LevelTool
{
    private static int totalPointCount;

    static LevelTool()
    {
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
        totalPointCount = GetTotalPointNum();
        Debug.Log(totalPointCount);
    }

    private static void OnHierarchyChanged()
    {
        GameObject selectedObject = Selection.activeGameObject;
        if(selectedObject == null) return;
        Point point = selectedObject.GetComponent<Point>();
        if (point)
        {
            if(Int32.TryParse(point.gameObject.name.Split("_")[1], out int pointNum))
            {
                if(pointNum == totalPointCount + 1)
                {
                    Point previousPoint = GameObject.Find("Point_" + (pointNum - 1)).GetComponent<Point>();
                    if(!previousPoint) return;
                    Debug.Log($"previous point {previousPoint.name} selected point {point.name}");
                }
            }
        }


        totalPointCount = GetTotalPointNum();
    }

    static int GetTotalPointNum()
    {
        return GameObject.FindObjectsByType<Point>(FindObjectsSortMode.None).Length;
    }
}
