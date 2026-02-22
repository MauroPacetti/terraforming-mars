using UnityEngine;

/// <summary>
/// Simple first-person controller for exploring the Mars scene.
/// WASD + Mouse look. Attach to Player object with CharacterController.
/// </summary>
public class FPSController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float gravity = 3.72f; // Mars gravity (m/s^2)

    [Header("Look")]
    public float mouseSensitivity = 2f;
    public float maxPitch = 85f;

    private CharacterController cc;
    private Transform camTransform;
    private float pitch = 0f;
    private float yaw = 0f;
    private float verticalVelocity = 0f;
    private bool cursorLocked = false;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        camTransform = Camera.main?.transform;

        if (camTransform == null)
        {
            Debug.LogError("[FPSController] No main camera found!");
            return;
        }

        // Lock cursor
        LockCursor();
    }

    void Update()
    {
        HandleCursorLock();
        HandleLook();
        HandleMovement();
    }

    void HandleCursorLock()
    {
        if (Input.GetMouseButtonDown(0) && !cursorLocked)
        {
            LockCursor();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cursorLocked = true;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cursorLocked = false;
    }

    void HandleLook()
    {
        if (!cursorLocked) return;

        float mx = Input.GetAxis("Mouse X") * mouseSensitivity;
        float my = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mx;
        pitch -= my;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        transform.rotation = Quaternion.Euler(0, yaw, 0);
        if (camTransform != null)
        {
            camTransform.localRotation = Quaternion.Euler(pitch, 0, 0);
        }
    }

    void HandleMovement()
    {
        if (cc == null) return;

        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float moveX = Input.GetAxis("Horizontal"); // A/D
        float moveZ = Input.GetAxis("Vertical");   // W/S

        Vector3 move = (forward * moveZ + right * moveX).normalized * speed;

        // Gravity
        if (cc.isGrounded)
        {
            verticalVelocity = -0.5f;
            // Jump on Mars (lower gravity!)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                verticalVelocity = Mathf.Sqrt(2f * gravity * 1.2f); // ~1.2m jump
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        move.y = verticalVelocity;
        cc.Move(move * Time.deltaTime);
    }
}
