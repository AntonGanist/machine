using System.Collections.Generic;
using UnityEngine;

public class IdleEnemy : EnemyState
{
    [SerializeField] float _cheakWorld;
    [SerializeField] float _waitTime;
    float _timer;
    float _waitTimer;

    public override void EnterState(bool sawEnemies)
    {
        _sawEnemies = sawEnemies;
        _enemyInfo._enemyHead.StartRandomMovement();
        _timer = 0f;
        _waitTimer = 0f;
    }

    public override void UpdateState()
    {
        _timer += Time.deltaTime;
        _waitTimer += Time.deltaTime;

        if (_waitTimer >= _waitTime)
        {
            CheakWorld(false);
            CheckTransitions();
            return;
        }

        if (_timer >= _cheakWorld)
        {
            _timer = 0f;
            CheakWorld();
        }

    }
    void CheakWorld(bool basic = true)
    {
        FindCarDetals();
        List<Health> enemies = _enemyInfo._npcEyes.WhatDoYouSee<Health>(_enemyInfo._visionParameters);
        if (enemies != null && enemies.Count != 0)
        {
            _sawEnemies = true;
            if(basic)
            {
                CheckTransitions();
            }
        }
        else
        {
            _sawEnemies = false;
            List<CarManager> cars = _enemyInfo._npcEyes.WhatDoYouSee<CarManager>(_enemyInfo._visionParameters, _exceptionLayerMask);
            if (cars != null && cars.Count != 0)
            {
                if (basic)
                {
                    CheckTransitions();
                }
            }
        }
    }
    protected override void CheckTransitions()
    {

        _changeState?.Invoke(0, _sawEnemies);
    }
}