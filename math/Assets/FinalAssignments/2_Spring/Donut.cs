using UnityEngine;
using UnityEditor;

public class Donut : MonoBehaviour
{
    [SerializeField] float radius = 1;
    [SerializeField] int resolution = 32;
    [SerializeField] int layerCount = 2;
    [SerializeField] float heightMultiplier = 0.5f;
    [SerializeField] Color bottom = Color.black;    
    [SerializeField] Color top = Color.white;
    [SerializeField] Transform helper;

    Vector3[] points;
    Color[] colors;
    
    private void OnDrawGizmos()
    {
        // Handles.matrix = transform.localToWorldMatrix;
        
        points = new Vector3[resolution * layerCount];
        colors = new Color[resolution * layerCount];
        var angle = 360.0f / resolution;
        var centerAngle = 360.0f / layerCount;

        Vector3 center = Vector3.zero;
        for (int j = 0; j < layerCount; ++j)
        {
            center.x = Mathf.Cos(centerAngle * j * Mathf.Deg2Rad) * 5;
            center.y = Mathf.Sin(centerAngle * j * Mathf.Deg2Rad) * 5;
            Gizmos.DrawWireSphere(transform.position + center, 0.5f);
            helper.position = transform.position + center;
            if (j == layerCount - 1)
            {
                Vector3 startingCenter = Vector3.zero;
                startingCenter.x = Mathf.Cos(centerAngle * 0 * Mathf.Deg2Rad) * 5;
                startingCenter.y = Mathf.Sin(centerAngle * 0 * Mathf.Deg2Rad) * 5;
                helper.up = startingCenter - center;
            }
            else
            {
                Vector3 nextCenter = Vector3.zero;
                nextCenter.x = Mathf.Cos(centerAngle * (j+1) * Mathf.Deg2Rad) * 5;
                nextCenter.y = Mathf.Sin(centerAngle * (j+1) * Mathf.Deg2Rad) * 5;
                helper.up = nextCenter - center;
            }
            for (int i = 0; i < resolution; i++)
            {
                var dir = Quaternion.AngleAxis(angle * i, helper.up) * helper.right;
                dir = dir.normalized;
                var pos = dir * radius;
                var debugPos = pos + helper.position;
                Debug.DrawRay(debugPos, helper.up, Color.green);
                // pos.y = Mathf.Lerp(j * heightMultiplier, (j+1) * heightMultiplier, i / (float)resolution);

                pos += helper.position;
                
                Debug.DrawRay(helper.position, helper.up, Color.magenta);
                
                points[j * resolution +  i] = pos;
            }    
        }

        for (var i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.Lerp(bottom, top, i / (float)colors.Length);
        }

        Handles.DrawAAPolyLine(5, colors,points);
    }
}
