using UnityEngine;

public class Player : MonoBehaviour
{
    Point point;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        point = GetComponent<Point>();
    }

    // Update is called once per frame
    private Vector2 velocity;
    private float moveSpeed = 3;
    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(moveInput) > 0) point.pinned = true;
        else point.pinned = false;
        velocity = new Vector2(moveInput * moveSpeed, velocity.y);
        transform.position += (Vector3)velocity * Time.deltaTime;
    }
}
