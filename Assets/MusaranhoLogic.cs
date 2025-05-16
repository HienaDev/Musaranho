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
    public float waitTimeAtBucket = 1.5f;
    public Transform player;
    public GameObject killTrigger;
    private Animator animator;

    [SerializeField] private Transform bucket;

    [Header("Audio")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip screeshClip;
    [SerializeField] private AudioClip sniffingClip; // 🆕 Add sniffing clip here

    private bool activatedSpeed = true;
    private bool screeshPlayed = false;

    private void Update()
    {
        if (!isInitialized || waitingAtBucket)
        {
            UpdateFootstepAudio(false);
            return;
        }

        bool isMoving = false;

        if (goingToBucket)
        {
            isMoving = MoveTowards(bucketPosition);

            if (Vector3.Distance(transform.position, bucketPosition) < 0.1f)
            {
                goingToBucket = false;
                StartCoroutine(WaitAtBucket());
            }
        }
        else if (returningToStart)
        {
            isMoving = MoveTowards(startingPosition);

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
            isMoving = MoveTowards(player.position);
        }

        UpdateFootstepAudio(isMoving);
    }

    private void UpdateFootstepAudio(bool isMoving)
    {
        if (footstepSource == null) return;

        if (isMoving)
        {
            if (!footstepSource.isPlaying)
                footstepSource.Play();

            footstepSource.volume = 1f;
        }
        else
        {
            footstepSource.volume = 0f;
            footstepSource.Stop();
        }
    }

    public void ActivatedBadMusaranho() => InitalizeMusaranho(bucket.position, false);
    public void ActivatedGoodMusaranho() => InitalizeMusaranho(bucket.position, true);

    public void InitalizeMusaranho(Vector3 bucketPos, bool correctWeight)
    {
        animator = GetComponent<Animator>();
        startingPosition = transform.position;
        bucketPosition = bucketPos;
        hasRightWeight = correctWeight;
        isInitialized = true;
        goingToBucket = true;
        screeshPlayed = false;

        if (killTrigger != null)
            killTrigger.SetActive(false);
    }

    private bool MoveTowards(Vector3 target)
    {
        if (!activatedSpeed) return false;

        float speed = (chasingPlayer && !hasRightWeight && killTrigger.activeSelf) ? moveSpeed * 3f : moveSpeed;

        Vector3 direction = (target - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction);
            float rotationSpeed = 720f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        return direction.magnitude > 0.01f;
    }

    private IEnumerator WaitAtBucket()
    {
        waitingAtBucket = true;

        if (animator != null)
            animator.enabled = false;

        UpdateFootstepAudio(false);

        // 🆕 Start sniffing sound
        if (sfxSource != null && sniffingClip != null)
        {
            sfxSource.clip = sniffingClip;
            sfxSource.loop = true;
            sfxSource.Play();
        }

        yield return new WaitForSeconds(waitTimeAtBucket);

        // 🆕 Stop sniffing sound
        if (sfxSource != null && sfxSource.clip == sniffingClip)
        {
            sfxSource.Stop();
            sfxSource.clip = null;
            sfxSource.loop = false;
        }

        if (hasRightWeight)
        {
            returningToStart = true;
        }
        else
        {
            chasingPlayer = true;

            if (killTrigger != null)
                killTrigger.SetActive(true);

            if (!screeshPlayed && sfxSource != null && screeshClip != null)
            {
                sfxSource.PlayOneShot(screeshClip);
                screeshPlayed = true;
            }
        }

        if (animator != null)
            animator.enabled = true;

        waitingAtBucket = false;
    }

    public void ActivateSpeed() => activatedSpeed = true;
    public void DeactivateSpeed() => activatedSpeed = false;
}
