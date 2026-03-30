using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct VisionParameters
{
    public float visionDistance;
    public float visionAngle;
    public int visionRays;
    public int layesRays;
    public float layesAngle;
    public LayerMask layerMask;
}


public class NpcEyes : MonoBehaviour
{

    public bool showGizmos = true;
    public VisionParameters debugVisionParameters;
    public Color gizmoColor = new Color(1f, 1f, 0f, 0.6f);
    public bool useEvenDistributionForGizmos = true;

    public List<T> WhatDoYouSee<T>(VisionParameters visionParameters, LayerMask? exceptionLayerMask = null) where T : Component
    {
        List<T> detectedObjects = new List<T>();

        float visionDistance = visionParameters.visionDistance;
        float visionAngle = visionParameters.visionAngle;
        int visionRays = Mathf.Max(1, visionParameters.visionRays);
        int layesRays = Mathf.Max(1, visionParameters.layesRays);
        float layesAngle = visionParameters.layesAngle;
        LayerMask layerMask = visionParameters.layerMask;

        if (exceptionLayerMask.HasValue)
        {
            int lm = layerMask.value;
            int excl = exceptionLayerMask.Value.value;        
            lm &= ~excl;
            layerMask.value = lm;
        }

        for (int layer = 0; layer < layesRays; layer++)
        {
            float verticalAngle = Mathf.Lerp(
                -layesAngle / 2f,
                layesAngle / 2f,
                (layesRays == 1) ? 0.5f : (float)layer / (layesRays - 1)
            );

            for (int i = 0; i < visionRays; i++)
            {
                float horizontalAngle = UnityEngine.Random.Range(-visionAngle / 2f, visionAngle / 2f);

                Vector3 rayDirection =
                    Quaternion.AngleAxis(horizontalAngle, transform.up) *
                    Quaternion.AngleAxis(verticalAngle, transform.right) *
                    transform.forward;

                if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, visionDistance, layerMask))
                {
                    T component = hit.collider.GetComponent<T>();
                    if (component != null && !detectedObjects.Contains(component))
                    {
                        detectedObjects.Add(component);
                    }
                }
            }
        }

        return detectedObjects;
    }

    void OnDrawGizmosSelected()
    {

        VisionParameters p = debugVisionParameters;

        float visionDistance = p.visionDistance;
        float visionAngle = p.visionAngle;
        int visionRays = Mathf.Max(1, p.visionRays);
        int layesRays = Mathf.Max(1, p.layesRays);
        float layesAngle = p.layesAngle;

        Gizmos.color = gizmoColor;

        int perimeterSteps = Mathf.Max(6, visionRays);
        for (int i = 0; i < perimeterSteps; i++)
        {
            float t1 = (float)i / perimeterSteps;
            float t2 = (float)(i + 1) / perimeterSteps;
            float ang1 = Mathf.Lerp(-visionAngle / 2f, visionAngle / 2f, t1);
            float ang2 = Mathf.Lerp(-visionAngle / 2f, visionAngle / 2f, t2);

            Vector3 dir1 = Quaternion.AngleAxis(ang1, transform.up) * transform.forward;
            Vector3 dir2 = Quaternion.AngleAxis(ang2, transform.up) * transform.forward;

            Gizmos.DrawLine(transform.position + dir1 * visionDistance, transform.position + dir2 * visionDistance);
        }

        for (int layer = 0; layer < layesRays; layer++)
        {
            float verticalAngle = Mathf.Lerp(
                -layesAngle / 2f,
                layesAngle / 2f,
                (layesRays == 1) ? 0.5f : (float)layer / (layesRays - 1)
            );

            for (int i = 0; i < visionRays; i++)
            {
                float horizontalAngle;
                if (useEvenDistributionForGizmos)
                {
                    horizontalAngle = Mathf.Lerp(-visionAngle / 2f, visionAngle / 2f, (visionRays == 1) ? 0.5f : (float)i / (visionRays - 1));
                }
                else
                {
                    horizontalAngle = UnityEngine.Random.Range(-visionAngle / 2f, visionAngle / 2f);
                }

                Vector3 rayDirection =
                    Quaternion.AngleAxis(horizontalAngle, transform.up) *
                    Quaternion.AngleAxis(verticalAngle, transform.right) *
                    transform.forward;

                Vector3 to = transform.position + rayDirection * visionDistance;

                Gizmos.DrawRay(transform.position, rayDirection * visionDistance);
                Gizmos.DrawWireSphere(to, Mathf.Min(0.12f, visionDistance * 0.02f));
            }
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 0.05f);
    }

}
