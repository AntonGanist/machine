using System;
using System.Collections.Generic;
using UnityEngine;

/// 0 переходник
/// 1 бродить (состояние должно активироваться почаще если видна машина)
/// 2 чинить машину
/// 3 ставить ловушки
/// 4 быть в засаде
/// 
/// 5 сражаться 
/// 6 прятаться
/// 7 бежать
/// 
/// 8 ждать
/// 
/// 
/// 
/// 
/// нужно сделать вход в машину где нет дверей
public class EnemyBrain : MonoBehaviour
{
    [SerializeField] EnemyBaseInformation enemy;
    [SerializeField] LayerMask _exceptionLayerMask;
    [SerializeField] Health _health;
    [SerializeField] EnemyInventory _inventory;

    [SerializeField] EnemyState[] _enemyStates;
    [SerializeField] EnemyState _chooseState;
    List<CarDetail> _listCarDetails = new List<CarDetail>();

    public void Initialize(Func<Transform, Transform, Transform> findshelter, Transform[] wayPoints)
    {
        enemy._getHealth = _health.GetHealth;
        enemy._findShelter = findshelter;
        for (int i = 0;  i < _enemyStates.Length; i++)
        {
            if(_enemyStates[i] != null)
            {
                var move = _enemyStates[i].GetComponent<MovmentEnemy>();
                move?.TakePoints(wayPoints);

                _enemyStates[i].Initialize(i, SwitchState, enemy, _exceptionLayerMask, TakeCarDetal, _inventory.TakeItem);
            }

        }
        _chooseState = _enemyStates[0];
        _chooseState.EnterState(false);
    }

    void Update()
    {
        _chooseState.UpdateState();
    }

    void SwitchState(int id, bool sawEnemies)
    {
        Debug.Log(id);
        if(id == 3 || id == 5) id = 8;
        _chooseState = _enemyStates[id];
        _chooseState.EnterState(sawEnemies);

        var fix = _chooseState.GetComponent<FixCarEnemy>();
        fix?.TakeCarDetals(_listCarDetails);
    }

    void TakeCarDetal(List<CarDetail> carDetails)
    {
        if(_listCarDetails.Count == 0)
            _listCarDetails = carDetails;
        else
        {
            foreach (var detail in carDetails)
            {
                if (!_listCarDetails.Contains(detail))
                {
                    _listCarDetails.Add(detail);
                }
            }

        }
    }
}
