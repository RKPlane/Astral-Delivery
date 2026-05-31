using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ViscousDrag : MonoBehaviour
{
    [Header("Resistencia viscosa cuadrática")]
    [Tooltip("Coeficiente de arrastre. Más alto = más resistencia del medio.")]
    public float dragCoefficient  = 0.08f;

    [Tooltip("Densidad del fluido/gas (kg/m³). Espacio vacío ≈ 0, nebulosa ≈ 0.01")]
    public float fluidDensity     = 0.01f;

    [Tooltip("Área frontal del objeto (m²)")]
    public float crossSectionArea = 0.5f;

    [Header("Resistencia angular")]
    public float angularDragCoeff = 0.04f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping  = 0f;
        rb.angularDamping = 0f;
    }

    void FixedUpdate()
    {
        ApplyLinearDrag();
        ApplyAngularDrag();
    }

    void ApplyLinearDrag()
    {
        Vector3 velocity = rb.linearVelocity;
        float   speed    = velocity.magnitude;
        if (speed < 0.001f) return;

        // F_drag = 0.5 * rho * Cd * A * v²
        float dragMagnitude = 0.5f * fluidDensity * dragCoefficient
                              * crossSectionArea * speed * speed;

        Vector3 dragForce = -velocity.normalized * dragMagnitude;
        rb.AddForce(dragForce, ForceMode.Force);
    }

    void ApplyAngularDrag()
    {
        Vector3 angVel = rb.angularVelocity;
        float   speed  = angVel.magnitude;
        if (speed < 0.001f) return;

        float dragMagnitude = angularDragCoeff * speed * speed;
        rb.AddTorque(-angVel.normalized * dragMagnitude, ForceMode.Force);
    }
}
