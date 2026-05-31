using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs de chatarra")]
    public GameObject[] junkPrefabs;

    [Header("SPAWN")]
    public int totalJunk = 35;
    public float minRadius = 10f;
    public float maxRadius = 45f;
    public Vector3 stationCenter = Vector3.zero;

    void Start()
    {
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
