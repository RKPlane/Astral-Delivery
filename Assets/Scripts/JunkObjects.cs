using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewJunk", menuName = "SpaceJunk/JunkData")]
public class JunkData : ScriptableObject
{
    [Header("Identidad")]
    public string label = "PANEL SOLAR";
    public GameObject prefab;
    public Color glowColor = Color.cyan;

    [Header("Física")]
    public float mass = 2f;
    public float driftSpeed = 0.15f;
    public float spinSpeed = 8f;

    [Header("Gameplay")]
    public int points = 5;
    public JunkRarity rarity = JunkRarity.Common;

    [TextArea(2, 4)]
    public string flavourText = "Módulo flotante de la estación destruida.";
}

public enum JunkRarity { Common, Rare, Critical }

[RequireComponent(typeof(Rigidbody))]
public class JunkObject : MonoBehaviour
{
    [Header("Datos")]
    public JunkData junkData;

    [Header("Efectos")]
    public ParticleSystem collectEffect;
    public Light glowLight;

    public static event Action<JunkObject> OnCollected;
    public static event Action<JunkObject> OnDocked;
    public static event Action<JunkObject> OnUndocked;

    public JunkData JunkData => junkData;
    public bool IsDocked { get; private set; }
    public bool IsCollected { get; private set; }

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;

        if (junkData != null)
        {
            rb.mass = junkData.mass;

            Vector3 randomDir = UnityEngine.Random.onUnitSphere;
            rb.linearVelocity = randomDir * UnityEngine.Random.Range(0f, junkData.driftSpeed);
            rb.angularVelocity = UnityEngine.Random.insideUnitSphere * junkData.spinSpeed;
        }
    }

    void Update()
    {
        if (IsDocked || IsCollected) return;
        if (glowLight != null)
            glowLight.intensity = 0.4f + Mathf.Sin(Time.time * 2.5f) * 0.2f;
    }

    public void SetDocked(bool docked)
    {
        IsDocked = docked;
        rb.isKinematic = docked;
        if (docked) OnDocked?.Invoke(this);
        else OnUndocked?.Invoke(this);
    }

    public void SetCollected(bool collected)
    {
        if (IsCollected) return;
        IsCollected = collected;
        if (!collected) return;

        OnCollected?.Invoke(this);
        PlayCollectEffect();
        gameObject.SetActive(false);
    }

    void PlayCollectEffect()
    {
        if (collectEffect == null) return;
        collectEffect.transform.SetParent(null);
        collectEffect.Play();
        Destroy(collectEffect.gameObject, collectEffect.main.duration + 0.5f);
    }

    void OnDrawGizmos()
    {
        if (IsDocked || IsCollected || junkData == null) return;
        Gizmos.color = junkData.glowColor * 0.4f;
        Gizmos.DrawWireSphere(transform.position, junkData.mass * 0.3f);
    }
}

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs de chatarra")]
    public GameObject[] junkPrefabs;

    [Header("Distribución")]
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
