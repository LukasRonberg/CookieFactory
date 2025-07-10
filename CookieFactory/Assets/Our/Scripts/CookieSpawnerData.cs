using UnityEngine;

public class CookieSpawnerData : MonoBehaviour
{
    public ConveyorBeltData belt;
    public float spawnInterval = 1.0f;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            //SpawnCookie();
            SpawnCookiesInAllLanes(); // Spawn cookies in all lanes
            timer = 0f;
        }
    }

    void SpawnCookie()
    {
        belt.cookies.Add(new CookieData
        {
            position = belt.startPoint.position,
            type = Random.value > 0.8f ? CookieType.Bad : CookieType.Normal // 20% bad cookies
        });
    }

    void SpawnCookiesInAllLanes()
    {
        for (int i = 0; i < belt.numberOfLanes; i++)
        {
            belt.cookies.Add(new CookieData
            {
                lane = i,
                progress = 0f,
                type = CookieType.Normal
            });
        }
    }

}
