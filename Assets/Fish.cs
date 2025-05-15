using AndreStuff;
using System.Collections;
using System.IO;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField] private float minDistanceToLure = 0.1f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float maxTimeOnLure = 10f;
    [SerializeField] private float giveUpDuration = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float diveAngleRange = 30f; // How wide the random dive angles can be
    [SerializeField] private float diveSharpness = 2f; // How sharply they dive downward


    private bool beingFished = false;

    private Transform lureTarget;
    private float timeOnLure = 0f;
    private bool hasGivenUp = false;
    private Vector3 giveUpDirection;
    private bool isActive = true;
    private bool ripplesActive = false;
    private bool canGoOnLure = true;

    private FishingController fishingController;  
    
    private Material fishMaterial;
    
    public FishData fishData;

    [SerializeField] private Transform fishRotationPivot;

    private void Awake()
    {
        // Get the material of the fish
        fishMaterial = GetComponentInChildren<Renderer>().material;
    }

    public void RotateFish(bool reverse)
    {
        fishRotationPivot.localEulerAngles = new Vector3(0, reverse ? 180 : 0, 0);
    }
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        fishData = GetComponent<FishManager>().GetFishData();

        switch (fishData.WeighType)
        {
            case FishWeight.LIGHT:
                gameObject.transform.localScale *= 0.25f;
                break;
            case FishWeight.MEDIUM:
                gameObject.transform.localScale *= 0.5f;

                break;
            case FishWeight.HEAVY:
                gameObject.transform.localScale *= 1f;

                break;
            case FishWeight.GIANT:
                gameObject.transform.localScale *= 2f;

                break;
        }
    }

    public void SetFishingState(bool state)
    {
        beingFished = state;
        
        if (!state)
        {
            canGoOnLure = false ;
            GiveUp();
        }
    }

    public void SetFishShakingSpeed(float amount)
    {
        fishMaterial.SetFloat("_ShakingSpeed", amount);
        fishMaterial.SetFloat("_Strength", amount/10);
    }


    public void SetLureTarget(Transform target, FishingController fishingController)
    {
        lureTarget = target;
        timeOnLure = 0f;
        hasGivenUp = false;
        isActive = true;
        ripplesActive = false;
        this.fishingController = fishingController;

        

        if (fishingController.isFishing)
        {
            canGoOnLure = false;
            GiveUp(Random.Range(1f, 3f));
        }
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
            float distance = Vector3.Distance(transform.position, lureTarget.position);
            if (distance <= minDistanceToLure && !ripplesActive && canGoOnLure)
            {
                ripplesActive = true;
                //activate ripples

                fishingController.TogglePullingLine(true, this);
            }

            Vector3 direction = (lureTarget.position - transform.position).normalized;

            if (distance > minDistanceToLure)
            {
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
                transform.position += transform.forward * speed * Time.deltaTime;
            }

            if (ripplesActive && !beingFished)
            {
                timeOnLure += Time.deltaTime;

                if (timeOnLure >= maxTimeOnLure)
                {
                    GiveUp();
                    fishingController.TogglePullingLine(false, this);
                }
            }
        }
    }

    private void GiveUp(float timeToGiveUp)
    {
        
        StartCoroutine(GiveUpCoroutine(timeToGiveUp));
    }

    private IEnumerator GiveUpCoroutine(float timeToGiveUp)
    {
        yield return new WaitForSeconds(timeToGiveUp);
        GiveUp();
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