using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform obj;

    void Update()
    {
        transform.LookAt(obj.position);
    }
}
