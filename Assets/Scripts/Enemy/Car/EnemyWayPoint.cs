using UnityEngine;

public class EnemyWayPoint : MonoBehaviour
{ 
    private void OnTriggerEnter(Collider col)
    {
        EnemyCarController enemy = col.GetComponent<EnemyCarController>();
        MovmentEnemy enemyMov = col.GetComponent<MovmentEnemy>();

        if (enemy != null)
        {
            enemy.NotifyWaypointReached(transform);
        }
        else if(enemyMov != null)
        {
            enemyMov.ChangePoint(transform);
        }
    }
}