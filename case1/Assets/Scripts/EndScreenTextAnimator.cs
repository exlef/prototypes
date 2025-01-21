using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenTextAnimator : MonoBehaviour
{
    [SerializeField] AnimationCurve curve;
    [SerializeField] float animSpeed = 5;
    Vector3 originalScale;
    private void OnEnable()
    {
        originalScale = transform.localScale;
        transform.localScale = Vector3.zero;
        StartCoroutine(Animate());
    }
    
    IEnumerator Animate()
    {
        float t = 0;

        while (t < 1)
        {
            // if (t > 1) yield break;
            t += Time.unscaledDeltaTime * animSpeed;
            float easedT = curve.Evaluate(t);
            transform.localScale = Vector3.LerpUnclamped(Vector3.zero, originalScale, easedT);
            yield return null;
        }
    }
}
