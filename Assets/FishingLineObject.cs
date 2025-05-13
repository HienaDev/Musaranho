using UnityEngine;

public class FishingLineObject : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The transform to which this object is attached (fishing rod tip)")]
    public Transform attachPoint;

    [Header("Line Renderer Settings")]
    [Tooltip("Should a fishing line be rendered automatically?")]
    public bool renderLine = true;

    [Tooltip("Width of the fishing line")]
    public float lineWidth = 0.01f;

    [Tooltip("Material for the fishing line")]
    public Material lineMaterial;

    [Tooltip("Color of the fishing line")]
    public Color lineColor = Color.white;

    [Header("Physics Settings")]
    [Range(0.1f, 10f)]
    [Tooltip("Length of the fishing line")]
    public float lineLength = 2.0f;

    [Range(0f, 20f)]
    [Tooltip("How much the object is affected by gravity")]
    public float gravity = 9.8f;

    [Range(0f, 1f)]
    [Tooltip("Damping factor (air resistance)")]
    public float damping = 0.1f;

    [Range(0f, 10f)]
    [Tooltip("How much the object sways with rod movement")]
    public float movementInfluence = 2.0f;

    [Range(0f, 1f)]
    [Tooltip("Additional random movement")]
    public float randomMovement = 0.2f;

    // Internal physics variables
    private Vector3 velocity;
    private Vector3 acceleration;
    private Vector3 lastAttachPointPosition;
    private LineRenderer lineRenderer;

    void Start()
    {
        // If no attach point is assigned, try to find a parent to use
        if (attachPoint == null && transform.parent != null)
        {
            attachPoint = transform.parent;
        }

        // Initialize last position for velocity calculation
        lastAttachPointPosition = attachPoint != null ? attachPoint.position : transform.position;

        // Initialize position below the attach point
        if (attachPoint != null)
        {
            Vector3 startPos = attachPoint.position;
            startPos.y -= lineLength;
            transform.position = startPos;
        }

        // Setup line renderer if enabled
        if (renderLine)
        {
            SetupLineRenderer();
        }
    }

    void SetupLineRenderer()
    {
        // Add line renderer component if it doesn't exist
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Configure line renderer
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 2;

        // Set material
        if (lineMaterial != null)
        {
            lineRenderer.material = lineMaterial;
        }
        else
        {
            // Use default material if none provided
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        // Set color
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
    }

    void Update()
    {
        if (attachPoint == null)
            return;

        // Calculate rod movement velocity
        Vector3 attachPointVelocity = (attachPoint.position - lastAttachPointPosition) / Time.deltaTime;
        lastAttachPointPosition = attachPoint.position;

        // Set up forces
        Vector3 gravityForce = Vector3.down * gravity;

        // Calculate the direction from attach point to object
        Vector3 lineDirection = (transform.position - attachPoint.position).normalized;

        // Calculate distance from attach point to object
        float distance = Vector3.Distance(transform.position, attachPoint.position);

        // Calculate spring force (if stretched beyond line length)
        Vector3 springForce = Vector3.zero;
        if (distance > lineLength)
        {
            // Apply spring force based on how far beyond the line length we are
            springForce = -lineDirection * ((distance - lineLength) * 10f);
        }

        // Calculate damping force (air resistance)
        Vector3 dampingForce = -velocity * damping;

        // Apply rod movement influence
        Vector3 movementForce = attachPointVelocity * movementInfluence;

        // Add random movement
        Vector3 randomForce = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ) * randomMovement;

        // Sum all forces to get acceleration
        acceleration = gravityForce + springForce + dampingForce + movementForce + randomForce;

        // Update velocity and position with simple physics integration
        velocity += acceleration * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        // Enforce maximum line length constraint
        if (distance > lineLength)
        {
            // Correct position to be exactly at line length
            transform.position = attachPoint.position + lineDirection * lineLength;

            // Project velocity onto the tangent plane of the sphere defined by the line length
            Vector3 velocityRadial = Vector3.Project(velocity, lineDirection);
            Vector3 velocityTangential = velocity - velocityRadial;
            velocity = velocityTangential;
        }

        // Update line renderer positions
        if (renderLine && lineRenderer != null)
        {
            lineRenderer.SetPosition(0, attachPoint.position);
            lineRenderer.SetPosition(1, transform.position);
        }
    }

    // Optional: Visualize the fishing line in the editor
    void OnDrawGizmos()
    {
        if (attachPoint != null && !Application.isPlaying)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, attachPoint.position);
        }
    }
}