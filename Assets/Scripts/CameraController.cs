using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 startRotation;
    private float moveSpeed = 10f; // Movement speed
    private float rotationSpeed = 100f; // Rotation speed
    private float zoomSpeed = 10f; // Zoom speed
    private float minZoom = 0f; // Minimum zoom (base Y position)
    private float maxZoom = 50f; // Maximum zoom relative to base Y position
    private float minRotationX = 0f; // Minimum X rotation
    private float maxRotationX = 90f; // Maximum X rotation

    private float rotationX = 0f; // Current X rotation
    private float rotationY = 0f; // Current Y rotation

    private void Start()
    {
        // Set initial camera Y position to the minimum zoom level
        transform.position = new Vector3(startPos.x, maxZoom, startPos.z);
        transform.rotation = Quaternion.Euler (startRotation.x, startRotation.y, startRotation.z);
    }

    private void Update()
    {
        HandleZoom();
        HandleRotation();
        HandleMovement();
    }

    private void HandleZoom()
    {
        float zoom = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        float newZoom = Mathf.Clamp(transform.position.y - zoom, minZoom, minZoom + maxZoom);
        transform.position = new Vector3(transform.position.x, newZoom, transform.position.z);
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButton(1)) // Right-click held
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            rotationY += mouseX * rotationSpeed * Time.unscaledDeltaTime; // Rotate around Y-axis (Z-axis rotation)
            rotationX -= mouseY * rotationSpeed * Time.unscaledDeltaTime; // Rotate around X-axis

            // Clamp the X rotation to prevent looking past the horizon
            rotationX = Mathf.Clamp(rotationX, minRotationX, maxRotationX);

            transform.eulerAngles = new Vector3(rotationX, rotationY, 0f); // Apply rotation
        }
    }

    private void HandleMovement()
    {
        // Use unscaled delta time so that the camera movement isnt affected by the changed time scale
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.unscaledDeltaTime; // Left/Right movement
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.unscaledDeltaTime;   // Forward/Backward movement

        // Calculate the forward movement vector without affecting the Y position
        Vector3 forwardMovement = transform.forward * moveZ;
        forwardMovement.y = 0; // Ensure no change in Y

        // Calculate the right movement vector
        Vector3 rightMovement = transform.right * moveX;

        // Combine the movements and apply them
        Vector3 movement = forwardMovement + rightMovement;
        transform.position += movement;
    }
}
