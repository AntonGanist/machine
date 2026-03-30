using System.Collections.Generic;
using UnityEngine;

public class RunnerEnemy : EnemyState
{
    [SerializeField] int _rays;
    [SerializeField] float _distance;

    [SerializeField] float _cheakWorld;
    float _timer;
    Transform enemyClosed;
    public override void EnterState(bool sawEnemies)
    {
        _sawEnemies = sawEnemies;
        enemyClosed = null;
        LookAround();
    }

    public override void UpdateState()
    {
        _timer += Time.deltaTime;
        if (_timer < _cheakWorld) return;
        _timer = 0f;
        CheckWorld();
    }
    void CheckWorld()
    {
        FindCarDetals();
        List<CarManager> cars = _enemyInfo._npcEyes.WhatDoYouSee<CarManager>(_enemyInfo._visionParameters, _exceptionLayerMask);
        List<Health> enemies = _enemyInfo._npcEyes.WhatDoYouSee<Health>(_enemyInfo._visionParameters);

        bool hasEnemies = enemies?.Count > 0;
        bool hasCars = cars?.Count > 0;
        _sawEnemies = true;

        if (hasEnemies && hasCars)
        {
            enemyClosed = FindNearestOne.FindClosestObj(enemies, transform).transform;
            Transform carClosed = FindNearestOne.FindClosestObj(cars, transform).transform;
            bool enemyCloser = Vector3.Distance(transform.position, enemyClosed.position) <
                              Vector3.Distance(transform.position, carClosed.position);
            if (enemyCloser) LookAround();
            else CheckTransitions();
        }
        else
        {
            if (hasEnemies) LookAround();
            else if (hasCars)
            {
                _sawEnemies = false;
                CheckTransitions();
            }
        }
    }


    protected override void CheckTransitions()
    {
        _changeState?.Invoke(0, _sawEnemies);
    }

    private void LookAround()
    {
        _sawEnemies = true;
        float maxDist = float.MinValue;
        Vector3 farthestPoint = Vector3.zero;
        for (int i = 0; i < _rays; i++)
        {
            float horizontalAngle = i * 360f / _rays;
            Vector3 rayDirection = Quaternion.AngleAxis(horizontalAngle, transform.up) * transform.forward * -1;
            Vector3 point;
            if(Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, _distance, _enemyInfo._visionParameters.layerMask))
            {
                point = hit.point;
            }
            else
            {
                point = transform.position + rayDirection * _distance;
            }
            Transform startPoint = transform;
            if (enemyClosed != null)
                startPoint = enemyClosed;

            float dist = Vector3.Distance(startPoint.position, point);
            if (dist > maxDist)
            {
                maxDist = dist;
                farthestPoint = point;
            }

        }
        _enemyInfo._enemyController.TakePoint(farthestPoint, ReachedPoint);
        _enemyInfo._enemyHead.SetTarget(farthestPoint);
    }
    void ReachedPoint()
    {
        _enemyInfo._enemyController.RotateByAngle(180, 0.45f, CheckTransitions);
    }
}
