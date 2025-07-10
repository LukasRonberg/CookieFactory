using UnityEngine;

public enum CookieType { Normal, Bad, Burnt }

public class CookieData
{
    public int lane;
    public float progress;
    public Vector3 position;
    public Quaternion rotation;
    public CookieType type;
}


