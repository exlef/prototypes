using System;
using UnityEngine;

public class PushBox : MonoBehaviour
{
    [SerializeField] private Transform box;

    public Vector2 pos
    {
        get => new Vector2(box.position.x, box.position.z);
        set => box.position = new Vector3(value.x, box.position.y, value.y);
    }
    public Vector2 size => new Vector2(box.localScale.x, box.localScale.z);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(new Vector3(pos.x, 0, pos.y), new Vector3(size.x, 0, size.y));
    }
}
