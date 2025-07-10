using System.Collections.Generic;
using UnityEngine;

#if UNITY_SPLINES
using UnityEngine.Splines;
#endif

public class ConveyorBeltData : MonoBehaviour, ICookieReceiver
{
    [SerializeField]
    private bool debugMode = false; // For debugging purposes

    [Header("Belt Settings")]
    public int numberOfLanes = 5;
    public float laneSpacing = 0.08f; // Juster til din båndbredde
    public float speed = 1f;

    [Header("Lane Offsets (autofilled)")]
    public Vector3[] laneOffsets;

    [Header("Lineært bånd")]
    public Transform startPoint;
    public Transform endPoint;

#if UNITY_SPLINES
    [Header("Spline (valgfrit)")]
    public SplineContainer spline; // Sæt hvis du bruger spline
#endif

    [Header("Handoff")]
    public MonoBehaviour nextReceiver; // Drag næste bånd/maskine ind her

    [HideInInspector]
    public List<CookieData> cookies = new();

    void Awake()
    {
        laneOffsets = new Vector3[numberOfLanes];
        float start = -((numberOfLanes - 1) * laneSpacing) / 2f;
        for (int i = 0; i < numberOfLanes; i++)
            laneOffsets[i] = transform.right * (start + i * laneSpacing); // Right = tværs af bånd
    }

    void Update()
    {
        float step = speed * Time.deltaTime;
        for (int i = cookies.Count - 1; i >= 0; i--)
        {
            var cookie = cookies[i];
            cookie.progress += step / GetBeltLength();

            Vector3 basePos;
            Quaternion rot;
            float laneOffsetAmount = -((numberOfLanes - 1) * laneSpacing) / 2f + (cookie.lane * laneSpacing);

#if UNITY_SPLINES
            if (spline != null)
            {
                // Spline-bånd: brug tangent og lokal right-vector til lane-offset
                Vector3 tangent = ((Vector3)spline.EvaluateTangent(cookie.progress)).normalized;
                Vector3 right = Vector3.Cross(Vector3.up, tangent).normalized;
                basePos = (Vector3)spline.EvaluatePosition(cookie.progress);

                cookie.position = basePos + right * laneOffsetAmount;
                rot = Quaternion.LookRotation(tangent, Vector3.up);
            }
            else
            {
                // Lineært bånd: brug world right for lane-offset
                Vector3 dir = (endPoint.position - startPoint.position).normalized;
                Vector3 right = Vector3.Cross(Vector3.up, dir).normalized;
                basePos = Vector3.Lerp(startPoint.position, endPoint.position, cookie.progress);

                cookie.position = basePos + right * laneOffsetAmount;
                rot = Quaternion.LookRotation(dir, Vector3.up);
            }
#else
        // Ingen spline-support: fallback til lineært bånd
        Vector3 dir = (endPoint.position - startPoint.position).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, dir).normalized;
        basePos = Vector3.Lerp(startPoint.position, endPoint.position, cookie.progress);

        cookie.position = basePos + right * laneOffsetAmount;
        rot = Quaternion.LookRotation(dir, Vector3.up);
#endif

            cookie.rotation = rot;

            // Handoff til næste bånd/maskine hvis forbi enden
            if (cookie.progress >= 1f)
            {
                if (nextReceiver is ICookieReceiver receiver)
                    receiver.ReceiveCookie(cookie);
                cookies.RemoveAt(i);
            }
        }
    }




    float GetBeltLength()
    {
#if UNITY_SPLINES
        if (spline != null)
            return spline.CalculateLength();
#endif
        return Vector3.Distance(startPoint.position, endPoint.position);
    }

    // Interface: kan modtage cookies fra andre bånd/maskiner
    public void ReceiveCookie(CookieData incoming)
    {
#if UNITY_SPLINES
        if (spline != null)
        {
            var cookie = new CookieData
            {
                lane = Mathf.Clamp(incoming.lane, 0, numberOfLanes - 1),
                progress = 0f,
                type = incoming.type
            };
            // Cast float3 til Vector3:
            cookie.position = (Vector3)spline.EvaluatePosition(0f) + laneOffsets[cookie.lane];

            cookies.Add(cookie);
            if (debugMode) Debug.Log($"Cookie received by {name} (spline). Lane={cookie.lane}");
            return;
        }
#endif
        // Fallback: lineært bånd
        incoming.progress = 0f;
        incoming.position = startPoint.position + laneOffsets[incoming.lane];
        cookies.Add(incoming);
        if(debugMode) Debug.Log($"Cookie received by {name} (linear). Lane={incoming.lane}");
    }


}
