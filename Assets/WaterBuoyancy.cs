using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WaterBuoyancy : MonoBehaviour
{
    [Header("Water Reference")]
    public Transform waterPlane; // Assign your water plane here

    [Header("Buoyancy Settings")]
    public float buoyancyForce = 9.8f; // Upward force when submerged
    public float waterDrag = 1f; // Resistance when in water
    public float waterAngularDrag = 1f; // Rotational resistance in water
    public float bounceDamping = 0.5f; // How much to dampen the bounce
    public float floatHeight = 0.5f; // How high the object should float above water

    [Header("Wave Settings")]
    public bool useWaves = true;
    public float waveFrequency = 1f;
    public float waveAmplitude = 0.1f;

    private Rigidbody rb;
    private float originalDrag;
    private float originalAngularDrag;
    private bool inWater;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalDrag = rb.linearDamping;
        originalAngularDrag = rb.angularDamping;
    }

    void FixedUpdate()
    {
        if (waterPlane == null)
        {
            Debug.LogWarning("Water plane not assigned to buoyancy script!");
            return;
        }

        float waterHeight = GetWaterHeightAtPosition(transform.position);
        float objectBottom = transform.position.y - GetObjectSubmersionHeight();

        // Check if object is touching water
        inWater = objectBottom < waterHeight;

        if (inWater)
        {
            // Calculate submersion percentage (0 = just touching, 1 = fully submerged)
            float submersion = Mathf.Clamp01((waterHeight - objectBottom) / (2f * GetObjectSubmersionHeight()));

            // Apply buoyancy force (stronger the more submerged)
            float buoyancy = buoyancyForce * submersion;
            rb.AddForce(Vector3.up * buoyancy, ForceMode.Acceleration);

            // Apply damping when coming out of water to create bounce effect
            if (rb.linearVelocity.y > 0 && transform.position.y > waterHeight + floatHeight)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * (1f - bounceDamping), rb.linearVelocity.z);
            }

            // Increase drag in water
            rb.linearDamping = waterDrag;
            rb.angularDamping = waterAngularDrag;
        }
        else
        {
            // Reset drag when out of water
            rb.linearDamping = originalDrag;
            rb.angularDamping = originalAngularDrag;
        }
    }

    private float GetWaterHeightAtPosition(Vector3 position)
    {
        float baseHeight = waterPlane.position.y;

        if (useWaves)
        {
            // Add simple wave effect using perlin noise
            float waveX = position.x * waveFrequency + Time.time;
            float waveZ = position.z * waveFrequency + Time.time;
            float waveOffset = (Mathf.PerlinNoise(waveX, waveZ) * 2f - 1f) * waveAmplitude;
            return baseHeight + waveOffset;
        }

        return baseHeight;
    }

    private float GetObjectSubmersionHeight()
    {
        // Estimate object height for buoyancy (using bounds)
        return GetComponent<Collider>().bounds.extents.y;
    }

    private void OnDrawGizmos()
    {
        if (waterPlane != null && Application.isPlaying)
        {
            Gizmos.color = inWater ? Color.blue : Color.red;
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, GetWaterHeightAtPosition(transform.position), transform.position.z), 0.5f);
        }
    }
}