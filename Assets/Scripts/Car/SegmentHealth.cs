using System;
using UnityEngine;

public class SegmentHealth : MonoBehaviour
{
    [SerializeField] CarDetail carDetail;
    PrometeoCarController prometeoCarController;
    float limitSpeed;
    int health;
    Action<int> destroy;
    Action<int, int> takeDamage;
    int i;
    public void Initialize(Action<int> destroy, Action<int, int> takeDamage, int i, int health, float limitSpeed,
        PrometeoCarController prometeoCarController)
    {
        this.destroy = destroy;
        this.takeDamage = takeDamage;
        this.i = i;
        this.health = health;
        this.limitSpeed = limitSpeed;
        this.prometeoCarController = prometeoCarController;
    }
    private void OnTriggerEnter(Collider col)
    {
        float speed = prometeoCarController.GetSpeed();
        if (col.transform.tag == "Wall")
        {
            if (speed > limitSpeed)
            {
                int damage = (int)speed;
                TakeDamage(damage);
            }
        }
        else
        {
            PrometeoCarController car = col.gameObject.GetComponent<PrometeoCarController>();
            if (car != null)
            {
                float modul = Math.Abs(car.GetSpeed() - speed);
                if (modul > limitSpeed)
                {
                    int damage = (int)modul;
                    TakeDamage(damage);
                }
            }
        }
        
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            destroy?.Invoke(i);
            destroy = null;
            takeDamage = null;
            return;
        }
        takeDamage?.Invoke(health, i);
    }

    public CarDetail GetDetal() => carDetail;
}
