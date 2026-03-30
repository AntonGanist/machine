using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float _health;
    public float GetHealth() => _health;
}
