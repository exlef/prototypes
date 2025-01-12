using UnityEngine;
using Ex;
using Ex.Verlet;
using System.Collections.Generic;

public class Demo : MonoBehaviour
{
    [SerializeField] Transform pointsParent;
    [SerializeField] Fruit[] fruitPrefabs;
    [SerializeField] GameObject plusOnePrefab;
    [SerializeField] Bounds bounds = new(Vector3.zero, new(12,12,0));
    [SerializeField] private float ropePointWeight = 1;
    
    List<PhysicsEntity> physicsEntities = new();
    // List<Point> points;
    List<Stick> sticks;

    private int lastFruit = (int)FruitType.plum;
    
    void Start()
    {
        CreateRope();
    }

    void CreateRope()
    {
        List<Point> points = new();
        sticks = new();
        
        for (int i = 0; i < pointsParent.childCount; i++)
        {
            Transform tr = pointsParent.GetChild(i);
            if (!tr.gameObject.activeInHierarchy) continue;
            Point p;
            if (i == 0 || i == pointsParent.childCount - 1) 
            {
                p = new(tr, true);
            }
            else
            {
                p = new (tr);
            }

            points.Add(p);
            physicsEntities.Add(new PhysicsEntity(p, false, FruitType.none, ropePointWeight));
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            Stick s = new(points[i], points[i+1]);
            sticks.Add(s);
        }
    }

    void Update()
    {
        AddFruitOnInput();
        RemoveStickOnInput();
    }

    void FixedUpdate()
    {
        UpdateSim();
    }

    void RemoveStickOnInput()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
           sticks.RemoveAt(3);
        }
    }

    void UpdateSim()
    {
        foreach (var p in physicsEntities)
        {
            p.point.Tick();
        }

        
        for (int i = 0; i < 12; i++)
        {
            for (int sIndex = 0; sIndex < 12; sIndex++)
            {
                foreach (var s in sticks)
                {
                    s.Tick();
                }    
            }
            

            foreach (var p in physicsEntities)
            {
                p.point.ConstrainWorldBounds(bounds);
            }
            for (int j = 0; j < physicsEntities.Count; j++)
            {
                for (int k = j + 1; k < physicsEntities.Count; k++)
                {
                    var physicsEntity1 = physicsEntities[j];
                    var physicsEntity2 = physicsEntities[k];

                    var p1 = physicsEntities[j].point;
                    var p2 = physicsEntities[k].point;
                    
                    float p1Weight = physicsEntity1.weight;
                    float p2Weight = physicsEntity2.weight;
                    
                    if(p1.pinned && p2.pinned) continue;

                    if(p1.pinned && !p2.pinned)
                    {
                        var dynCirclePos = ExPhysics2d.SolveCirclesStaticDynamic(p1.pos, p1.radius, p2.pos, p2.radius);
                        p2.tr.position = dynCirclePos;
                        continue;
                    }
                    if(p2.pinned && !p1.pinned)
                    {
                        var dynCirclePos = ExPhysics2d.SolveCirclesStaticDynamic(p2.pos, p2.radius, p1.pos, p1.radius);
                        p1.tr.position = dynCirclePos;
                        continue;
                    }

                    var result = ExPhysics2d.SolveCirclesCollisionBasedOnWeight(p1.pos, p1.radius, p1Weight, p2.pos, p2.radius, p2Weight);
                    p1.tr.position = result.Item1;
                    p2.tr.position = result.Item2;
                }
            }
        }
        
        bool shouldBreak = false;
        for (int j = 0; j < physicsEntities.Count; j++)
        {
            for (int k = j + 1; k < physicsEntities.Count; k++)
            {
                var physicsEntity1 = physicsEntities[j];
                var physicsEntity2 = physicsEntities[k];

                if (!physicsEntity1.isFruit || !physicsEntity2.isFruit) continue;
                float padding = 0.001f;
                bool isColliding = ExPhysics2d.CirclesCheck(physicsEntity1.point.pos, physicsEntity1.point.radius + padding, physicsEntity2.point.pos, physicsEntity2.point.radius, out float overlap);
                if(!isColliding) continue;
                if(physicsEntity1.fruitType != physicsEntity2.fruitType) continue;
                MergeFruits(j,k);
                shouldBreak = true;
                break;
            }
            if (shouldBreak) break;
        }
    }

    void AddFruitOnInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = Utils.MousePos2D();
            var fruit = Instantiate(fruitPrefabs.GetRandomItem<Fruit>(), mousePos, Quaternion.identity);
            Point p = new(fruit.transform);
            physicsEntities.Add(new PhysicsEntity(p, true, fruit.type, fruit.weight));
        }
    }

    void AddScore(Vector3 pos)
    {
        Instantiate(plusOnePrefab, pos, Quaternion.identity);
                
    }

    void AddFruitOfMerge(PhysicsEntity mergedFruit1, PhysicsEntity mergedFruit2)
    {
        int t = (int)mergedFruit1.fruitType + 1;
        
        var pos = mergedFruit1.point.pos;

        if (t > lastFruit)
        {
            DestroyMergedFruits();
            AddScore(pos);
            return;
        }
        
        var fruit = Instantiate(fruitPrefabs[t], pos, Quaternion.identity);
        Point p = new(fruit.transform);
        physicsEntities.Add(new PhysicsEntity(p, true, fruit.type, fruit.weight));
        
        DestroyMergedFruits();

        void DestroyMergedFruits()
        {
            Destroy(mergedFruit1.point.tr.gameObject);
            Destroy(mergedFruit2.point.tr.gameObject);    
        }
    }

    void MergeFruits(int i1, int i2)
    {
        var physicsEntity1 = physicsEntities[i1];
        var physicsEntity2 = physicsEntities[i2];
        
        physicsEntities.Remove(physicsEntity1);
        physicsEntities.Remove(physicsEntity2);
        
        AddFruitOfMerge(physicsEntity1, physicsEntity2);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(bounds.center, bounds.extents);
    }
}

public enum FruitType
{
    none = -1,
    watermelon = 0,
    coconut = 1,
    orange = 2,
    plum = 3,
}
