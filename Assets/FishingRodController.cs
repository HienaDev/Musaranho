using UnityEngine;

public class FishingRodController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The camera the rod is already attached to (optional)")]
    public Camera mainCamera;

    [Header("Sway Settings")]
    [Range(0f, 10f)]
    [Tooltip("How fast the rod sways")]
    public float swaySpeed = 1.5f;

    [Range(0f, 30f)]
    [Tooltip("How much the rod sways")]
    public float swayAmount = 5f;

    [Range(0f, 1f)]
    [Tooltip("Damping factor for sway movement")]
    public float swayDamping = 0.2f;

    [Range(0f, 2f)]
    [Tooltip("How much camera movement affects sway")]
    public float cameraMovementInfluence = 0.8f;

    // Internal variables for controlling sway
    private Vector3 initialLocalRotation;
    private Vector3 targetRotation;
    private Vector3 currentRotation;
    private Vector3 lastCameraRotation;
    private float swayTimer = 0f;

    void Start()
    {
        // If no camera is assigned, use the parent camera or main camera
        if (mainCamera == null)
        {
            if (transform.parent != null && transform.parent.GetComponent<Camera>() != null)
                mainCamera = transform.parent.GetComponent<Camera>();
            else
                mainCamera = Camera.main;
        }

        // Store initial local rotation
        initialLocalRotation = transform.localEulerAngles;

        // Store current camera rotation for initial reference
        lastCameraRotation = mainCamera.transform.eulerAngles;

        // Initialize current rotation to match initial rotation
        currentRotation = initialLocalRotation;
    }

    void Update()
    {
        // Calculate camera rotation change
        Vector3 cameraRotationDelta = mainCamera.transform.eulerAngles - lastCameraRotation;

        // Normalize angles to avoid issues when crossing 360 degrees
        if (cameraRotationDelta.y > 180) cameraRotationDelta.y -= 360;
        if (cameraRotationDelta.y < -180) cameraRotationDelta.y += 360;
        if (cameraRotationDelta.x > 180) cameraRotationDelta.x -= 360;
        if (cameraRotationDelta.x < -180) cameraRotationDelta.x += 360;

        // Calculate automatic sway based on time
        swayTimer += Time.deltaTime * swaySpeed;
        float swayX = Mathf.Sin(swayTimer) * swayAmount;
        float swayY = Mathf.Cos(swayTimer * 0.6f) * swayAmount * 0.5f;

        // Add camera movement influence
        swayX += cameraRotationDelta.y * cameraMovementInfluence;
        swayY -= cameraRotationDelta.x * cameraMovementInfluence;

        // Set target rotation
        targetRotation = new Vector3(
            initialLocalRotation.x + swayY,
            initialLocalRotation.y + swayX,
            initialLocalRotation.z
        );

        // Apply smoothed rotation with damping
        currentRotation = Vector3.Lerp(currentRotation, targetRotation, 1 - swayDamping);
        transform.localEulerAngles = currentRotation;

        // Store current camera rotation for next frame
        lastCameraRotation = mainCamera.transform.eulerAngles;
    }
}