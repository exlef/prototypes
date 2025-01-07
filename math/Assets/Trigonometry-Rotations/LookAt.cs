using UnityEngine;

public class LookAt : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float xDelta = Input.mousePositionDelta.x / Screen.width;
        float yDelta = Input.mousePositionDelta.y / Screen.height;

        // var forward = Quaternion.Euler(xDelta * 360, yDelta * 360, 0) * transform.forward;
        var forward = Quaternion.Euler(yDelta * 360, xDelta * 360, 0) * transform.forward;

        transform.forward = forward;
    }
}
