using System;
using UnityEngine;

public class CarDoor : MonoBehaviour
{
    [SerializeField] Transform _exitPoint;
    Action<Transform, bool> _startCar;
    private bool _lock;
    public void Initialize(Action<Transform, bool> startCar)
    {
        _startCar = startCar;
    }

    public void StartCar(Transform driver, bool player)
    {
        if(_lock) return;
        _startCar?.Invoke(driver, player);
    }
    public void ChaeangeLock() => _lock = !_lock;
    public bool GetLock() => _lock;
    public Transform GetExitPoint() => _exitPoint;
}
