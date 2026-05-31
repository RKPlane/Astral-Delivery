using UnityEngine;

public class StationDebris : MonoBehaviour
{
    [Header("SpringJoint — fuerza elástica")]
    [Tooltip("Rigidez del resorte (N/m). F = -k * x")]
    public float spring    = 40f;
    [Tooltip("Amortiguación del resorte")]
    public float damper    = 5f;
    [Tooltip("Distancia de reposo del resorte")]
    public float distance  = 0f;

    [Header("Conexión")]
    [Tooltip("Déjalo vacío para anclarlo al mundo")]
    public Rigidbody connectedBody;

    [Header("Oscilación inicial")]
    public float initialImpulse = 2f;

    private SpringJoint joint;

    void Start()
    {
        Rigidbody rb  = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping      = 0f;

        joint                    = gameObject.AddComponent<SpringJoint>();
        joint.spring             = spring;
        joint.damper             = damper;
        joint.minDistance        = 0f;
        joint.maxDistance        = distance;
        joint.autoConfigureConnectedAnchor = true;

        if (connectedBody != null)
            joint.connectedBody = connectedBody;

        Vector3 impulseDir = Random.onUnitSphere;
        rb.AddForce(impulseDir * initialImpulse, ForceMode.Impulse);
    }

    void OnDrawGizmosSelected()
    {
        if (joint == null) return;
        Gizmos.color = Color.yellow;
        Vector3 anchor = connectedBody != null
            ? connectedBody.position
            : transform.TransformPoint(joint.connectedAnchor);
        Gizmos.DrawLine(transform.position, anchor);
        Gizmos.DrawWireSphere(anchor, 0.2f);
    }
}
