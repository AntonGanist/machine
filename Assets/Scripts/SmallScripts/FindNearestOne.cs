using System.Collections.Generic;
using UnityEngine;

public static class FindNearestOne
{
    public static T FindClosestObj<T>(IEnumerable<T> collection, Transform pos, bool onliAktive = false) where T : Component
    {
        float closestDistance = float.MaxValue;
        T obj = null;
        foreach (var elem in collection)
        {
            float dist = Vector3.Distance(elem.transform.position, pos.position);
            if (dist < closestDistance)
            {
                if (!onliAktive || (onliAktive && obj.gameObject.activeSelf))
                {
                    closestDistance = dist;
                    obj = elem;
                }
            }
        }
        return obj;
    }
}
