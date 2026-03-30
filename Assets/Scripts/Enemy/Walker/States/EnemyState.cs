using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable] public class EnemyBaseInformation
{
    public NpcEyes _npcEyes;
    public EnemyController _enemyController;
    public EnemyHead _enemyHead;
    public VisionParameters _visionParameters;
    public Func<float> _getHealth;
    public Func<Transform, Transform, Transform> _findShelter;
    public float _criticalHealth;
    public float _combatDistance;
    public float _runDistance;
}

public abstract class EnemyState : MonoBehaviour 
{
    private int _id;
    protected Action<int, bool> _changeState;
    protected bool _sawEnemies;
    protected Transform _myTransform;
    protected EnemyBaseInformation _enemyInfo;
    protected LayerMask _exceptionLayerMask;
    protected Action<List<CarDetail>> _seeDetal;
    protected Action<Item> _takeItem;
    public virtual void Initialize(int id, Action<int, bool> Change, EnemyBaseInformation enemyBaseInformation, LayerMask exceptionLayerMask,
        Action<List<CarDetail>> seeDetal, Action<Item> takeItem)
    {
        _id = id;
        _changeState = Change;
        _myTransform = transform;
        _enemyInfo = enemyBaseInformation;
        _exceptionLayerMask = exceptionLayerMask;
        _seeDetal = seeDetal;
        _takeItem = takeItem;
    }

    public abstract void EnterState(bool sawEnemies);
    public abstract void UpdateState();

    protected abstract void CheckTransitions();

    public virtual void FindCarDetals()
    {
        List<CarDetail> carDetals = _enemyInfo._npcEyes.WhatDoYouSee<CarDetail>(_enemyInfo._visionParameters);
        if (carDetals != null && carDetals.Count != 0)
        {
            _seeDetal?.Invoke(carDetals);
        }
    }

    public virtual int GetId() => _id;
}
