using UnityEngine;
using System.Collections;

public class ExTweener
{
    private MonoBehaviour coroutineRunner;
    
    public ExTweener( MonoBehaviour runner)
    {
        coroutineRunner = runner;
    }

    // Scale Methods
    public Coroutine ScaleTo(Transform targetTransform, Vector3 targetScale, float speed)
    {
        return coroutineRunner.StartCoroutine(ScaleRoutine(targetTransform, targetScale, speed));
    }

    private IEnumerator ScaleRoutine(Transform targetTransform, Vector3 targetScale, float speed)
    {
        Vector3 startScale = targetTransform.localScale;
        float journey = Vector3.Distance(startScale, targetScale) / speed;
        float elapsedTime = 0f;

        while (elapsedTime < journey)
        {
            targetTransform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / journey);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        targetTransform.localScale = targetScale;
    }

    // Rotation Methods
    public Coroutine RotateTo(Transform targetTransform, Quaternion targetRotation, float speed)
    {
        return coroutineRunner.StartCoroutine(RotateRoutine(targetTransform, targetRotation, speed));
    }

    private IEnumerator RotateRoutine(Transform targetTransform, Quaternion targetRotation, float speed)
    {
        Quaternion startRotation = targetTransform.rotation;
        float angle = Quaternion.Angle(startRotation, targetRotation);
        float journey = angle / speed;
        float elapsedTime = 0f;

        while (elapsedTime < journey)
        {
            targetTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / journey);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        targetTransform.rotation = targetRotation;
    }

    // Translation Methods
    public Coroutine TranslateTo(Transform targetTransform, Vector3 targetPosition, float speed, bool useLocalSpace = false)
    {
        return coroutineRunner.StartCoroutine(TranslateRoutine(targetTransform, targetPosition, speed, useLocalSpace));
    }

    private IEnumerator TranslateRoutine(Transform targetTransform, Vector3 targetPosition, float speed, bool useLocalSpace)
    {
        Vector3 startPosition = useLocalSpace ? targetTransform.localPosition : targetTransform.position;
        float distance = Vector3.Distance(startPosition, targetPosition);
        float journey = distance / speed;
        float elapsedTime = 0f;

        while (elapsedTime < journey)
        {
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / journey);
            
            if (useLocalSpace)
                targetTransform.localPosition = newPosition;
            else
                targetTransform.position = newPosition;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final position is exact
        if (useLocalSpace)
            targetTransform.localPosition = targetPosition;
        else
            targetTransform.position = targetPosition;
    }
}
