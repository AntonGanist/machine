using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable] public struct HealthElement
{
    public Slider slider;
    public SegmentHealth healthSegment;
    public int health;
    public Transform destroySegment;
    public CarSegment carSegment;
    public GameObject[] segments;
    public float mass;
}
public class CarHealth : MonoBehaviour
{
    [SerializeField] private HealthElement[] healthElements;
    [SerializeField] private float limitSpeed;
    private int power;
    private PrometeoCarController prometeoCarController;
    private Rigidbody rigidbody;
    private CarCondition carCondition;
    public void Initialize(PrometeoCarController prometeoCarController, Rigidbody rigidbody)
    {
        this.prometeoCarController = prometeoCarController;
        this.rigidbody = rigidbody;
        int numberMotors = 0;
        int numberBody = 0;
        int numberTire = 0;
        for (int i = 0; i < healthElements.Length; i++)
        {
            healthElements[i].healthSegment.Initialize(DestroySegment, TakeDamage, i,
                healthElements[i].health, limitSpeed, prometeoCarController);

            if (healthElements[i].carSegment == CarSegment.Motor) numberMotors++;
            else if (healthElements[i].carSegment == CarSegment.Body) numberBody++;
            else numberTire++;

            if (healthElements[i].slider == null) continue;
            healthElements[i].slider.maxValue = healthElements[i].health;
            healthElements[i].slider.value = healthElements[i].health;
        }
        carCondition.MaxMotor = numberMotors;
        carCondition.CurrentMotor = numberMotors;
        carCondition.MaxTire = numberTire;
        carCondition.CurrentTire = numberTire;
        carCondition.MaxtBody = numberBody;
        carCondition.CurrentBody = numberBody;
        power = prometeoCarController.maxSpeed/ numberMotors;
    }
    void TakeDamage(int health, int i)
    {
        if(healthElements[i].slider != null)
            healthElements[i].slider.value = health;
    }

    void DestroySegment(int i)
    {
        healthElements[i].slider.value = 0;

        float mass = healthElements[i].mass;
        Transform segment = healthElements[i].healthSegment.transform;
        Transform debris = Instantiate(healthElements[i].destroySegment, 
            segment.position, segment.rotation);

        Rigidbody rb = debris.GetComponent<Rigidbody>();
        float launchForce = mass*0.05f; 
        rb.AddForce(segment.up * launchForce, ForceMode.Impulse);

        for (int j = 0; j < healthElements[i].segments.Length; j++)
            healthElements[i].segments[j].SetActive(false);

        rigidbody.mass -= mass;
        if (healthElements[i].carSegment == CarSegment.Motor)
        {
            prometeoCarController.maxSpeed -= power;
            carCondition.CurrentMotor--;
        }
        else if (healthElements[i].carSegment == CarSegment.Body)
        {
            carCondition.CurrentBody--;
        }
        else
        {
            carCondition.CurrentTire--;
        }

        CheckDestroyCar();
    }
    
    void CheckDestroyCar()
    {
        int tiresAlive = 0;
        int motorsAlive = 0;

        foreach (var element in healthElements)
        {
            if (element.healthSegment.gameObject.activeSelf)
            {
                if (element.carSegment == CarSegment.Tire)
                    tiresAlive++;

                else if (element.carSegment == CarSegment.Motor)
                    motorsAlive++;
            }
        }

        if (tiresAlive <= 1)
        {
            DestroyCar("Осталось слишком мало колёс.");
            return;
        }

        if (motorsAlive == 0)
        {
            DestroyCar("Уничтожены все моторы.");
            return;
        }
    }
    void DestroyCar(string reason)
    {
        Debug.Log($"[Health] Машина уничтожена: {reason}");
        //gameObject.SetActive(false);
    }

    public CarCondition GetCondition() => carCondition;

    public void FixCar(CarDetail detail)
    {
        if (carCondition.Working())
        {
            for(int i = 0; i < healthElements.Length; i++)
            {
                //if (healthElements[i].carSegment == detail)

            }
        }
    }
}
public class CarCondition
{
    public int MaxMotor;
    public int CurrentMotor;
    public int MaxtBody;
    public int CurrentBody;
    public int CurrentTire;
    public int MaxTire;

    public bool Working()
    {
        if(MaxMotor == CurrentMotor && MaxtBody == CurrentBody && MaxTire == CurrentTire)
            return true;
        return false;
    }
}

public enum CarSegment
{
    Body = 1,
    Tire = 2,
    Motor = 3
}