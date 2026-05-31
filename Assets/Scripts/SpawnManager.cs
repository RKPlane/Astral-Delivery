using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs de chatarra")]
    public GameObject[] junkPrefabs;

    [Header("SPAWN")]
    public int totalJunk = 12;
    public float minRadius = 10f;
    public float maxRadius = 30f;
    public Vector3 stationCenter = Vector3.zero;

    public int randomSeed = 0;

    void Start()
    {
        if (randomSeed != 0) UnityEngine.Random.InitState(randomSeed);
        SpawnAll();
    }

    void SpawnAll()
    {
        if (junkPrefabs == null || junkPrefabs.Length == 0) return;

        for (int i = 0; i < totalJunk; i++)
        {
            Vector3 dir = UnityEngine.Random.onUnitSphere;
            float radius = UnityEngine.Random.Range(minRadius, maxRadius);
            Vector3 pos = stationCenter + dir * radius;

            GameObject prefab = junkPrefabs[UnityEngine.Random.Range(0, junkPrefabs.Length)];
            Instantiate(prefab, pos, UnityEngine.Random.rotation);
        }
    }
}
