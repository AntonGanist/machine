using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] List<EnemyBrain> _enemys;
    [SerializeField] CarWayManager carWayManager;
    Transform _player;
    public void Initialize(Func<Transform, Transform, Transform> findshelter, Transform player)
    {
        for(int i = 0; i < _enemys.Count; i++)
        {
            _enemys[i].Initialize(findshelter, carWayManager.GetWay());
        }
        _player = player;
    }
}
