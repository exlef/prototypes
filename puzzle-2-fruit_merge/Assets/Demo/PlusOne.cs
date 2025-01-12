using System;
using UnityEngine;

public class PlusOne : MonoBehaviour
{
    [SerializeField] float fadeSpeed = 1.0f;
    [SerializeField] float moveSpeed = 1.0f;
    SpriteRenderer spriteRenderer;
    Color originalColor;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        transform.Translate(Vector3.up * (moveSpeed * Time.deltaTime));
        
        var newColor = spriteRenderer.color;
        newColor.a -= fadeSpeed * Time.deltaTime;
        newColor.a = Mathf.Clamp(newColor.a, 0, originalColor.a);
        spriteRenderer.color = newColor;
    }
}
