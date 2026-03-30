using UnityEngine;

public class CarManager : MonoBehaviour// метод починки авто
{
    [SerializeField] PrometeoCarController _prometeoCarController;
    [SerializeField] CarHealth _carHealth;
    [SerializeField] CarDoor[] _carDoors;

    [SerializeField] EnemyCarController _enemyController;

    [SerializeField] CameraCar _cameraCar;
    [SerializeField] GetOutOfCar _getOutOfCar;

    [SerializeField] Rigidbody _rigidbody;
    Transform _driver;
    private void Awake()
    {
        Initialize();
    }
    public void Initialize()
    {
        _carHealth.Initialize(_prometeoCarController, _rigidbody);
        _enemyController.Initialize(_prometeoCarController);
        for(int i = 0;  i < _carDoors.Length; i++)
        {
            _carDoors[i].Initialize(StartCar);
        }
        _getOutOfCar.Initialize(ExitCar);
    }

    public void StartCar(Transform driver, bool player)
    {
        if(_driver != null) return;
        _driver = driver;
        _driver.gameObject.SetActive(false);
        _prometeoCarController.enabled = true;
        if (player)
        {
            _cameraCar.gameObject.SetActive(true);
            _prometeoCarController.externalControl = false;
        }
        else
        {
            _enemyController.enabled = true;
            _prometeoCarController.externalControl = true;
        }
    }
    void ExitCar(bool player, Transform exitPoint)
    {
        _driver.position = exitPoint.position;
        _driver.gameObject.SetActive(true);
        _driver = null;

        _prometeoCarController.enabled = false;
        if (player)
            _cameraCar.gameObject.SetActive(false);
        else
            _enemyController.enabled = false;
    }
    
    public bool HasDriver() => _driver != null;
    public CarCondition GetCondition() => _carHealth.GetCondition();
}
