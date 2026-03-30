using System;
using UnityEngine;

public class GetOutOfCar : MonoBehaviour
{
    [SerializeField] KeyCode _sitDown;
    Action<bool, Transform> _done;
    public void Initialize(Action<bool, Transform> done)
    {
        _done = done;
    }
    void Update()
    {
        if (Input.GetKeyDown(_sitDown))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 9))
            {
                CarDoor car = hit.collider.GetComponent<CarDoor>();
                if (car != null && !car.GetLock())
                {
                    Transform transform = car.GetExitPoint();
                    _done?.Invoke(true, transform);
                }
            }
        }
    }
}
