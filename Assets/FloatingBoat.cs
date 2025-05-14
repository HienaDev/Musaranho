using UnityEngine;

public class FloatingBoat : MonoBehaviour
{
    [Header("Setup")]
    public Transform player;

    [Header("Boat Tilt Settings")]
    public float tiltAmount = 5f;        // Max degrees of tilt
    public float tiltSpeedMultiplier = 1f; // How tilt scales with player movement

    [Header("Spring Settings")]
    public float springStrength = 10f;   // How strong the spring pulls back
    public float springDamping = 1.5f;   // Damping to slow down oscillation (higher = less wobbly)


    public enum TiltMode { XZ, XY, YZ }
    [Header("Rotation Axes")]
    public TiltMode tiltMode = TiltMode.XY; // For a boat rotated -90 on X, use XY

    private Vector2 currentTilt;         // Stores current tilt values
    private Vector2 tiltVelocity;        // Current velocity of the tilt
    private Quaternion originalRotation; // Store original rotation as Quaternion

    void Start()
    {
        // Store the original rotation as a Quaternion
        originalRotation = transform.rotation;
    }

    void Update()
    {
        if (player == null) return;

        // Local player position relative to the boat
        Vector3 localPlayerPos = transform.InverseTransformPoint(player.position);

        // Target tilt based on player position
        // For a boat rotated -90 on X, we need different axes
        float targetTiltX = 0f;
        float targetTiltY = 0f;
        float targetTiltZ = 0f;

        switch (tiltMode)
        {
            case TiltMode.XZ: // Original mode (standard upright boat)
                targetTiltX = Mathf.Clamp(-localPlayerPos.z * tiltSpeedMultiplier, -1f, 1f) * tiltAmount;
                targetTiltZ = Mathf.Clamp(localPlayerPos.x * tiltSpeedMultiplier, -1f, 1f) * tiltAmount;
                break;

            case TiltMode.XY: // For boat rotated -90 on X (your case)
                targetTiltX = Mathf.Clamp(-localPlayerPos.y * tiltSpeedMultiplier, -1f, 1f) * tiltAmount;
                targetTiltY = Mathf.Clamp(localPlayerPos.x * tiltSpeedMultiplier, -1f, 1f) * tiltAmount;
                break;

            case TiltMode.YZ: // Another possible configuration
                targetTiltY = Mathf.Clamp(-localPlayerPos.z * tiltSpeedMultiplier, -1f, 1f) * tiltAmount;
                targetTiltZ = Mathf.Clamp(localPlayerPos.x * tiltSpeedMultiplier, -1f, 1f) * tiltAmount;
                break;
        }

        Vector2 targetTilt = new Vector2(targetTiltX, (tiltMode == TiltMode.XZ) ? targetTiltZ : targetTiltY);

        // Simulate spring physics
        Vector2 displacement = currentTilt - targetTilt;
        Vector2 springForce = (-springStrength * displacement) - (springDamping * tiltVelocity);
        tiltVelocity += springForce * Time.deltaTime;
        currentTilt += tiltVelocity * Time.deltaTime;

        // Apply the tilt based on selected tilt mode
        Quaternion tiltRotation;

        switch (tiltMode)
        {
            case TiltMode.XZ:
                tiltRotation = Quaternion.Euler(currentTilt.x, 0f, currentTilt.y);
                break;

            case TiltMode.XY:
                tiltRotation = Quaternion.Euler(currentTilt.x, currentTilt.y, 0f);
                break;

            case TiltMode.YZ:
                tiltRotation = Quaternion.Euler(0f, currentTilt.x, currentTilt.y);
                break;

            default:
                tiltRotation = Quaternion.identity;
                break;
        }

        // Apply the tilt relative to the original rotation
        transform.rotation = originalRotation * tiltRotation;
    }
}