using UnityEngine;

public class Cube : MonoBehaviour
{
   void OnMouseDown()
   {
       FindAnyObjectByType<GridExample>().Clicked(this.gameObject);
   }
}
