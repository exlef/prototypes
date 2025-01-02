using UnityEngine;
using Ex;

public class Test : MonoBehaviour
{
    [SerializeField] Transform anchor;
    [SerializeField] Transform point;
    Spring spring;
    [SerializeField] Spring.SpringFeatures springFeatures;

    void Start()
    {
        spring = new Spring(springFeatures);
    }

    void FixedUpdate()
    {
        spring.AnchorPointSpring(anchor, point);
    }
}
