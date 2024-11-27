using UnityEngine;

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

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, bounds);
    }

    void Update()
    {
    }

    struct Circle
    {
        public Transform tr;
        public float radius;

        public Circle(Transform i_tr)
        {
            tr = i_tr;
            radius = tr.GetComponent<Renderer>().bounds.extents.x;
        }
    }
}
