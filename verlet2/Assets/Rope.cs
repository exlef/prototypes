using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] Point nodePrefab;
    [SerializeField] Vector2 endPoint;
    [SerializeField] int NodeCount = 1;
    List<Point> nodes = new();
    List<Stick> sticks = new();
    int iterations = 7; // Adjust based on simulation needs

    private bool canRun;

    public void Init()
    {
        canRun = true;
        GenerateRopeNodes();
    }

    [ContextMenu("Generate")]
    void GenerateRopeNodes()
    {
        nodes.Clear();
        sticks.Clear();
        
        for (int i = transform.childCount-1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
                
        float size = endPoint.x - transform.position.x;
        var nodeSize = size / NodeCount;
        Vector2 startPoint = transform.position;
        for (int i = 0; i < NodeCount; i++)
        {
            var n = Instantiate(nodePrefab, transform);
            n.transform.localScale = Vector3.one * nodeSize;
            float t = i / (float)(NodeCount - 1); // Normalize index to [0, 1]
            Vector2 position = Vector2.Lerp(startPoint, endPoint, t);
            n.transform.position = position;
            if (i == 0) n.pinned = true;
            if (i == NodeCount - 1) n.pinned = true;
            n.gameObject.AddComponent<CircleCollider2D>();
            n.Init();
            nodes.Add(n);
            
            if (i > 0)
            {
                var s = n.gameObject.AddComponent<Stick>();
                s.a = nodes[i - 1];
                s.b = nodes[i];
                s.Init();
                sticks.Add(s);
            }
        }

        
    }
    
    void FixedUpdate()
    {
        if (!canRun) return;
        foreach (var n in nodes)
        {
            n.Tick();
        }

        for (int j = 0; j < iterations; j++)
        {
            foreach (var s in sticks)
            {
                s.Tick();
            }
        }
        
        for (int i = 1; i < nodes.Count - 1; i++)
        {
            nodes[i].transform.right = (nodes[i + 1].transform.position - nodes[i].transform.position).normalized;
            Debug.DrawLine(nodes[i].transform.position, nodes[i + 1].transform.position, Color.magenta);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(endPoint, 0.2f);
    }
}
