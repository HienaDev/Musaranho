using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField] private Transform mouth;
    [SerializeField] private float minDistanceToLure = 0.1f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float maxTimeOnLure = 10f;
    [SerializeField] private float giveUpDuration = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float diveAngleRange = 30f; // How wide the random dive angles can be
    [SerializeField] private float diveSharpness = 2f; // How sharply they dive downward




    private Transform lureTarget;
    private float timeOnLure = 0f;
    private bool hasGivenUp = false;
    private Vector3 giveUpDirection;
    private bool isActive = true;
    private bool ripplesActive = false;

    private void Start()
    {
        
    }

    public void SetLureTarget(Transform target)
    {
        lureTarget = target;
        timeOnLure = 0f;
        hasGivenUp = false;
        isActive = true;
        ripplesActive = false;
    }

    void Update()
    {
        if (!isActive) return;

        if (hasGivenUp)
        {
            // Move in give up direction
            transform.position += giveUpDirection * speed * Time.deltaTime;

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
            return;
        }

        if (lureTarget != null)
        {
            float distance = Vector3.Distance(mouth.position, lureTarget.position);
            if (distance <= minDistanceToLure && !ripplesActive)
            {
                ripplesActive = true;
                //activate ripples
            }

            Vector3 direction = (lureTarget.position - mouth.position).normalized;

            if (distance > minDistanceToLure)
            {
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
                transform.position += transform.forward * speed * Time.deltaTime;
            }

            if (ripplesActive)
            {
                timeOnLure += Time.deltaTime;

                if (timeOnLure >= maxTimeOnLure)
                {
                    GiveUp();
                }
            }
        }
    }

    private void GiveUp()
    {
        hasGivenUp = true;
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
        if (lureTarget != null)
        {
            Vector3 awayFromLure = (transform.position - lureTarget.position).normalized;
            awayFromLure.y = 0; // Keep mostly horizontal component
            giveUpDirection = (randomDirection + awayFromLure * 0.3f).normalized;
        }
        else
        {
            giveUpDirection = randomDirection;
        }
    }

    private void Disappear()
    {
        isActive = false;
        gameObject.SetActive(false);
    }
}