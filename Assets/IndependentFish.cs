using UnityEngine;
using System.Collections.Generic;

public class IndependentFish : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform mouth;

    [Header("Movement Settings")]
    [SerializeField] private float roamSpeed = 1f;
    [SerializeField] private float chaseSpeed = 2.5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float roamRadius = 5f;
    [SerializeField] private float maxYDeviation = 0.5f;
    [SerializeField] private float minRoamTime = 2f;
    [SerializeField] private float maxRoamTime = 5f;

    [Header("Lure Detection")]
    [SerializeField] private float detectionRadius = 7f;
    [SerializeField] private float minDistanceToLure = 0.1f;
    [SerializeField] private float interestProbability = 0.7f;
    [SerializeField] private int maxLureAttempts = 3;
    [SerializeField] private float maxTimeOnLure = 10f;

    [Header("Boundary Settings")]
    [SerializeField] private float avoidRadius = 2f;

    [Header("Disappearance Settings")]
    [SerializeField] private float giveUpDuration = 5f;
    [SerializeField] private float lifetimeDuration = 60f;
    [SerializeField] private float diveAngleRange = 30f;
    [SerializeField] private float diveSharpness = 2f;

    // Private state variables
    private Vector3 initialPosition;
    private float initialY;
    private Vector3 currentRoamTarget;
    private float roamTimer;
    private float lifetimeTimer = 0f;
    private int lureAttempts = 0;
    private float timeOnLure = 0f;
    private bool isChasing = false;
    private Transform currentLure = null;
    private bool hasGivenUp = false;
    private Vector3 giveUpDirection;
    private bool isActive = true;
    private bool ripplesActive = false;
    private Transform avoidObject = null;

    // Fish states
    private enum FishState { Roaming, ChasingLure, NibblingLure, GivingUp }
    private FishState currentState = FishState.Roaming;

    void Start()
    {
        if (mouth == null)
            mouth = transform;

        initialPosition = transform.position;
        initialY = transform.position.y;
        SetNewRoamTarget();
    }

    public void SetAvoidObject(Transform objectToAvoid, float radius)
    {
        avoidObject = objectToAvoid;
        avoidRadius = radius;
    }

    void Update()
    {
        if (!isActive) return;

        // Increment lifetime timer
        lifetimeTimer += Time.deltaTime;
        if (lifetimeTimer > lifetimeDuration)
        {
            Disappear();
            return;
        }

        // Handle different states
        switch (currentState)
        {
            case FishState.Roaming:
                Roam();
                DetectLures();
                break;

            case FishState.ChasingLure:
                ChaseLure();
                break;

            case FishState.NibblingLure:
                NibbleLure();
                break;

            case FishState.GivingUp:
                ApplyGiveUp();
                break;
        }
    }

    private void Roam()
    {
        // Update roam timer
        roamTimer -= Time.deltaTime;
        if (roamTimer <= 0)
        {
            SetNewRoamTarget();
        }

        // Move toward current roam target
        Vector3 direction = (currentRoamTarget - transform.position).normalized;

        // Check if we need to avoid an object
        if (avoidObject != null)
        {
            float distToAvoid = Vector3.Distance(transform.position, avoidObject.position);
            if (distToAvoid < avoidRadius)
            {
                Vector3 avoidDir = (transform.position - avoidObject.position).normalized;
                direction = Vector3.Lerp(direction, avoidDir, 1 - (distToAvoid / avoidRadius));
            }
        }

        // Rotate toward the target direction
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Move forward
        transform.position += transform.forward * roamSpeed * Time.deltaTime;

        // Constrain Y position to stay close to initial height
        Vector3 position = transform.position;
        position.y = Mathf.Clamp(position.y, initialY - maxYDeviation, initialY + maxYDeviation);
        transform.position = position;
    }

    private void SetNewRoamTarget()
    {
        // Generate a random point within the roam radius
        Vector2 randomCircle = Random.insideUnitCircle * roamRadius;
        Vector3 targetPosition = initialPosition + new Vector3(randomCircle.x, 0, randomCircle.y);

        // Add a small random Y variation but constrain it
        float randomY = Random.Range(-maxYDeviation, maxYDeviation);
        targetPosition.y = initialY + randomY;

        // Set new target and reset timer
        currentRoamTarget = targetPosition;
        roamTimer = Random.Range(minRoamTime, maxRoamTime);
    }

    private void DetectLures()
    {
        if (lureAttempts >= maxLureAttempts) return;

        // Find all lure objects
        Lure[] lures = FindObjectsOfType<Lure>();
        List<Lure> validLures = new List<Lure>();

        // Filter lures to find those within detection radius
        foreach (Lure lure in lures)
        {
            float distance = Vector3.Distance(transform.position, lure.transform.position);
            if (distance <= detectionRadius)
            {
                validLures.Add(lure);
            }
        }

        // Only proceed if we found lures
        if (validLures.Count > 0)
        {
            // Randomly decide if fish is interested
            if (Random.value <= interestProbability)
            {
                // Pick a random lure from the valid ones
                Lure selectedLure = validLures[Random.Range(0, validLures.Count)];
                currentLure = selectedLure.transform;
                isChasing = true;
                currentState = FishState.ChasingLure;
                lureAttempts++;
            }
        }
    }

    private void ChaseLure()
    {
        if (currentLure == null)
        {
            // Lost the lure, go back to roaming
            currentState = FishState.Roaming;
            return;
        }

        // Calculate distance to lure
        float distance = Vector3.Distance(mouth.position, currentLure.position);

        // Check if we've reached the lure
        if (distance <= minDistanceToLure)
        {
            // Start nibbling
            currentState = FishState.NibblingLure;
            timeOnLure = 0f;
            ripplesActive = true;
            // You could add code here to activate ripple effects
            return;
        }

        // Move toward the lure
        Vector3 direction = (currentLure.position - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        transform.position += transform.forward * chaseSpeed * Time.deltaTime;
    }

    private void NibbleLure()
    {
        if (currentLure == null)
        {
            // Lost the lure, go back to roaming
            currentState = FishState.Roaming;
            return;
        }

        // Update time on lure
        timeOnLure += Time.deltaTime;

        // Check if fish has been on lure long enough
        if (timeOnLure >= maxTimeOnLure)
        {
            // Fish gives up
            GiveUp();
        }

        // Optionally face the lure while nibbling
        Vector3 direction = (currentLure.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * 0.5f * Time.deltaTime);
        }
    }

    private void GiveUp()
    {
        hasGivenUp = true;
        currentState = FishState.GivingUp;
        timeOnLure = 0f;

        // Calculate random downward direction
        Vector3 randomDirection = Vector3.down;

        // Add random horizontal angle
        float randomAngle = Random.Range(-diveAngleRange, diveAngleRange);
        Quaternion angleRotation = Quaternion.AngleAxis(randomAngle, Vector3.forward);
        randomDirection = angleRotation * randomDirection;

        // Make the dive sharper by biasing more downward
        randomDirection = Vector3.Slerp(randomDirection, Vector3.down, diveSharpness * 0.1f).normalized;

        // Combine with opposite direction from lure (if available)
        if (currentLure != null)
        {
            Vector3 awayFromLure = (transform.position - currentLure.position).normalized;
            awayFromLure.y = 0; // Keep mostly horizontal component
            giveUpDirection = (randomDirection + awayFromLure * 0.3f).normalized;
        }
        else
        {
            giveUpDirection = randomDirection;
        }
    }

    private void ApplyGiveUp()
    {
        // Move in give up direction
        transform.position += giveUpDirection * chaseSpeed * Time.deltaTime;

        // Rotate to face the give up direction
        if (giveUpDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(giveUpDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        timeOnLure += Time.deltaTime;

        if (timeOnLure >= giveUpDuration)
        {
            Disappear();
        }
    }

    private void Disappear()
    {
        isActive = false;
        gameObject.SetActive(false);
    }

    // Optional debugging visualization
    void OnDrawGizmosSelected()
    {
        // Draw the roam radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(initialPosition, roamRadius);

        // Draw detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw avoid radius if there's an avoid object
        if (avoidObject != null && Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(avoidObject.position, avoidRadius);
        }
    }
}