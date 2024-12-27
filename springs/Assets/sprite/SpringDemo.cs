using UnityEngine;

public class SpringDemo : MonoBehaviour
{
    [SerializeField] Transform anchor;
    [SerializeField] Transform point;
    
    // [SerializeField] float springStiffness = 10f;  // Controls how "stiff" the spring is
    // [SerializeField] float damping = 0.5f;         // Controls how quickly oscillations settle
    // [SerializeField] float mass = 1f;              // Mass of the point
    [SerializeField] bool followMouse = false;
    
    private Vector3 velocity = Vector3.zero;        // Current velocity of the point

    void Update()
    {   
        if(followMouse)
            anchor.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    // void FixedUpdate()
    // {
    //     // Calculate spring force using Hooke's Law: F = -kx
    //     Vector3 displacement = point.position - anchor.position;
    //     Vector3 springForce = -springStiffness * displacement;
        
    //     // Calculate damping force: F = -cv
    //     Vector3 dampingForce = -damping * velocity;
        
    //     // Sum up forces
    //     Vector3 totalForce = springForce + dampingForce;
        
    //     // Calculate acceleration (F = ma)
    //     Vector3 acceleration = totalForce / mass;
        
    //     // Update velocity (integrate acceleration)
    //     velocity += acceleration;
        
    //     // Update position (integrate velocity)
    //     point.position += velocity;
    // }
}
