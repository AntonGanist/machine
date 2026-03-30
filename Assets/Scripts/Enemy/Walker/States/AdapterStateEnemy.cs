using System.Collections.Generic;
using UnityEngine;

public class AdapterStateEnemy : EnemyState
{
    public override void EnterState(bool sawEnemies)
    {
        _sawEnemies = sawEnemies;
        CheckTransitions();
    }

    public override void UpdateState()
    {
        
    }

    protected override void CheckTransitions()
    {
        int chooseId;

        FindCarDetals();

        if (_sawEnemies)//вернуть переменную с преведущим состоянием и сделать чтобы после пряток он ждал
        {
            List<Health> enemies = _enemyInfo._npcEyes.WhatDoYouSee<Health>(_enemyInfo._visionParameters);
            Transform closestenemy = FindNearestOne.FindClosestObj(enemies, transform).transform;// передавать еще и список врагов
            float distance = Vector3.Distance(closestenemy.position, transform.position);// между состояниями или убрать рандомное
                                                                                         // зрение из глаз?

            float health = (float)_enemyInfo._getHealth?.Invoke();
            float critHealth = _enemyInfo._criticalHealth;
            float visionDistance = _enemyInfo._visionParameters.visionDistance;
            float combatDist = _enemyInfo._combatDistance;
            float runDist = _enemyInfo._runDistance;

            if (distance > visionDistance * combatDist && health > critHealth)
            {
                chooseId = 5;
            }
            else if (distance >= visionDistance * runDist && distance <= visionDistance * combatDist && health > critHealth)
            {
                chooseId = 6;
            }
            else
            {
                chooseId = 7;
            }
        }
        else
        {
            List<CarManager> cars = _enemyInfo._npcEyes.WhatDoYouSee<CarManager>(_enemyInfo._visionParameters, _exceptionLayerMask);
            if (HasAny(cars))
            {
                Transform nearestCar = FindNearestOne.FindClosestObj(cars, transform).transform;
                if (Vector3.Distance(transform.position, nearestCar.position) > 3)
                {
                    chooseId = 1;
                }
                else
                {
                    chooseId = 2;
                }
            }
            else
            {
                if (/*есть ли ловушки*/ 1 == 2)
                {
                    chooseId = 3;
                }
                else
                {
                    chooseId = 1;
                }
            }
        }
        _changeState?.Invoke(chooseId, _sawEnemies);
    }

    bool HasAny<T>(List<T> list) => list != null && list.Count > 0;
}
