using System;
using System.Collections;
using UnityEngine;

public class PushBox : MonoBehaviour
{
    public bool isDropped { get; private set; } = false;
    [SerializeField] Transform box;
    [SerializeField] Transform point1, point2;
    [SerializeField] float dropRadius = 0.5f;
    [SerializeField] float dropAnimDuration = 1f;
    [SerializeField] GameObject text;
    [SerializeField] AnimationCurve curve;
    private bool isDropping;

    public Vector2 pos
    {
        get => new Vector2(box.position.x, box.position.z);
        set => box.position = new Vector3(value.x, box.position.y, value.y);
    }
    public Vector2 size => new Vector2(box.localScale.x, box.localScale.z);

    private void Update()
    {
        if (isDropped) return;
        if (isDropping) return;
        if (Vector3.Distance(box.position, point1.position + Vector3.up * box.localScale.y/2) < dropRadius)
        {
            StartCoroutine(Drop());
        }
        else if (Vector3.Distance(box.position, point2.position  + Vector3.up * box.localScale.y/2) < dropRadius)
        {
            StartCoroutine(Drop());
        }
    }

    IEnumerator Drop()
    {
        isDropping = true;
        text.SetActive(false);
        Vector3 startValue = box.position;
        Vector3 endValue = box.position;
        float targetY = -box.localPosition.y + 0.1f;
        endValue.y = targetY;
        float elapseTime = 0;
        while (elapseTime < dropAnimDuration)
        {
            elapseTime += Time.deltaTime;
            float t = elapseTime / dropAnimDuration;
            t = curve.Evaluate(t);

            box.position = Vector3.LerpUnclamped(startValue, endValue, t);
            yield return null;
        }
        
        box.position = endValue;
        isDropped = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(new Vector3(pos.x, 0, pos.y), new Vector3(size.x, 0, size.y));
        
        Gizmos.DrawWireSphere(point1.position, dropRadius);
        Gizmos.DrawWireSphere(point2.position, dropRadius);
    }
}
