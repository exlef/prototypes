using System;
using System.Collections;
using UnityEngine;

public class PushBox : MonoBehaviour
{
    public bool isDropped { get; private set; } = false;
    [SerializeField] Transform box;
    [SerializeField] Transform point1, point2;
    [SerializeField] private float dropRadius = 0.5f;
    [SerializeField] private float dropAnimDuration = 1f;
    [SerializeField] AnimationCurve curve;

    public Vector2 pos
    {
        get => new Vector2(box.position.x, box.position.z);
        set => box.position = new Vector3(value.x, box.position.y, value.y);
    }
    public Vector2 size => new Vector2(box.localScale.x, box.localScale.z);

    private void Update()
    {
        if (isDropped) return;
        if (Vector3.Distance(box.position, point1.position) < dropRadius)
        {
            StartCoroutine(Drop());
        }
        else if (Vector3.Distance(box.position, point2.position) < dropRadius)
        {
            StartCoroutine(Drop());
        }
    }

    IEnumerator Drop()
    {
        Vector3 startValue = box.position;
        Vector3 endValue = box.position;
        endValue.y *= -1;
        float elapseTime = 0;
        while (elapseTime < dropAnimDuration)
        {
            elapseTime += Time.deltaTime;
            float t = curve.Evaluate(elapseTime);

            box.position = Vector3.LerpUnclamped(startValue, endValue, t);
            yield return null;
        }
        
        isDropped = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(new Vector3(pos.x, 0, pos.y), new Vector3(size.x, 0, size.y));
    }
}
