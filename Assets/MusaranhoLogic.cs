using UnityEngine;

public class MusaranhoLogic : MonoBehaviour
{
    private Vector3 startingPosition;
    private Vector3 bucketPosition;
    private bool hasRightWeight;

    private bool isInitialized = false;
    private bool goingToBucket = false;
    private bool returningToStart = false;
    private bool chasingPlayer = false;

    public float moveSpeed = 3f;
    public Transform player;
    public GameObject killTrigger;
    Animator animator;

    [SerializeField] private Transform bucket;

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
        if (!isInitialized) return;

        if (goingToBucket)
        {
            MoveTowards(bucketPosition);

            if (Vector3.Distance(transform.position, bucketPosition) < 0.1f)
            {
                goingToBucket = false;

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
            
            animator.SetTrigger("Running");
            MoveTowards(player.position);
        }
    }

    private void MoveTowards(Vector3 target)
    {
        if(!hasRightWeight && killTrigger.activeSelf)
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * 2 * Time.deltaTime);

        else
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }
}
