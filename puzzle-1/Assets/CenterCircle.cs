using UnityEngine;

public class CenterCircle : MonoBehaviour
{
    [SerializeField] float turningSpeed = 10;
    float angleDeg;

    public void Tick()
    {
        transform.Rotate(0,0, -turningSpeed * Time.deltaTime);
    }
}
