using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Thruster")]
    public float thrustForce = 6f;
    public float rotationSpeed = 90f;
    public float maxSpeed = 6f;

    [Header("Freno (retro-thruster)")]
    public float brakeForce = 10f;
    public float brakeDeadzone = 0.08f;
    public ParticleSystem brakeParticles;

    [Header("Particulas")]
    public ParticleSystem thrusterParticles;

    [Header("Camara")]
    public Transform cameraTarget;
    public float cameraSmoothing = 5f;
    public Vector3 cameraOffset = new Vector3(0f, 4f, -10f);

    [Header("Input")]
    public InputActionAsset inputActions;

    [Header("Visual")]
    public Transform shipModel;
    private float visualYaw;
    private float visualPitch;

    private InputAction moveAction;
    private InputAction verticalMoveAction;
    private InputAction brakeAction;
    private InputAction boostAction;

    private Vector2 moveInput;
    private float verticalInput;
    private bool thrusting;
    private bool braking;
    private bool boostPressed;

    private Rigidbody rb;

    public Vector3 Velocity => rb.linearVelocity;
    public float Mass => rb.mass;
    public float Momentum => rb.mass * rb.linearVelocity.magnitude;

    void Awake()
    {
        //Valida rb
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 0.02f;
        rb.angularDamping = 0.5f;

        var playerMap = inputActions.FindActionMap("Player");
        moveAction = playerMap.FindAction("Move");
        verticalMoveAction = playerMap.FindAction("VerticalMove");
        brakeAction = playerMap.FindAction("Brake");
        boostAction = playerMap.FindAction("Thrusters");
    }

    void OnEnable()
    {
        moveAction.Enable();
        verticalMoveAction.Enable();
        brakeAction.Enable();
        boostAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        verticalMoveAction.Disable();
        brakeAction.Disable();
        boostAction.Disable();
    }

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        verticalInput = verticalMoveAction.ReadValue<float>();
        braking = brakeAction.IsPressed();
        boostPressed = boostAction.IsPressed();
    }

    void FixedUpdate()
    {
        ApplyThrust();
        ApplyBrake();
        ApplyRotation();
        ClampSpeed();
        ApplyBoost();
    }

    void ApplyThrust()
    {
        Vector3 dir = new Vector3(moveInput.x, verticalInput, moveInput.y).normalized;
        thrusting = dir.sqrMagnitude > 0.01f;

        if (thrusting)
            rb.AddForce(transform.TransformDirection(dir) * thrustForce, ForceMode.Force);

        if (thrusterParticles == null) return;
        if (thrusting && !thrusterParticles.isEmitting) thrusterParticles.Play();
        if (!thrusting && thrusterParticles.isEmitting) thrusterParticles.Stop();
    }

    void ApplyBoost()
    {
        if (!boostPressed) return;

        Vector3 forward = transform.forward;

        rb.AddForce(forward * thrustForce, ForceMode.Impulse);
    }

    void ApplyBrake()
    {
        float speed = rb.linearVelocity.magnitude;

        if (braking && speed > brakeDeadzone)
        {
            Vector3 brakeDir = -rb.linearVelocity.normalized;
            float force = Mathf.Min(brakeForce, speed * rb.mass / Time.fixedDeltaTime);
            rb.AddForce(brakeDir * force, ForceMode.Force);
        }

        if (brakeParticles == null) return;
        bool shouldBrake = braking && speed > brakeDeadzone;
        if (shouldBrake && !brakeParticles.isEmitting) brakeParticles.Play();
        if (!shouldBrake && brakeParticles.isEmitting) brakeParticles.Stop();
    }

    /*Rotacion fisica sim
     * void ApplyRotation()
    {
        if (!thrusting) return;

        float yaw = moveInput.x * rotationSpeed * Time.fixedDeltaTime;
        float pitch = moveInput.y * rotationSpeed * Time.fixedDeltaTime * 0.5f;

        //FIX
        Quaternion yawRot = Quaternion.AngleAxis(yaw, Vector3.up);
        Quaternion pitchRot = Quaternion.AngleAxis(-pitch, transform.right);
        rb.MoveRotation(yawRot * pitchRot * rb.rotation);
    }*/

    void ApplyRotation()
    {
        if (shipModel == null) return;

        visualYaw += moveInput.x * rotationSpeed * Time.fixedDeltaTime;
        visualPitch -= moveInput.y * rotationSpeed * 0.5f * Time.fixedDeltaTime;

        visualPitch = Mathf.Clamp(visualPitch, -45f, 45f);

        shipModel.localRotation =
            Quaternion.Euler(visualPitch, visualYaw, 0f);
    }

    void ClampSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    /*void LateUpdate()
    {
        FollowCamera();
    }

    void FollowCamera()
    {
        if (cameraTarget == null) return;

        Vector3 desired = transform.position + transform.TransformDirection(cameraOffset);
        cameraTarget.position = Vector3.Lerp(
            cameraTarget.position, desired, cameraSmoothing * Time.deltaTime);
        cameraTarget.LookAt(transform.position + transform.forward * 3f);
    }*/

    public void ApplyReactionImpulse(float junkMass, Vector3 dir, float speed)
    {
        rb.AddForce(-dir * (junkMass * speed), ForceMode.Impulse);
    }

    public void SetMass(float newMass)
    {
        rb.mass = Mathf.Max(0.5f, newMass);
    }
}