using UnityEngine;

public class EnemyHead : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private FollowObject _follow;
    [SerializeField] private Transform _point; 
    [SerializeField] private Transform _point2; 

    [Header("Random point movement")]
    [SerializeField] private Vector3 randomRadius;
    [SerializeField] private float minTime;
    [SerializeField] private float maxTime;
    [SerializeField] private LayerMask obstructionMask; 
    [SerializeField] private int maxPlacementAttempts;
    [SerializeField] private float rayEpsilon;

    [SerializeField] private bool _headStateFollow;

    private Transform _currentTarget;
    private float _timer;
    private float _nextMoveTime;

    private void Awake()
    {
        _point2.parent = null;
    }
    private void Update()
    {
        if (!_headStateFollow)
        {
            _follow.ChengeTarget(_point);

            _timer += Time.deltaTime;
            if (_timer >= _nextMoveTime)
            {
                TryMovePointWithLOS();
                ScheduleNextMove();
            }
        }
        else
        {
            if (_point2.position == Vector3.zero)
                _follow.ChengeTarget(_currentTarget);
            else
                _follow.ChengeTarget(_point2);
        }
    }
    
    public void SetTarget(Transform target)
    {
        _currentTarget = target;
        _headStateFollow = true;
        _point2.position = Vector3.zero;
        _follow.ChengeTarget(_currentTarget);
    }
    public void SetTarget(Vector3 pos)
    {
        _point2.position = pos;
        _headStateFollow = true;
        _follow.ChengeTarget(_currentTarget);
    }

    public void StartRandomMovement()
    {
        _headStateFollow = false;
        _currentTarget = _point;
        _point2.position = Vector3.zero;
        _follow.ChengeTarget(_point);
        ScheduleNextMove();
    }

    private void ScheduleNextMove()
    {
        _nextMoveTime = Random.Range(minTime, maxTime);
        _timer = 0f;
    }

    private void TryMovePointWithLOS()
    {
        for (int attempt = 0; attempt < Mathf.Max(1, maxPlacementAttempts); attempt++)
        {
            Vector3 offsetLocal = new Vector3(
                Random.Range(-randomRadius.x, randomRadius.x),
                Random.Range(0, randomRadius.y),
                Random.Range(0, randomRadius.z)
            );

            Vector3 worldCandidate = transform.TransformPoint(offsetLocal);

            Vector3 from = transform.position;
            Vector3 dir = worldCandidate - from;
            float dist = dir.magnitude;
            if (dist <= Mathf.Epsilon) continue;
            Vector3 dirNorm = dir / dist;

            bool blocked = Physics.Raycast(from, dirNorm, dist - rayEpsilon, obstructionMask, QueryTriggerInteraction.Ignore);

            if (!blocked)
            {
                _point.position = worldCandidate;
                _follow.ChengeTarget(_point);
                return;
            }
        }

    }
}
