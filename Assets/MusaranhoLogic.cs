using UnityEngine;
using System.Collections;

public class MusaranhoLogic : MonoBehaviour
{
    private Vector3 startingPosition;
    private Vector3 bucketPosition;
    private bool hasRightWeight;

    private bool isInitialized = false;
    private bool goingToBucket = false;
    private bool returningToStart = false;
    private bool chasingPlayer = false;
    private bool waitingAtBucket = false;

    public float moveSpeed = 3f;
    public float waitTimeAtBucket = 1.5f; // Time to pause at the bucket
    public Transform player;
    public GameObject killTrigger;
    private Animator animator;

    [SerializeField] private Transform bucket;

    private bool activatedSpeed = true;

    public void ActivatedBadMusaranho()
    {
        InitalizeMusaranho(bucket.position, false);
    }

    public void ActivatedGoodMusaranho()
    {
        InitalizeMusaranho(bucket.position, true);
    }

    public void InitalizeMusaranho(Vector3 bucketPos, bool correctWeight)
    {
        animator = GetComponent<Animator>();
        startingPosition = transform.position;
        bucketPosition = bucketPos;
        hasRightWeight = correctWeight;
        isInitialized = true;
        goingToBucket = true;

        if (killTrigger != null)
        {
            killTrigger.SetActive(false);
        }
    }

    void Update()
    {
        if (!isInitialized || waitingAtBucket) return;

        if (goingToBucket)
        {
            MoveTowards(bucketPosition);

            if (Vector3.Distance(transform.position, bucketPosition) < 0.1f)
            {
                goingToBucket = false;
                StartCoroutine(WaitAtBucket());
            }
        }
        else if (returningToStart)
        {
            MoveTowards(startingPosition);

            if (Vector3.Distance(transform.position, startingPosition) < 0.1f)
            {
                returningToStart = false;
                isInitialized = false;
            }
        }
        else if (chasingPlayer && player != null)
        {
            if (animator != null && !animator.enabled)
                animator.enabled = true;

            animator.SetTrigger("Running");
            MoveTowards(player.position);
        }
    }

    public void ActivateSpeed()
    {
        activatedSpeed = true;
    }

    public void DeactivateSpeed()
    {
        activatedSpeed = false;
    }

    private void MoveTowards(Vector3 target)
    {
        if (!activatedSpeed)
            return;

        float speed = (chasingPlayer && !hasRightWeight && killTrigger.activeSelf) ? moveSpeed * 3f : moveSpeed;

        // Movement
        Vector3 direction = (target - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Rotation (sharp turning)
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction);
            float rotationSpeed = 720f; // Degrees per second — adjust for sharpness
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private IEnumerator WaitAtBucket()
    {
        waitingAtBucket = true;

        if (animator != null)
            animator.enabled = false; // Disable animator during pause

        yield return new WaitForSeconds(waitTimeAtBucket);

        if (hasRightWeight)
        {
            returningToStart = true;
        }
        else
        {
            chasingPlayer = true;
            if (killTrigger != null)
                killTrigger.SetActive(true);
        }

        if (animator != null)
            animator.enabled = true; // Re-enable animator after pause

        waitingAtBucket = false;
    }
}
