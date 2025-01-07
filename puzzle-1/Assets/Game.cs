using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] CenterCircle centerCircle;
    [SerializeField] Segment segmentPrefab;
    [SerializeField] Vector2 spawnPoint;
    Segment currentMovingSegment;

    // Update is called once per frame
    void Update()
    {
        centerCircle.Tick();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SpawnSegment();
            currentMovingSegment.Move();
        }
    }

    void SpawnSegment()
    {
        currentMovingSegment = Instantiate(segmentPrefab, spawnPoint, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(spawnPoint, 0.1f);
    }
}
