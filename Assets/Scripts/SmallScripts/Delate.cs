using UnityEngine;

public class Delate : MonoBehaviour
{
    public float limit;
    public float time;
    void Update()
    {
        time += Time.deltaTime;
        if (time > limit)
        {
            Destroy(gameObject);
        }
    }
}
