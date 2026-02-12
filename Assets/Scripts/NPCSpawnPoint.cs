using UnityEngine;

public class NPCSpawnPoint : MonoBehaviour
{
   public GameObject[] patrolPoints;
   private void OnDrawGizmos()
   {
      if (patrolPoints == null || patrolPoints.Length == 0)
         return;

      Gizmos.color = Color.yellow;
      for (int i = 0; i < patrolPoints.Length; i++)
      {
         var pt = patrolPoints[i];
         if (pt != null)
         {
            Gizmos.DrawSphere(pt.transform.position, 0.3f);
            // Draw line to next point (looped)
            var next = patrolPoints[(i + 1) % patrolPoints.Length];
            if (next != null && patrolPoints.Length > 1)
               Gizmos.DrawLine(pt.transform.position, next.transform.position);
         }
      }
   }
}
