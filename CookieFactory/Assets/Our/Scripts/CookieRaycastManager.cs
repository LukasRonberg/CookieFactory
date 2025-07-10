using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CookieRaycastManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private List<ConveyorBeltData> allBelts;
    [SerializeField] private float cookieRadius = 0.05f;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            int beltIdx, cookieIdx;
            if (FindHitCookie(cam, allBelts, cookieRadius, out beltIdx, out cookieIdx))
            {
                Debug.Log($"Cookie hit and removed! ?? Belt: {beltIdx} | Index: {cookieIdx}");
                allBelts[beltIdx].cookies.RemoveAt(cookieIdx);
            }
        }
    }

    // Returnerer true hvis noget rammes. Finder nærmeste cookie på tværs af ALLE bånd
    bool FindHitCookie(Camera cam, List<ConveyorBeltData> belts, float radius, out int beltIndex, out int cookieIndex)
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        float minDist = float.MaxValue;
        beltIndex = -1;
        cookieIndex = -1;

        for (int b = 0; b < belts.Count; b++)
        {
            var cookies = belts[b].cookies;
            for (int c = 0; c < cookies.Count; c++)
            {
                if (RayIntersectsSphere(ray, cookies[c].position, radius, out float dist))
                {
                    if (dist < minDist)
                    {
                        minDist = dist;
                        beltIndex = b;
                        cookieIndex = c;
                    }
                }
            }
        }
        return beltIndex != -1;
    }

    bool RayIntersectsSphere(Ray ray, Vector3 center, float radius, out float t)
    {
        Vector3 oc = ray.origin - center;
        float a = Vector3.Dot(ray.direction, ray.direction);
        float b = 2.0f * Vector3.Dot(oc, ray.direction);
        float c = Vector3.Dot(oc, oc) - radius * radius;
        float discriminant = b * b - 4 * a * c;
        if (discriminant > 0)
        {
            t = (-b - Mathf.Sqrt(discriminant)) / (2.0f * a);
            return t > 0;
        }
        t = -1;
        return false;
    }
}
