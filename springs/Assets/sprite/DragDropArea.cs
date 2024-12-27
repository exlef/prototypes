using Unity.VisualScripting;
using UnityEngine;

public class DragDropArea : MonoBehaviour
{
    [SerializeField] Quad2 heart;
    void OnMouseDrag()
    {
        heart.followMouse = true;
    }

    void OnMouseUp()
    {
        heart.followMouse = false;
        heart.transform.position = (Vector2)transform.position;
    }
}
