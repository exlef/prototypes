using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] CenterCircle centerCircle;
    [SerializeField] Segment segmentPrefab;
    [SerializeField] Vector2 spawnPoint;
    Segment currentMovingSegment;

    void Update()
    {
        centerCircle.Rotate();
        if(Input.GetKeyDown(KeyCode.Space) && currentMovingSegment == null)
        {
            currentMovingSegment = SpawnSegment();
            currentMovingSegment.SetSector(centerCircle.sectorDir, centerCircle.transform.position, centerCircle.transform);
        }
        if(currentMovingSegment != null)
        {
            currentMovingSegment.Move(centerCircle.transform.position, centerCircle.radius);
            if (currentMovingSegment.hasArrived)
            {
                currentMovingSegment = null;
            }
        }

    }

    Segment SpawnSegment()
    {
        return Instantiate(segmentPrefab, spawnPoint, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(spawnPoint, 0.1f);
    }
}
