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
        Orientation();
        float moveInput = Input.GetAxis("Horizontal");
        // if (Mathf.Abs(moveInput) > 0) point.pinned = true;
        // else point.pinned = false;
        velocity = new Vector2(moveInput * moveSpeed, velocity.y);
        transform.position += transform.right * (Mathf.Sign(moveInput) * velocity.magnitude * Time.deltaTime);
    }

    void Orientation()
    {

        Ray ray = new Ray(transform.position, Vector3.down);
        var hit = Physics2D.CircleCast(ray.origin, 0.5f, ray.direction, 10f);

        if (!hit) return;
        transform.up = Vector3.Slerp(transform.up, hit.transform.up, Time.deltaTime * 10);
        // transform.up = hit.transform.up;
    }
    
    void OrientationOld()
    {

        Ray ray1 = new Ray(transform.position,Vector3.right + Vector3.down);
        Ray ray2 = new Ray(transform.position,Vector3.left + Vector3.down);

        Color col1 = Color.white, col2 = Color.white;

        var hit1 = Physics2D.Raycast(ray1.origin, ray1.direction);
        var hit2 = Physics2D.Raycast(ray2.origin, ray2.direction);
        
        Debug.DrawRay(hit1.point, Vector2.up);
        Debug.DrawRay(hit2.point, Vector2.up);
        var center = Vector2.Lerp(hit1.point, hit2.point, 0.5f);
        var centerToPlayerDir = ((Vector2)transform.position - center).normalized;
        centerToPlayerDir = new(-centerToPlayerDir.x, centerToPlayerDir.y);
        transform.up = (Vector3)centerToPlayerDir;
        Debug.DrawRay(center, centerToPlayerDir, Color.magenta);
        
        Debug.DrawRay(ray1.origin, ray1.direction, col1);
        Debug.DrawRay(ray2.origin, ray2.direction, col2);
        
        
    }
}
