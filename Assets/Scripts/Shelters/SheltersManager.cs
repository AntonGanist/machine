using System.Collections.Generic;
using UnityEngine;

public class SheltersManager : MonoBehaviour
{
    [SerializeField] private Transform shelterPrefab;

    [SerializeField] private LayerMask obstructionLayers;
    private List<Shelters> shelters = new List<Shelters>();

    public void Initialize(string tag)
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);

        if (taggedObjects.Length == 0) return;

        for (int i = 0; i < taggedObjects.Length; i++)
        {
            Transform taggedPoint = taggedObjects[i].transform;
            CreateShelter(taggedPoint);
        }
    }
    public void CreateShelter(Transform shelter)
    {
        Transform shelterInstance = Instantiate(shelterPrefab,
            shelter.position, shelter.rotation, shelter);

        Shelters shelterComponent = shelterInstance.GetComponent<Shelters>();

        shelterComponent.Initialize();
        shelters.Add(shelterComponent);
    }

    public Transform GetShelter(Transform point1, Transform customer)
    {
        Transform best = null;
        float bestDistSqr = float.MaxValue;

        const float epsilon = 0.01f;

        for (int i = 0; i < shelters.Count; i++)
        {
            Shelters s = shelters[i];
            Transform[] points = s.GetShelters();

            for (int j = 0; j < points.Length; j++)
            {
                Transform candidate = points[j];

                Vector3 from = point1.position;
                Vector3 to = candidate.position;
                Vector3 dir = to - from;
                float dist = dir.magnitude;
                if (dist <= Mathf.Epsilon) continue;

                Vector3 dirNorm = dir / dist;

                RaycastHit[] hits = Physics.RaycastAll(from, dirNorm, dist - epsilon, obstructionLayers, QueryTriggerInteraction.Ignore);

                float nearestHitDist = float.MaxValue;
                RaycastHit nearestHit = default;
                bool anyBlockingHit = false;

                for (int h = 0; h < hits.Length; h++)
                {
                    RaycastHit hh = hits[h];

                    if (hh.collider != null && hh.collider.transform != null)
                    {
                        if (hh.collider.transform.IsChildOf(customer))
                        {
                            continue;
                        }
                    }

                    if (hh.distance < nearestHitDist)
                    {
                        nearestHitDist = hh.distance;
                        nearestHit = hh;
                        anyBlockingHit = true;
                    }
                }

                bool blocked = anyBlockingHit;

                if (blocked)
                {
                    float distToCustomerSqr = (candidate.position - customer.position).sqrMagnitude;
                    if (distToCustomerSqr < bestDistSqr)
                    {
                        bestDistSqr = distToCustomerSqr;
                        best = candidate;
                    }
                }
            }
        }

        return best;
    }


}