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
                    string sticksParentName = "Sticks";
                    Transform sticks = GameObject.Find(sticksParentName).GetComponent<Transform>();
                    if(!sticks) Debug.LogWarning($"there is no game object called as {sticksParentName} in the hierarchy.");
                    if (!sticks) return;
                    InstantiatePrefab("Assets/SpiderWebDemo/Stick.prefab", sticks);
                }
            }
        }


        totalPointCount = GetTotalPointNum();
    }

    static int GetTotalPointNum()
    {
        return GameObject.FindObjectsByType<Point>(FindObjectsSortMode.None).Length;
    }

    /// <summary>
    /// path example:
    /// Assets/Path/To/Your/Prefab.prefab
    /// </summary>
    /// <param name="path"></param>
    public static void InstantiatePrefab(string prefabPath, Transform parent = null)
    {
        // Load the prefab
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (prefab != null)
        {
            GameObject instance;
            if (parent) instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            else instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            Undo.RegisterCreatedObjectUndo(instance, "Instantiate Prefab");
            // Selection.activeObject = instance; // Select the new instance in the hierarchy
            // Mark the instance as dirty to refresh the hierarchy
            EditorUtility.SetDirty(instance);
        }
        else
        {
            Debug.LogWarning("Prefab not found at the specified path: " + prefabPath);
        }
    }
}
