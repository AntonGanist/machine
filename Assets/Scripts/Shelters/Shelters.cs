using UnityEngine;

public class Shelters : MonoBehaviour
{
    [SerializeField] private Transform[] sheltersPoints;

    [SerializeField] private float checkRadius;
    [SerializeField] private float moveStep;
    [SerializeField] private int maxIterations;     
    [SerializeField] private LayerMask parentLayerMask; 

    Collider _parentCollider;

    public void Initialize()
    {
        _parentCollider = transform.parent.GetComponent<Collider>();

        for (int i = 0; i < sheltersPoints.Length; i++)
        {
            Transform p = sheltersPoints[i];

            Collider[] hits = Physics.OverlapSphere(p.position, checkRadius, parentLayerMask);
            bool touchingParent = false;
            foreach (var c in hits)
            {
                if (c == _parentCollider)
                {
                    touchingParent = true;
                    break;
                }
            }

            if (!touchingParent)
                continue; 

            Vector3 parentCenter;
            try
            {
                parentCenter = _parentCollider.bounds.center;
            }
            catch
            {
                parentCenter = transform.parent != null ? transform.parent.position : Vector3.zero;
            }

            Vector3 dir = (p.position - parentCenter).normalized;
            if (dir.sqrMagnitude < 1e-6f) 
            {
                dir = transform.forward;
                if (dir.sqrMagnitude < 1e-6f) dir = Vector3.up;
            }

            int iter = 0;
            while (iter < maxIterations)
            {
                iter++;
                p.position += dir * moveStep;
                Collider[] newHits = Physics.OverlapSphere(p.position, checkRadius, parentLayerMask);
                bool nowTouching = false;
                foreach (var c in newHits)
                {
                    if (c == _parentCollider)
                    {
                        nowTouching = true;
                        break;
                    }
                }

                if (!nowTouching)
                {
                    Vector3 closest = _parentCollider.ClosestPoint(p.position);
                    if ((closest - p.position).sqrMagnitude > 1e-6f)
                        break; 
                }
            } 
        } 
    }

    public Transform[] GetShelters() => sheltersPoints;

    public void Update()
    {
        transform.rotation = Quaternion.identity;
    }
}
