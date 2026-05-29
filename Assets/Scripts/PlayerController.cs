using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Thruster")]
    public float thrustForce = 6f;
    public float rotationSpeed = 90f;

    [Header("Particulas")]
    public ParticleSystem thrusterParticles;

    [Header("Camara")]
    public Transform cameraTarget;
    public float cameraSmoothing = 5f;
    public Vector3 cameraOffset = new Vector3(0f, 4f, -10f);

    [Header("Input")]
    public InputActionAsset inputActions;

    private InputAction moveAction;
    private InputAction verticalMoveAction;

    private Vector2 moveInput;
    private float verticalInput;

    private Rigidbody rb;

    public Vector3 Velocity => rb.linearVelocity;
    public float Mass => rb.mass;
    public float Momentum => rb.mass * rb.linearVelocity.magnitude;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.linearDamping = 0.02f;
        rb.angularDamping = 0.5f;

        // Buscar acciones
        var playerMap = inputActions.FindActionMap("Player");

        moveAction = playerMap.FindAction("Move");
        verticalMoveAction = playerMap.FindAction("VerticalMove");
    }

    void OnEnable()
    {
        moveAction.Enable();
        verticalMoveAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        verticalMoveAction.Disable();
    }

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        verticalInput = verticalMoveAction.ReadValue<float>();
    }

    void FixedUpdate()
    {
        HandleThrust();
        HandleRotation();
        ClampMaxSpeed();
    }

    void LateUpdate()
    {
        FollowCamera();
    }

    void HandleThrust()
    {
        Vector3 inputDir = new Vector3(
            moveInput.x,
            verticalInput,
            moveInput.y
        ).normalized;

        bool thrusting = inputDir.sqrMagnitude > 0.01f;

        if (thrusting)
        {
            rb.AddForce(
                transform.TransformDirection(inputDir) * thrustForce,
                ForceMode.Force
            );
        }

        if (thrusterParticles != null)
        {
            if (thrusting && !thrusterParticles.isEmitting)
                thrusterParticles.Play();

            if (!thrusting && thrusterParticles.isEmitting)
                thrusterParticles.Stop();
        }
    }

    void HandleRotation()
    {
        float yaw =
            moveInput.x *
            rotationSpeed *
            Time.fixedDeltaTime;

        float pitch =
            moveInput.y *
            rotationSpeed *
            Time.fixedDeltaTime *
            0.5f;

        transform.Rotate(Vector3.up, yaw, Space.World);
        transform.Rotate(Vector3.right, -pitch, Space.Self);
    }

    void ClampMaxSpeed()
    {
        float maxSpeed = 10f;

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity =
                rb.linearVelocity.normalized * maxSpeed;
        }
    }

    void FollowCamera()
    {
        if (cameraTarget == null)
            return;

        Vector3 desired =
            transform.position +
            transform.TransformDirection(cameraOffset);

        cameraTarget.position = Vector3.Lerp(
            cameraTarget.position,
            desired,
            cameraSmoothing * Time.deltaTime
        );

        cameraTarget.LookAt(
            transform.position + transform.forward * 3f
        );
    }

    public void ApplyReactionImpulse(
        float junkMass,
        Vector3 launchDirection,
        float launchSpeed)
    {
        Vector3 reaction =
            -launchDirection * (junkMass * launchSpeed);

        rb.AddForce(reaction, ForceMode.Impulse);
    }

    public void SetMass(float newMass)
    {
        rb.mass = Mathf.Max(0.5f, newMass);
    }
}