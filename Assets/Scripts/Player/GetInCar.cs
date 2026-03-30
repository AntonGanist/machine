using UnityEngine;

public class GetInCar : MonoBehaviour
{
    [SerializeField] KeyCode _sitDown;
    [SerializeField] float distance;
    [SerializeField] Transform camera;
    [SerializeField] LayerMask layerMask;

    void Update()
    {
        if (Input.GetKeyDown(_sitDown))
        {
            Ray ray = new Ray(camera.position, camera.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, distance, layerMask))
            {
                CarDoor car = hit.collider.GetComponent<CarDoor>();
                if (car != null)
                {
                    car.StartCar(transform, true);
                }
            }
        }
    }
}
