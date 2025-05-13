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

    private Vector2 currentTilt;         // (X = forward/back tilt, Y = left/right tilt)
    private Vector2 tiltVelocity;        // Current velocity of the tilt

    private Vector3 originalRotation;

    void Start()
    {
        originalRotation = transform.eulerAngles;
    }

    void Update()
    {
        if (player == null) return;

        // Local player position relative to the boat
        Vector3 localPlayerPos = transform.InverseTransformPoint(player.position);

        // Target tilt based on player position
        float targetTiltX = Mathf.Clamp(-localPlayerPos.z * tiltSpeedMultiplier, -1f, 1f) * tiltAmount;
        float targetTiltY = Mathf.Clamp(localPlayerPos.x * tiltSpeedMultiplier, -1f, 1f) * tiltAmount;

        Vector2 targetTilt = new Vector2(targetTiltX, targetTiltY);

        // Simulate spring physics
        Vector2 displacement = currentTilt - targetTilt;
        Vector2 springForce = (-springStrength * displacement) - (springDamping * tiltVelocity);

        tiltVelocity += springForce * Time.deltaTime;
        currentTilt += tiltVelocity * Time.deltaTime;

        // Apply the tilt to boat rotation
        transform.rotation = Quaternion.Euler(
            originalRotation.x + currentTilt.x,
            originalRotation.y,
            originalRotation.z + currentTilt.y
        );
    }
}
