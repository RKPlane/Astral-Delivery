using UnityEngine;
using UnityEngine.Events;

public class DeliveryZone : MonoBehaviour
{
    [Header("Referencia")]
    public DockingSystem dockingSystem;

    [Header("Feedback")]
    public ParticleSystem deliveryEffect;
    public UnityEvent onDelivery;


    void Awake()
    {

        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (dockingSystem == null) return;
        if (!other.CompareTag("Player")) return;
        if (dockingSystem.DockedCount == 0) return;

        dockingSystem.DeliverAll();
        if (deliveryEffect) deliveryEffect.Play();
        onDelivery?.Invoke();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0.5f, 0.25f);
        Gizmos.DrawSphere(transform.position, GetComponent<SphereCollider>()?.radius ?? 3f);
        Gizmos.color = new Color(0f, 1f, 0.5f, 0.8f);
        Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>()?.radius ?? 3f);
    }
}
