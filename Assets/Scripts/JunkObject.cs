using System;
using UnityEngine;

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
        ParticleSystem fx = Instantiate(collectEffect, transform.position, Quaternion.identity);
        fx.Play();
        Destroy(fx.gameObject, fx.main.duration + 0.5f);
    }

    void OnDrawGizmos()
    {
        if (IsDocked || IsCollected || junkData == null) return;
        Gizmos.color = junkData.glowColor * 0.4f;
        Gizmos.DrawWireSphere(transform.position, junkData.mass * 0.3f);
    }
}
