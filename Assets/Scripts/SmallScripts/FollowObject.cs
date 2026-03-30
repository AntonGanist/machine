using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] bool followX = true;
    [SerializeField] bool followY = true;

    [SerializeField] Vector2 xRotationLimits;
    [SerializeField] Vector2 yRotationLimits;

    [SerializeField] public float yawOffset;

    Transform target;

    public void ChengeTarget(Transform target) => this.target = target;
    void Update()
    {
        if (target == null) return;

        Vector3 dirWorld = target.position - transform.position;
        if (dirWorld.sqrMagnitude < 1e-8f) return;

        float targetY_world = transform.eulerAngles.y; 
        float parentYaw = 0f;

        if (transform.parent != null)
            parentYaw = transform.parent.eulerAngles.y;

        if (followY)
        {
            float desiredYawWorld = Mathf.Atan2(dirWorld.x, dirWorld.z) * Mathf.Rad2Deg + yawOffset;

            float desiredLocalYaw = Mathf.DeltaAngle(parentYaw, desiredYawWorld);

            float clampedLocalYaw = Mathf.Clamp(desiredLocalYaw, yRotationLimits.x, yRotationLimits.y);

            targetY_world = parentYaw + clampedLocalYaw;
        }
        else
        {
            targetY_world = transform.eulerAngles.y;
        }

        float targetX = transform.eulerAngles.x;
        if (followX)
        {
            float horizontalDist = new Vector3(dirWorld.x, 0f, dirWorld.z).magnitude;
            float desiredPitch = -Mathf.Atan2(dirWorld.y, horizontalDist) * Mathf.Rad2Deg;

            float clampedPitch = Mathf.Clamp(desiredPitch, xRotationLimits.x, xRotationLimits.y);

            targetX = clampedPitch;
        }

        Quaternion want = Quaternion.Euler(targetX, targetY_world, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, want, Time.deltaTime * speed);
    }

}
