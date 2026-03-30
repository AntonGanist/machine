using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayerClasses _player;

    [SerializeField] SheltersManager _sheltersManager;
    [SerializeField] EnemyManager enemyManager;
    void Awake()
    {
        Initialize();
    }
    public void Initialize()
    {
        _sheltersManager.Initialize("Shelter");
        enemyManager.Initialize(_sheltersManager.GetShelter, _player.transform);
    }
}
