using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixCarEnemy : EnemyState
{
    [SerializeField] float _cheakWorld;
    [SerializeField] float _exitTime;
    [SerializeField] float _minSearchDistance;
    [SerializeField] float _maxSearchDistance;

    CarCondition _chosedCarCondition;
    CarDetail _chosedDetal;

    List<CarDetail> _listCarDetails;
    float _timer;
    float _exitTimer;

    public override void EnterState(bool sawEnemies)
    {
        _chosedCarCondition = new CarCondition();
        _listCarDetails = null;
        _chosedDetal = null;
        _sawEnemies = sawEnemies;
        _exitTimer = 0;

        List<CarManager> cars = _enemyInfo._npcEyes.WhatDoYouSee<CarManager>(_enemyInfo._visionParameters, _exceptionLayerMask);
        CarManager chosedCar = FindNearestOne.FindClosestObj(cars, transform);
        _chosedCarCondition = chosedCar.GetCondition();
        CheckPartsCompliance();
    }
    public void TakeCarDetals(List<CarDetail> list)
    {
        _listCarDetails = list;
        for(int i = 0;  i < _listCarDetails.Count; i++)
        {
            if(_listCarDetails[i] == null || !_listCarDetails[i].gameObject.activeSelf)
            {
                _listCarDetails.RemoveAt(i);
            }
        }
    } 

    public override void UpdateState()
    {
        _timer += Time.deltaTime;
        _exitTimer += Time.deltaTime;
        if (_exitTimer > _exitTime) CheckTransitions();
        if (_timer < _cheakWorld) return;
        _timer = 0f;
        CheckWorld();
    }

    protected override void CheckTransitions()
    {
        _changeState?.Invoke(0, _sawEnemies);
    }

    private void PointSelection()
    {
        if(_listCarDetails != null)
        {
            _chosedDetal = FindNearestOne.FindClosestObj(_listCarDetails, transform);
            float dist = Vector3.Distance(transform.position, _chosedDetal.transform.position);
            if (dist > _maxSearchDistance)
            {
                LookAround();
            }
            else
            {
                _enemyInfo._enemyController.TakePoint(_chosedDetal.transform, TakeDetal);
            }
        }
        else
        {
            LookAround();
        }
    }

    void LookAround()
    {
        int randRay = Random.Range(0, 20);
        for (int i = 0; i < 20; i++)
        {
            if (i != randRay) continue;

            float distance = Random.Range(_minSearchDistance, _maxSearchDistance);

            float horizontalAngle = i * 18;
            Vector3 rayDirection = Quaternion.AngleAxis(horizontalAngle, transform.up) * transform.forward * -1;
            Vector3 point;
            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, distance, _enemyInfo._visionParameters.layerMask))
            {
                point = hit.point;
            }
            else
            {
                point = transform.position + rayDirection * distance;
            }
            _enemyInfo._enemyController.TakePoint(point, Wait);
            break;
        }
    }
    void Wait()
    {
        StartCoroutine(WaitEnumerator());
    }
    private IEnumerator WaitEnumerator()
    {
        yield return new WaitForSeconds(1);
        PointSelection();
    }

    void CheckWorld()
    {
        List<Health> enemies = _enemyInfo._npcEyes.WhatDoYouSee<Health>(_enemyInfo._visionParameters);
        if(enemies != null && enemies.Count != 0)
        {
            _sawEnemies = true;
            CheckTransitions();
            return;
        }

        List<CarDetail> carDetals = _enemyInfo._npcEyes.WhatDoYouSee<CarDetail>(_enemyInfo._visionParameters);
        if (carDetals != null && carDetals.Count != 0)
        {
            foreach (var detail in carDetals)
            {
                if (!_listCarDetails.Contains(detail))
                {
                    _listCarDetails.Add(detail);
                }
            }
            StopAllCoroutines();
            PointSelection();
        }
    }

    void CheckPartsCompliance()
    {
        int currentMotor = 0;
        int currentTire = 0;
        for(int i = 0; i < _listCarDetails.Count; i++)
        {
            if (_listCarDetails[i].GetSegmentType() == CarSegment.Motor)
                currentMotor++;
            else if (_listCarDetails[i].GetSegmentType() == CarSegment.Tire)
                currentTire++;
        }
        if((_chosedCarCondition.CurrentMotor + currentMotor)/ _chosedCarCondition.MaxMotor > 0 &&
            (_chosedCarCondition.CurrentTire + currentTire) / _chosedCarCondition.MaxTire >= 1)
        {
            // чиним тачку
        }
        else
        {
            //продолжить поиск
        }
    }

    void TakeDetal()
    {
        _takeItem?.Invoke(_chosedDetal);
        _listCarDetails.Remove(_chosedDetal);
        _chosedDetal = null;
        LookAround();
    }
}
