using UnityEngine;
using UnityEngine.Events;

public class DeliveryZone : MonoBehaviour
{
    [Header("Feedback")]
    public ParticleSystem deliveryEffect;
    public UnityEvent onDelivery;

    void Awake()
    {
        Collider col = GetComponent<Collider>();

        if (col != null)
            col.isTrigger = true;
    }

    void OnTriggerStay(Collider other)
    {
        JunkObject junk = other.GetComponent<JunkObject>();

        if (junk == null) return;

        // Sigue pegado al player
        if (junk.IsDocked) return;

        // Ya entregado
        if (junk.IsCollected) return;

        junk.SetCollected(true);

        if (deliveryEffect)
            deliveryEffect.Play();

        onDelivery?.Invoke();

        Destroy(junk.gameObject);
    }

    void OnDrawGizmos()
    {
        SphereCollider sphere = GetComponent<SphereCollider>();

        float radius = sphere ? sphere.radius : 3f;

        Gizmos.color = new Color(0f, 1f, 0.5f, 0.25f);
        Gizmos.DrawSphere(transform.position, radius);

        Gizmos.color = new Color(0f, 1f, 0.5f, 0.8f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
