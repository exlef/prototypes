using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public static class TransformShaker
{
    public enum ShakeType
    {
        Position,
        Rotation,
        Scale
    }

    public static IEnumerator ShakeTransform(Transform target, AnimationCurve curve, float duration, Vector3 axisMagnitudes, ShakeType shakeType)
    {
        if (!target)
        {
            yield break;
        }

        // Save the original values
        Vector3 originalPosition = target.localPosition;
        Vector3 originalRotation = target.localEulerAngles;
        Vector3 originalScale = target.localScale;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float normalizedTime = elapsedTime / duration;
            float curveValue = curve.Evaluate(normalizedTime);

            // Calculate shake offsets for each axis
            // Vector3 offset = new Vector3(
            //     Random.Range(-1f, 1f) * curveValue * axisMagnitudes.x,
            //     Random.Range(-1f, 1f) * curveValue * axisMagnitudes.y,
            //     Random.Range(-1f, 1f) * curveValue * axisMagnitudes.z
            // );
            
            Vector3 offset = new Vector3(
                curveValue * axisMagnitudes.x,
                curveValue * axisMagnitudes.y,
                curveValue * axisMagnitudes.z
            );

            switch (shakeType)
            {
                case ShakeType.Position:
                    target.localPosition = originalPosition + offset;
                    break;

                case ShakeType.Rotation:
                    target.localEulerAngles = originalRotation + offset;
                    break;

                case ShakeType.Scale:
                    target.localScale = originalScale + offset;
                    break;
            }

            elapsedTime += Time.deltaTime;
            if (!target)
            {
                yield break;
            }
            yield return null;
        }
        
        if (!target)
        {
            yield break;
        }

        // Reset to the original value
        switch (shakeType)
        {
            case ShakeType.Position:
                target.localPosition = originalPosition;
                break;

            case ShakeType.Rotation:
                target.localEulerAngles = originalRotation;
                break;

            case ShakeType.Scale:
                target.localScale = originalScale;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(shakeType), shakeType, null);
        }
    }
}
