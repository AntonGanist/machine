using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    [SerializeField] CharacterController controller;

    [Header("Movement")]
    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;

    [Header("Gravity & Jump")]
    [SerializeField] float gravity;
    [SerializeField] float jumpHeight;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance;
    [SerializeField] LayerMask groundMask;

    [SerializeField] float stopDistance;
    float _useStopDistance;

    [Header("Ground Way")]
    [SerializeField] LayerMask layerMask;
    [SerializeField] float distance;
    [SerializeField] float obstacleSideSearchStep; 
    [SerializeField] float obstacleSideSearchMax;

    Vector3 targetPos;
    Transform targetPoint;

    Vector3 velocity;
    bool isGrounded;
    Action gotThere;

    Vector3 _savedTargetPos;
    Transform _savedTargetPoint;
    Action _savedGotThere;
    bool _isDetouring = false;
    bool _internalGotThere = false;

    bool _startRotate;
    private void Awake()
    {
        _useStopDistance = stopDistance;
    }

    public void TakePoint(Transform target, Action gotThere, float stopDist = 0)
    {
        targetPoint = target;
        targetPos = Vector3.zero;
        this.gotThere = gotThere;
        if (stopDist == 0)
            _useStopDistance = stopDistance;
        else
            _useStopDistance = stopDist;
    }
    public void TakePoint(Vector3 tergetPos, Action gotThere, float stopDist = 0)
    {
        targetPos = tergetPos;
        targetPoint = null;
        this.gotThere = gotThere;
        if (stopDist == 0)
            _useStopDistance = stopDistance;
        else
            _useStopDistance = stopDist;
    }


    void Update()
    {
        ApplyGravity();
        if (targetPoint != null || targetPos != Vector3.zero)
        {
            HandleMovementToPoint();
        }
    }
    void HandleMovementToPoint()
    {
        Vector3 toTarget; 
        if (targetPoint != null) 
            toTarget = targetPoint.position - transform.position; 
        else 
            toTarget = targetPos - transform.position;

        toTarget.y = 0f;
        float dist = toTarget.magnitude;
        if(!_startRotate)
            CheakWay((targetPoint != null) ? targetPoint.position : targetPos);

        if (dist <= _useStopDistance)
        {
            gotThere?.Invoke();

            if (!_internalGotThere)
            {
                gotThere = null;
                targetPos = Vector3.zero;
                targetPoint = null;
            }

            _internalGotThere = false;
            return;
        }


        Vector3 moveDir = toTarget.normalized;

        Vector3 horizontalVelocity = moveDir * speed;
        Vector3 totalMove = horizontalVelocity + new Vector3(0f, velocity.y, 0f);
        controller.Move(totalMove * Time.deltaTime);

        RotatetionObj(moveDir);
    }
    void RotatetionObj(Vector3 moveDir)
    {
        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion want = Quaternion.LookRotation(moveDir, Vector3.up);
            Vector3 e = want.eulerAngles;
            Quaternion wantY = Quaternion.Euler(0f, e.y, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, wantY, Time.deltaTime * rotationSpeed);
        }
    }

    void CheakWay(Vector3 target)
    {
        if (_isDetouring) return;

        Vector3 dir = target - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        Vector3 forwardDir = dir.normalized;

        if (Physics.Raycast(transform.position, forwardDir, out RaycastHit hit, distance, layerMask))
        {
            Collider obstacle = hit.collider;
            if (obstacle == null) return;

            float agentBaseY = (groundCheck != null) ? groundCheck.position.y : transform.position.y;
            float obstacleTopY = obstacle.bounds.max.y;
            float obstacleHeight = obstacleTopY - agentBaseY;

            if (obstacleHeight <= jumpHeight)
            {
                float hitDistance = Vector3.Distance(transform.position, hit.point);
                if (hitDistance < 1f)
                {
                    HandleJump();
                }
                return;
            }

            _savedTargetPos = targetPos;
            _savedTargetPoint = targetPoint;
            _savedGotThere = gotThere;

            _isDetouring = true;

            Vector3 foundPoint = FindFreAngle(agentBaseY, hit, obstacle);

            if (foundPoint != Vector3.zero)
            {
                targetPos = foundPoint;
                targetPoint = null;

                _internalGotThere = true;
                gotThere = () =>
                {
                    targetPos = _savedTargetPos;
                    targetPoint = _savedTargetPoint;
                    gotThere = _savedGotThere;
                    _savedGotThere = null;
                    _isDetouring = false;
                };

            }
            else
            {
                _isDetouring = false;
            }
        }
    }
    Vector3 FindFreAngle(float agentBaseY, RaycastHit hit, Collider obstacle)
    {
        Vector3 foundPoint = Vector3.zero;

        for (int side = 0; side < 2; side++)
        {
            Vector3 sideDir = (side == 0) ? transform.right : -transform.right;
            for (float s = obstacleSideSearchStep; s <= obstacleSideSearchMax; s += obstacleSideSearchStep)
            {
                Vector3 sample = hit.point + sideDir * s;
                sample.y = agentBaseY;

                Vector3 toSample = sample - transform.position;
                float sampleDist = toSample.magnitude;
                if (sampleDist < 0.01f) continue;

                Vector3 toSampleDir = toSample.normalized;

                if (Physics.Raycast(transform.position, toSampleDir, out RaycastHit hit2, sampleDist, layerMask))
                {
                    if (hit2.collider == obstacle)
                    {
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    foundPoint = sample;
                    break;
                }
            }
        }
        return foundPoint;
    }

    void HandleJump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y <= 0f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void ApplyGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
    }

    public void RotateByAngle(float relativeAngleDegrees, float duration, Action onComplete)
    {
        StopCoroutine(nameof(RotateCoroutine));
        _startRotate = true;
        StartCoroutine(RotateCoroutine(relativeAngleDegrees, duration, onComplete));
    }
    private IEnumerator RotateCoroutine(float relativeAngleDegrees, float duration, Action onComplete)
    {
        Quaternion start = transform.rotation;
        Quaternion end = start * Quaternion.Euler(0f, relativeAngleDegrees, 0f);
        float t = 0f;
        while (t < duration)
        {
            float factor = t / duration;
            transform.rotation = Quaternion.Slerp(start, end, factor);
            t += Time.deltaTime;
            yield return null;
        }

        transform.rotation = end;
        onComplete?.Invoke();
        _startRotate = false;

    }

}
