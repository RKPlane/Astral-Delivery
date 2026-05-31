using UnityEngine;
using UnityEngine.InputSystem;

public class FreeLookCamera : MonoBehaviour
{
    public Transform target;

    [Header("Orbit")]
    public float distance = 10f;
    public float sensitivity = 2f;
    public float minPitch = -80f;
    public float maxPitch = 80f;

    private float yaw;
    private float pitch;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        yaw += mouseDelta.x * sensitivity * Time.deltaTime;
        pitch -= mouseDelta.y * sensitivity * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        transform.position =
            target.position - rotation * Vector3.forward * distance;

        transform.rotation = rotation;
    }
}
