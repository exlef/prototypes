using System;
using UnityEditor;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    [SerializeField] [Range(0.0f,100.0f)] float health = 100f;
    [SerializeField] private Vector3 p1;
    [SerializeField] private Vector3 p2;
    private void OnDrawGizmos()
    {
        Handles.color = Color.black;
        Handles.DrawLine(p1, p2, 12);
        
        Vector3 point = Vector3.Lerp(p1, p2, health / 100.0f);
        Handles.color = Color.Lerp(Color.red, Color.green, (health - 20) / 80.0f);
        Handles.DrawLine(p1, point, 6);
    }
}
