using UnityEngine;

public class CameraCar : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f;

    [SerializeField] private float minX = -60f;
    [SerializeField] private float maxX = 60f;

    [SerializeField] private float minY = -90f;
    [SerializeField] private float maxY = 90f;

    private float rotationX;
    private float rotationY;

    void Start()
    {
        Vector3 angles = transform.localEulerAngles;
        rotationX = angles.x;
        rotationY = angles.y;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationY += mouseX * rotationSpeed * Time.deltaTime;
        rotationX -= mouseY * rotationSpeed * Time.deltaTime;

        rotationX = Mathf.Clamp(rotationX, minX, maxX);
        rotationY = Mathf.Clamp(rotationY, minY, maxY);

        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
}
