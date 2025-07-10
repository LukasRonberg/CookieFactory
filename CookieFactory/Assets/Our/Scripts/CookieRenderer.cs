using System.Collections.Generic;
using UnityEngine;

public class CookieRenderer : MonoBehaviour
{
    [SerializeField] private ConveyorBeltData belt;
    [SerializeField] private Mesh normalMesh;
    [SerializeField] private Mesh badMesh;
    [SerializeField] private Material normalMat;
    [SerializeField] private Material badMat;

    private const int MAX_INSTANCES = 1023; // URP DrawMeshInstanced limit

    void LateUpdate()
    {
        if (belt == null || belt.cookies == null || normalMesh == null || normalMat == null) return;

        List<Matrix4x4> normals = new();
        List<Matrix4x4> bads = new();

        foreach (var c in belt.cookies)
        {
            // Fail-safe: check rotation validity
            Quaternion rot = c.rotation;
            if (rot == default || rot.w == 0f || float.IsNaN(rot.x) || float.IsNaN(rot.y) || float.IsNaN(rot.z) || float.IsNaN(rot.w))
                rot = Quaternion.identity;

            var m = Matrix4x4.TRS(c.position, rot, Vector3.one * 0.2f);
            if (c.type == CookieType.Normal) normals.Add(m);
            else bads.Add(m);
        }

        // Render normals (in batches of 1023)
        for (int i = 0; i < normals.Count; i += MAX_INSTANCES)
        {
            int count = Mathf.Min(MAX_INSTANCES, normals.Count - i);
            Graphics.DrawMeshInstanced(normalMesh, 0, normalMat, normals.GetRange(i, count).ToArray());
        }

        // Render bads (if any)
        if (badMesh != null && badMat != null)
        {
            for (int i = 0; i < bads.Count; i += MAX_INSTANCES)
            {
                int count = Mathf.Min(MAX_INSTANCES, bads.Count - i);
                Graphics.DrawMeshInstanced(badMesh, 0, badMat, bads.GetRange(i, count).ToArray());
            }
        }
    }
}
