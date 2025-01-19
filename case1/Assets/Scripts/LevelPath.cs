using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class LevelPath : MonoBehaviour
{
    public List<Vector3> points = new List<Vector3>();
    
    public bool TryGetNextPointFromCannonToTower(int index, out Vector3 point)
    {
        if (index >= points.Count)
        {
            point = Vector3.zero;
            return false;
        }

        point = points[index];
        return true;
    }
    
    public bool TryGetNextPointFromTowerToCannon(int index, out Vector3 point)
    {
        if (index >= points.Count)
        {
            point = Vector3.zero;
            return false;
        }

        point = points[^index];
        return true;
    }
    
    private void OnDrawGizmos()
    {
        // Draw a sphere at each point
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + points[i], 0.1f);
            
            if (i < points.Count - 1)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position + points[i], transform.position + points[i + 1]);
            }
        }
    }
}

#if UNITY_EDITOR

[UnityEditor.CustomEditor(typeof(LevelPath))]
public class PointToolEditor : UnityEditor.Editor
{
    private void OnSceneGUI()
    {
        LevelPath tool = (LevelPath)target;

        // Allow manipulation of each point
        for (int i = 0; i < tool.points.Count; i++)
        {
            UnityEditor.EditorGUI.BeginChangeCheck();
            Vector3 newPoint = UnityEditor.Handles.PositionHandle(tool.transform.position + tool.points[i], Quaternion.identity);
            if (UnityEditor.EditorGUI.EndChangeCheck())
            {
                UnityEditor.Undo.RecordObject(tool, "Move Point");
                tool.points[i] = newPoint - tool.transform.position;
            }
        }
    }
}

#endif
