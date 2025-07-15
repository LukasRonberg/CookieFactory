using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private float xRotation = 0f;

    void Start()
    {
        // Lås og skjul cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Cursor locked and hidden. ???");
    }

    void Update()
    {
        Look();
        Move();
    }

    void Look()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;

        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotér kamera op/ned (pitch)
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // Rotér “player” (her bare kamera) venstre/højre (yaw)
        transform.parent.Rotate(Vector3.up * mouseDelta.x);
    }

    void Move()
    {
        Vector3 direction = Vector3.zero;
        if (Keyboard.current.wKey.isPressed) direction += transform.parent.forward;
        if (Keyboard.current.sKey.isPressed) direction -= transform.parent.forward;
        if (Keyboard.current.aKey.isPressed) direction -= transform.parent.right;
        if (Keyboard.current.dKey.isPressed) direction += transform.parent.right;

        direction = direction.normalized;
        transform.parent.position += direction * moveSpeed * Time.deltaTime;

        //if (direction != Vector3.zero)
            //Debug.Log($"Walking: {direction}");
    }
}
