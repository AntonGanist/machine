using UnityEngine;

public class CarWayManager : MonoBehaviour
{
    [SerializeField] EnemyWayPoint[] enemyWayPoints;

    public Transform[] GetWay()
    {
        Transform[] transforms = new Transform[enemyWayPoints.Length];
        for (int i = 0; i < enemyWayPoints.Length; i++)
            transforms[i] = enemyWayPoints[i].transform;
        return transforms;
    }
    
}
