using UnityEngine;

using Mathf = UnityEngine.Mathf;

public class Demo : MonoBehaviour
{
    [SerializeField] Vector2 bounds;
    [SerializeField] Transform circlePrefab;    
    [SerializeField] int circleCount = 10;
    Circle[] circles;

    void Start()
    {
        circles = new Circle[circleCount];
        for (int i = 0; i < circleCount; i++)
        {
            Transform go = Instantiate(circlePrefab, transform);
            circles[i] = new Circle(go);
            go.position = new Vector3(Random.Range(-bounds.x/2 + circles[i].radius, bounds.x/2 - circles[i].radius), Random.Range(-bounds.y/2 + circles[i].radius, bounds.y/2 - circles[i].radius));
        }
    }

    void Update()
    {
        for (int i = 0; i < circles.Length; i++)
        {
            Circle circle = circles[i];
            circle.tr.position += (Vector3)circle.velocity * Time.deltaTime;
            var (isCollide, otherIndex) = CheckForCollisions(i);
            if(isCollide) ResolveCollisions_Circles(i, otherIndex);
            ResolveCollisions_LevelBoundries(i);
        }
    }

    void ResolveCollisions_LevelBoundries(int currentIndex)
    {
        var c = circles[currentIndex];

        Vector2 halfBoundsSize = bounds / 2 - Vector2.one * c.radius;

        if (Mathf.Abs(c.tr.position.x) > halfBoundsSize.x)
        {
            c.tr.SetPosX(halfBoundsSize.x * Mathf.Sign(c.tr.position.x));
            c.velocity.x *= -1;
        }
        if (Mathf.Abs(c.tr.position.y) > halfBoundsSize.y)
        {
            c.tr.SetPosY(halfBoundsSize.y * Mathf.Sign(c.tr.position.y));
            c.velocity.y *= -1;
        }
        
        circles[currentIndex] = c;
    }

    void ResolveCollisions_Circles(int currentIndex, int otherIndex)
    {
        var current = circles[currentIndex];
        var other = circles[otherIndex];

        Vector2 co = other.tr.position - current.tr.position;
        Vector2 normal = co.normalized;

        float overlap = current.radius + other.radius - co.magnitude;
        if (overlap > 0)
        {
            Vector2 correction = co.normalized * (overlap / 2);
            current.tr.position -= (Vector3)correction;
            other.tr.position += (Vector3)correction;
        }

        current.velocity = Vector2.Reflect(current.velocity, normal);
        other.velocity = Vector2.Reflect(other.velocity, normal);

        circles[currentIndex] = current;
        circles[otherIndex] = other;
    }

    (bool, int) CheckForCollisions(int cci) // cci = current circle index
    {
        for (int i = 0; i < circles.Length; i++)
        {
            if(i == cci) continue;
            
            var other = circles[i];
            var current = circles[cci];

            Vector2 distance = other.tr.position - current.tr.position;
            if(Vector2.SqrMagnitude(distance) > (other.radius + current.radius) * (other.radius + current.radius)) continue;
            return (true, i);
        }

        return (false, -1);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, bounds);
    }

    struct Circle
    {
        public Transform tr;
        public float radius;
        public Vector2 velocity;

        public Circle(Transform i_tr)
        {
            tr = i_tr;
            radius = tr.GetComponent<Renderer>().bounds.extents.x;
            velocity = ExUtils.RndVec2(3);
        }
    }
}
