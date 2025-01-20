using UnityEngine;

public class AABB_Entity : MonoBehaviour
{
   public Vector2 pos => new Vector2(transform.position.x, transform.position.z);
   public Vector2 size => new Vector2(transform.localScale.x, transform.localScale.z);
   [field:SerializeReference] public bool isStatic { get; private set; }
   private void OnDrawGizmos()
   {
      Gizmos.DrawWireCube(transform.position, transform.localScale);
      Gizmos.color = Color.magenta;
      Gizmos.DrawWireCube(new Vector3(pos.x, 0, pos.y), new Vector3(size.x, 0, size.y));
   }
}
