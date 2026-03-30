using System.Collections;
using UnityEngine;

public class EnemyCarController : MonoBehaviour
{
    [Header("Path (trigger-driven)")]
    public Transform[] waypoints; 
    public bool loop = true;

    [Header("Simple obstacle detection")]
    public Transform rayTransform;
    public float rayHeight = 0.5f;
    public float forwardCheckDistance = 4f; 
    public float emergencyBrakeDistance = 1.2f; 
    [Tooltip("Радиус для SphereCast, чтобы машина не пролазила между узкими объектами")]
    public float rayRadius = 0.45f;
    public LayerMask obstacleMask = ~0;

    [Header("Reverse / recovery")]
    public float reverseDuration = 1.0f;
    public float postReverseClearDistance = 1.0f; 
    public float reverseCooldown = 0.6f; 
    public int maxReverseAttempts = 3; 
    [Tooltip("Если после реверса путь всё ещё заблокирован — ждать этот интервал перед следующей попыткой")]
    public float reverseAttemptDelay = 0.7f;
    [Tooltip("Множитель увеличения длины проверяющего луча на каждой следующей попытке")]
    public float checkDistanceIncreasePerAttempt = 0.5f;

    [Header("Steering smoothing")]
    public float steerSmoothSpeed = 6f; 
    public float steerDeadzone = 3f; 

    PrometeoCarController controller;

    int currentWP = 0;

    bool isReversing = false;
    Coroutine reverseRoutine = null;

    float desiredSteerAngle = 0f;

    public void Initialize(PrometeoCarController controller)
    {
        this.controller = controller;

        if (waypoints == null || waypoints.Length == 0)
            Debug.LogWarning($"{name}: waypoints not assigned.");

        ComputeDesiredSteerTowards(waypoints[currentWP].position);
    }

    void FixedUpdate()
    {
        if (isReversing)
        {
            ApplySteering();
            return;
        }

        Vector3 rayOrigin = rayTransform.position + Vector3.up * rayHeight;
        Vector3 forwardDir = rayTransform.forward;

        bool forwardHit = Physics.SphereCast(rayOrigin, rayRadius, forwardDir, out RaycastHit hit, forwardCheckDistance, obstacleMask);

        if (forwardHit)
        {
            // Проверяем, есть ли на объекте PrometeoCarController
            PrometeoCarController otherCar = hit.collider.GetComponent<PrometeoCarController>();
            if (otherCar != null)
            {
                // Это машина — не тормозим, просто выводим имя
                Debug.Log("Перед машиной: " + otherCar.name);
            }
            else
            {
                if (hit.distance <= emergencyBrakeDistance)
                {
                    controller.Brakes();
                    controller.ThrottleOff();
                }
            }
        }

        Vector3 target = waypoints[currentWP].position;
        //Vector3 toTarget = target - transform.position;
        //toTarget.y = 0f;

        ComputeDesiredSteerTowards(target);
        ApplySteering();

        if (!(forwardHit && hit.distance <= forwardCheckDistance))
        {
            controller.GoForward();
        }
        else
        {
            controller.ThrottleOff();
        }
    }

    void ComputeDesiredSteerTowards(Vector3 worldTarget)
    {
        Vector3 local = transform.InverseTransformPoint(worldTarget);
        if (Mathf.Approximately(local.sqrMagnitude, 0f))
        {
            desiredSteerAngle = 0f;
            return;
        }
        float angle = Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg; 
        if (Mathf.Abs(angle) < steerDeadzone) angle = 0f;
        desiredSteerAngle = Mathf.Clamp(angle, -controller.maxSteeringAngle, controller.maxSteeringAngle);
    }

    void ComputeDesiredSteerForReverseTowards(Vector3 worldTarget)
    {
        Vector3 local = transform.InverseTransformPoint(worldTarget);
        if (Mathf.Approximately(local.sqrMagnitude, 0f))
        {
            desiredSteerAngle = 0f;
            return;
        }
        float angle = Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg;
        angle = -angle;
        if (Mathf.Abs(angle) < steerDeadzone) angle = 0f;
        desiredSteerAngle = Mathf.Clamp(angle, -controller.maxSteeringAngle, controller.maxSteeringAngle);
    }

    void ApplySteering()
    {
        float curL = controller.frontLeftCollider.steerAngle;
        float curR = controller.frontRightCollider.steerAngle;

        float newL = Mathf.Lerp(curL, desiredSteerAngle, Time.fixedDeltaTime * steerSmoothSpeed);
        float newR = Mathf.Lerp(curR, desiredSteerAngle, Time.fixedDeltaTime * steerSmoothSpeed);

        controller.frontLeftCollider.steerAngle = newL;
        controller.frontRightCollider.steerAngle = newR;
    }

    void StartReverse()
    {
        if (isReversing) return;
        if (reverseRoutine != null) StopCoroutine(reverseRoutine);
        reverseRoutine = StartCoroutine(ReverseRoutine());
    }

    IEnumerator ReverseRoutine()
    {
        isReversing = true;

        int attempt = 0;
        float baseCheckDistance = forwardCheckDistance;

        while (attempt < maxReverseAttempts)
        {
            attempt++;

            controller.Brakes();
            controller.ThrottleOff();
            yield return new WaitForSeconds(0.05f);

            float t = 0f;
            while (t < reverseDuration)
            {
                controller.GoReverse();
                ComputeDesiredSteerForReverseTowards(waypoints[currentWP].position);
                ApplySteering();

                t += Time.deltaTime;
                yield return null;
            }

            controller.ThrottleOff();
            controller.ResetSteeringAngle();

            float increasedCheck = baseCheckDistance * (1f + (attempt - 1) * checkDistanceIncreasePerAttempt);

            float waitTime = 0f;
            bool pathClear = false;
            while (waitTime < reverseAttemptDelay)
            {
                Vector3 origin = rayTransform.position + Vector3.up * rayHeight;
                Vector3 forwardDir = rayTransform.forward;
                bool forwardNow = Physics.SphereCast(origin, rayRadius, forwardDir, out RaycastHit fHit, increasedCheck, obstacleMask, QueryTriggerInteraction.Ignore);

                if (!forwardNow || fHit.distance > postReverseClearDistance)
                {
                    pathClear = true;
                    break;
                }

                yield return new WaitForSeconds(0.12f);
                waitTime += 0.12f;
            }

            if (pathClear)
            {
                isReversing = false;
                reverseRoutine = null;
                forwardCheckDistance = baseCheckDistance;
                yield break;
            }

            yield return new WaitForSeconds(reverseCooldown);
        }

        AdvanceWaypoint();
        isReversing = false;
        reverseRoutine = null;

        forwardCheckDistance = baseCheckDistance;
    }

    void AdvanceWaypoint()// как будто можно вырезать
    {
        currentWP++;
        if (currentWP >= waypoints.Length)
        {
            if (loop) currentWP = 0;
            else currentWP = waypoints.Length - 1;
        }
    }

    // Метод, который должен вызвать ваш EnemyWayPoint (OnTriggerEnter)
    public void NotifyWaypointReached(Transform waypoint)
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == waypoint)
            {
                int next = i + 1;
                if (next >= waypoints.Length)
                {
                    next = loop ? 0 : waypoints.Length - 1;
                }
                currentWP = next;

                desiredSteerAngle = 0f;
                controller.ThrottleOff();
                controller.ResetSteeringAngle();
                if (reverseRoutine != null) { StopCoroutine(reverseRoutine); reverseRoutine = null; }
                isReversing = false;
                return;
            }
        }
    }
}
