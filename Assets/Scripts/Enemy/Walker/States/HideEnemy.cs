using System.Collections.Generic;
using UnityEngine;

public class HideEnemy : EnemyState
{
    [SerializeField] float _cheakWorld;
    [SerializeField] float sideCheckDistance;
    [SerializeField] float turnAngle;
    float _timer;

    void TryFindShelterAndTakePoint()
    {
        FindCarDetals();
        List<Health> enemies = _enemyInfo._npcEyes.WhatDoYouSee<Health>(_enemyInfo._visionParameters);

        if (enemies == null || enemies.Count == 0)
        {
            _sawEnemies = false;
            return;
        }
        _sawEnemies = true;

        float distSqrThreshold = _enemyInfo._visionParameters.visionDistance * _enemyInfo._visionParameters.visionDistance;

        for (int i = 0; i < enemies.Count; i++)
        {
            var enemy = enemies[i];
            float dSqr = (enemy.transform.position - _myTransform.position).sqrMagnitude;
            if (dSqr > distSqrThreshold)
                continue;

            Transform shelter = _enemyInfo._findShelter?.Invoke(enemy.transform, _myTransform);

            _enemyInfo._enemyController.TakePoint(shelter, () => OnReachedShelterAndRotate(shelter), 0.1f);

            _enemyInfo._enemyHead.SetTarget(shelter);

            return;
        }
    }

    public override void EnterState(bool sawEnemies)
    {
        _sawEnemies = sawEnemies;
        _timer = 0f;
        //_exitTimer = 0f;
        TryFindShelterAndTakePoint();
    }

    public override void UpdateState()
    {
        _timer += Time.deltaTime;
        if (_timer < _cheakWorld) return;

        _timer = 0f;
        TryFindShelterAndTakePoint();
    }

    void OnReachedShelterAndRotate(Transform shelter)
    {
        Vector3 origin = _myTransform.position + Vector3.up * 0.5f; 
        Vector3 rightDir = _myTransform.right;
        Vector3 leftDir = -_myTransform.right;

        bool rightBlocked = Physics.Raycast(origin, rightDir, sideCheckDistance);
        bool leftBlocked = Physics.Raycast(origin, leftDir, sideCheckDistance);

        float chosenAngle = 0f;

        if (!rightBlocked && leftBlocked)
        {
            chosenAngle = turnAngle; 
        }
        else if (!leftBlocked && rightBlocked)
        {
            chosenAngle = -turnAngle; 
        }
        else if (!leftBlocked && !rightBlocked)
        {
            float rightDist = RayDistanceOrMax(origin, rightDir, sideCheckDistance);
            float leftDist = RayDistanceOrMax(origin, leftDir, sideCheckDistance);
            chosenAngle = (rightDist >= leftDist) ? turnAngle : -turnAngle;
        }
        else
        {
            chosenAngle = turnAngle;
        }

        _enemyInfo._enemyController.RotateByAngle(chosenAngle, 0.45f, () =>
        {
            CheckTransitions();
        });
    }

    float RayDistanceOrMax(Vector3 origin, Vector3 dir, float maxDist)
    {
        if (Physics.Raycast(origin, dir, out RaycastHit h, maxDist))
            return h.distance;
        return maxDist;
    }


    protected override void CheckTransitions()
    {
        _enemyInfo._enemyHead.StartRandomMovement();
        _changeState?.Invoke(0, _sawEnemies);
    }
}
