using System.Collections.Generic;
using UnityEngine;


public class MovmentEnemy : EnemyState
{

    [SerializeField] float _cheakWorld;
    float _timer;

    CarManager _chosedCar;
    Transform _nextPoint;
    Transform[] _allPoint;

    public override void EnterState(bool sawEnemies)
    {
        _chosedCar = null;
        _sawEnemies = sawEnemies;
        bool seeCar = CheakWorld();
        if(!seeCar)
        {
            _enemyInfo._enemyController.TakePoint(_nextPoint, ReachedPoint, 0.1f);
        }
    }
    public void TakePoints(Transform[] transforms)
    {
        _allPoint = transforms;
        _nextPoint = _allPoint[0];
    }
    public void ChangePoint(Transform point)
    {
        for(int i = 0; i < _allPoint.Length; i++)
        {
            if (point == _allPoint[i])
            {
                int nextIndex = (i + 1) % _allPoint.Length;
                _nextPoint = _allPoint[nextIndex];
                break;
            }
        }
    }

    void ReachedCar()
    {
        if (!_chosedCar.HasDriver())
        {
            if (_chosedCar.GetCondition().Working())
                _chosedCar.StartCar(transform, false);
            else
                CheckTransitions();
        }
        else
        {
            _enemyInfo._enemyController.TakePoint(_nextPoint, ReachedPoint);
        }
    }
    void ReachedPoint()
    {
        ChangePoint(_nextPoint);
        _enemyInfo._enemyController.TakePoint(_nextPoint, ReachedPoint);
    }


    bool CheakWorld()
    {
        FindCarDetals();
        List<Health> enemy = _enemyInfo._npcEyes.WhatDoYouSee<Health>(_enemyInfo._visionParameters);
        if (enemy != null && enemy.Count != 0)
        {
            _sawEnemies = true;
            CheckTransitions();
            return true;
        }

        List<CarManager> cars = _enemyInfo._npcEyes.WhatDoYouSee<CarManager>(_enemyInfo._visionParameters, _exceptionLayerMask);
        if (cars != null && cars.Count != 0)
        {
            _chosedCar = FindNearestOne.FindClosestObj(cars, transform); 
            _enemyInfo._enemyController.TakePoint(_chosedCar.transform, ReachedCar, 2);
            _sawEnemies = false;
            return true;
        }
        return false;
    }
    public override void UpdateState()
    {
        _timer += Time.deltaTime;

        if (_timer >= _cheakWorld)
        {
            _timer = 0f;
            CheakWorld();
        }
    }

    protected override void CheckTransitions()
    {
        _changeState?.Invoke(0, _sawEnemies);
    }
}
