using System.Collections;
using UnityEngine;

public class Segment : MonoBehaviour
{
    [SerializeField] float speed = 10;
    bool canMove = true;

    public void Move()
    {
        StartCoroutine(MoveCo());
        IEnumerator MoveCo()
        {
            while(canMove)
            {
                transform.Translate(Vector2.up * (speed * Time.deltaTime));

                yield return new WaitForFixedUpdate();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        canMove = false;
        transform.parent = other.transform;
    }
}
