using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using AndreStuff;
using UnityEngine.Timeline;

public class FishingController : MonoBehaviour, IItem
{
    [Header("Setup")]
    public Transform player;
    public Transform fishingRod;
    public Transform fishingLine;
    [Header("Fishing Settings")]
    public float castDistance = 10f;
    public float reelSpeed = 5f;
    private bool isCasting = false;
    public bool isCast = false;
    private bool isReeling = false;
    public bool RodBusy => isCasting || isReeling || isCast || isFishing || hasFishHooked;
    public bool hasRodEquipped = true;

    [Header("Fishing Effects")]
    [SerializeField] private GameObject lure;
    [SerializeField] private GameObject bait;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject throwableLure;
    [SerializeField] private Transform rodTip;
    [SerializeField] private GameObject particleSystemRipplesLure;

    private float chargeTime;
    private float startedCharging;

    [SerializeField] private FishSpawner fishManager;
    private bool fishBiting = false;

    [Header("Fishing Minigame")]
    [SerializeField] private GameObject UI;
    [SerializeField] private Image slider;
    private float sliderDistortion = 0;
    [SerializeField] private RectTransform bar;
    private float barDistortion = 0.05f;
    private float startingBarSize;
    [SerializeField] private Image hitMarker;
    private float hitMarkerDistortion = 0.05f;
    [SerializeField] private Vector2 barHeighLimits = new Vector2(-230, 230);
    private float currentBarPosition;
    public bool isFishing = false;

    [SerializeField] private float barAcceleration = 400f; // Acceleration when space is pressed
    [SerializeField] private float barGravity = 200f; // Downward acceleration when space is released
    [SerializeField] private float directionChangeMultiplier = 2.0f; // Multiplier when changing direction
    private float barVelocity = 0f; // Current velocity of the bar
    private Vector3 barStartPosition; // Initial position of the bar

    [SerializeField] private float timeNeededToReel = 3f;
    [SerializeField] private float timeToLoseFish = 3f;
    private float currentTimeReeled = 0f;
    private float currentTimeLost = 0f;
    private int numberOfReels = 2;
    private int currentAmountOfReels = 0;
    [SerializeField] private float bringFishUpRadius = 5f;

    [SerializeField] private GameManager gameManager;

    private Fish currentfish;
    private GameObject hookedFish;
    private bool hasFishHooked = false;


    [SerializeField] private Transform hand;

    private void Awake()
    {
        startingBarSize = bar.sizeDelta.x;
    }

    public void ToggleFishingRod(bool toggle)
    {
        hasRodEquipped = toggle;

        if (toggle)
        {
            fishingRod.gameObject.SetActive(true);
            lure.SetActive(true);
            throwableLure.SetActive(false);
            animator.SetTrigger("RodIdle");

            


        }
        else
        {

            if (hasFishHooked)
            {
                if (currentfish != null)
                {
                    currentfish.SetFishingState(false);
                }
                hasFishHooked = false;
                bait.SetActive(true);
                hookedFish.AddComponent<Rigidbody>();
                hookedFish.AddComponent<EquipInteractable>();
                hookedFish.GetComponent<Collider>().isTrigger = false;
                hookedFish.transform.parent = null;
                hookedFish = null;
            }

            if (currentfish != null)
            {
                currentfish.SetFishingState(false);
            }

            currentfish = null;
            ToggleFishingUI(false);
            

            isFishing = false;
            isCast = false;
            isCasting = false;

            isReeling = false;
            currentAmountOfReels = 0;
            currentTimeReeled = 0f;
            currentTimeLost = 0f;
            fishBiting = false;


            fishingRod.gameObject.SetActive(false);
            lure.SetActive(false);
            throwableLure.SetActive(false);
            animator.SetTrigger("Nothing");
        }
    }

    // LeftClickItem method to start casting the fishing line
    public void LeftClickItem()
    {
        if (!isCasting && !isFishing)
        {
            if (isCast)
            {
                if (fishBiting)
                {
                    StartFishingMinigame();
                }
                else
                    UncastLine();
            }
            else
            {
                CastLine();
            }
        }
    }

    private void ToggleFishingUI(bool toggle)
    {


        if (toggle)
        {
            UI.SetActive(toggle);
            hitMarker.enabled = true;
            slider.material.DOFloat(sliderDistortion, "_DistortionStrength", 1);
            bar.GetComponent<Image>().material.DOFloat(barDistortion, "_DistortionStrength", 1);
            hitMarker.material.DOFloat(hitMarkerDistortion, "_DistortionStrength", 1);
        }
        else
        {
            slider.material.DOFloat(1f, "_DistortionStrength", 1);
            bar.GetComponent<Image>().material.DOFloat(1f, "_DistortionStrength", 1);
            hitMarker.material.DOFloat(1f, "_DistortionStrength", 1).OnComplete(() => UI.SetActive(toggle));
        }
    }
    private void StartFishingMinigame()
    {
        StartCoroutine(StartFishingGameCR(1f));
    }

    private IEnumerator StartFishingGameCR(float time)
    {
        hitMarker.enabled = true;
        isFishing = true;
        ToggleFishingUI(true);

        switch (currentfish.fishData.WeighType)
        {
            case FishWeight.LIGHT:
                numberOfReels = 2;
                break;
            case FishWeight.MEDIUM:
                numberOfReels = 3;
                break;
            case FishWeight.HEAVY:
                numberOfReels = 4;
                break;
            case FishWeight.GIANT:
                numberOfReels = 5;
                break;
        }

        yield return new WaitForSeconds(time);
        if (currentfish != null)
        {
            currentfish.SetFishingState(true);
        }
        hitMarker.transform.localPosition = new Vector3(hitMarker.transform.localPosition.x, Random.Range(barHeighLimits.x + hitMarker.rectTransform.sizeDelta.y / 2, barHeighLimits.y - hitMarker.rectTransform.sizeDelta.y / 2), hitMarker.transform.localPosition.z);



        barVelocity = 0f;
        barStartPosition = bar.localPosition;
    }

    // LeftHoldItem method to hold the fishing line
    public void LeftHoldItem()
    {
        if (isCast)
        {
            //ReelInLine();
        }
    }
    // LeftReleaseItem method to release the fishing line
    public void LeftReleaseItem()
    {
        if (isCasting)
        {
            ReleaseLine();
        }
    }
    // RightClickItem method for any right-click action (not implemented)
    public void RightClickItem()
    {
        if (hasFishHooked)
        {
            if (currentfish != null)
            {
                currentfish.SetFishingState(false);
            }
            hasFishHooked = false;
            bait.SetActive(true);
            hookedFish.AddComponent<Rigidbody>();
            hookedFish.AddComponent<EquipInteractable>();
            hookedFish.GetComponent<Collider>().isTrigger = false;
            hookedFish.transform.parent = null;
            hookedFish = null;
        }
        else if (isCast)
        {
            UncastLine();
        }
    }



    // RightHoldItem method for any right-hold action (not implemented)
    public void RightHoldItem() { }
    // RightReleaseItem method for any right-release action (not implemented)
    public void RightReleaseItem() { }
    private void CastLine()
    {
        if(currentfish != null)
        {
            currentfish.SetFishingState(false);
            currentfish = null;
        }
        isCasting = true;
        // Logic to cast the fishing line
        Debug.Log("Casting line...");
        // Add casting animation or effects here
        animator.SetTrigger("Cast");

        startedCharging = Time.time;
    }

    public void TogglePullingLine(bool toggle, Fish fish)
    {
        if (isFishing)
        {
            return;
        }

        particleSystemRipplesLure.SetActive(toggle);
        if (!toggle)
        {
            if (currentfish == fish)
            {
                currentfish = null;
            }
        }
        else
            currentfish = fish;

        fishBiting = toggle;
    }

    private void UncastLine()
    {

        //animator.ResetTrigger("Cast");

        if (currentfish != null)
        {
            currentfish.SetFishingState(false);
            currentfish = null;
        }

        animator.SetTrigger("Uncast");


    }

    public void UncastAnimatorLine()
    {
        fishManager.ToggleFishSpawn(false);
        isCasting = false;
        isReeling = false;
        isCast = false;
        isFishing = false;
        currentAmountOfReels = 0;
        currentTimeReeled = 0f;
        currentTimeLost = 0f;
        // Logic to uncast the fishing line
        Debug.Log("Uncasting line...");
        // Add uncasting animation or effects here
        if(currentfish != null)
        {
            currentfish.SetFishingState(false);
            currentfish = null;
        }
        lure.SetActive(true);
        throwableLure.SetActive(false);
        ToggleFishingUI(false);
    }

    private void ReelInLine()
    {
        isReeling = true;
        // Logic to reel in the fishing line
        Debug.Log("Reeling in...");
        // Add reeling animation or effects here
    }

    private void ReleaseLine()
    {
        isCasting = false;
        isReeling = false;
        isCast = true;
        // Logic to release the fishing line
        Debug.Log("Releasing line...");
        // Add releasing animation or effects here

        animator.SetTrigger("Throw");

        lure.SetActive(false);

    }

    public void ThrowLure()
    {
        if (lure.activeSelf)
            return;

        throwableLure.SetActive(true);
        throwableLure.transform.position = rodTip.position;
        float castDistanceMultiplier = Mathf.Min((Time.time - startedCharging), 2f);
        throwableLure.GetComponent<Rigidbody>().AddForce(player.forward * (castDistance), ForceMode.Impulse);

        fishManager.ToggleFishSpawn(true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (UI != null)
        {
            ToggleFishingUI(false);
        }




        if (bar != null)
        {
            barStartPosition = bar.localPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            if(hasRodEquipped)
            {
                ToggleFishingRod(false);
            }
            else if (hand.childCount == 0)
            {
                ToggleFishingRod(true);
            }

        }

        if (!hasRodEquipped)
            return;

        if (Input.GetButtonDown("Fire1"))
        {
            LeftClickItem();
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            LeftReleaseItem();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            RightClickItem();
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            RightReleaseItem();
        }

        if (isFishing)
        {
            UpdateFishingBar();
        }
    }

    private void UpdateFishingBar()
    {
        float accelerationToApply;

        // Check if spacebar is pressed
        if (Input.GetButton("Fire1"))
        {
            // If moving down but trying to go up, apply enhanced acceleration
            if (barVelocity < 0)
            {
                accelerationToApply = barAcceleration * directionChangeMultiplier;
            }
            else
            {
                accelerationToApply = barAcceleration;
            }

            // Apply upward acceleration
            barVelocity += accelerationToApply * Time.deltaTime;
        }
        else
        {
            // If moving up but now falling, apply enhanced gravity
            if (barVelocity > 0)
            {
                accelerationToApply = barGravity * directionChangeMultiplier;
            }
            else
            {
                accelerationToApply = barGravity;
            }

            // Apply downward acceleration (gravity)
            barVelocity -= accelerationToApply * Time.deltaTime;
        }

        // Move the bar
        Vector3 newPosition = bar.localPosition;
        newPosition.y += barVelocity * Time.deltaTime;

        // Clamp position within limits
        if (newPosition.y <= barHeighLimits.x)
        {
            newPosition.y = barHeighLimits.x;
            barVelocity = 0; // Stop at bottom limit
        }
        else if (newPosition.y >= barHeighLimits.y)
        {
            newPosition.y = barHeighLimits.y;
            barVelocity = 0; // Stop at top limit
        }

        // Update bar position
        bar.localPosition = newPosition;
        currentBarPosition = bar.localPosition.y;



        if (currentBarPosition < hitMarker.transform.localPosition.y + hitMarker.rectTransform.sizeDelta.y / 2 &&
            currentBarPosition > hitMarker.transform.localPosition.y - hitMarker.rectTransform.sizeDelta.y / 2)
        {
            // Hit marker logic
            hitMarker.color = Color.green; // Change color to red when hit
            currentTimeReeled += Time.deltaTime;


            if (currentTimeLost > 0f)
            {
                currentTimeLost -= Time.deltaTime;
            }

            bar.sizeDelta = new Vector2(Mathf.Lerp(startingBarSize, 0f, currentTimeLost / timeToLoseFish), bar.sizeDelta.y);


            if (currentTimeReeled > timeNeededToReel)
            {
                currentTimeLost = 0f;
                currentAmountOfReels++;
                currentTimeReeled = 0f;
                ReelInFish();
                if (currentAmountOfReels >= numberOfReels)
                {
                    Debug.Log("Fish caught!");
                    hitMarker.enabled = false;
                }
                else
                    hitMarker.transform.localPosition = new Vector3(hitMarker.transform.localPosition.x, Random.Range(barHeighLimits.x + hitMarker.rectTransform.sizeDelta.y / 2, barHeighLimits.y - hitMarker.rectTransform.sizeDelta.y / 2), hitMarker.transform.localPosition.z);


            }
        }
        else
        {
            hitMarker.color = Color.red; // Reset color when not hit
            currentTimeLost += Time.deltaTime;

            bar.sizeDelta = new Vector2(Mathf.Lerp(startingBarSize, 0f, currentTimeLost / timeToLoseFish), bar.sizeDelta.y);

            if (currentTimeLost > timeToLoseFish)
            {
                currentTimeLost = 0f;
                UncastLine();
                currentfish.SetFishingState(false);
            }
        }

        // Debug information
        Debug.Log($"Bar Velocity: {barVelocity}, Position: {currentBarPosition}");
    }

    private void ReelInFish()
    {
        if (throwableLure == null)
            return;

        // Get the lure position
        Vector3 lurePosition = throwableLure.transform.position;

        // Get the player position
        Vector3 playerPosition = player.position;

        // Use the same Y position as the lure for the calculation to make it a circle instead of a sphere
        Vector3 playerPositionSameY = new Vector3(playerPosition.x, lurePosition.y, playerPosition.z);

        // Get direction from player to lure (in the XZ plane only)
        Vector3 direction = lurePosition - playerPositionSameY;

        // If the lure is already within the radius, no need to reel
        float currentDistance = direction.magnitude;
        if (currentDistance <= bringFishUpRadius)
        {
            currentfish.transform.position = lure.transform.position;
            currentfish.transform.parent = lure.transform;
            currentfish.transform.localEulerAngles = new Vector3(-90, 0, 0);
            currentfish.SetFishShakingSpeed(3);
            currentfish.GetComponent<Fish>().enabled = false;
            hasFishHooked = true;
            hookedFish = currentfish.gameObject;
            bait.SetActive(false);
            UncastLine();
            return;
        }
            

        // Normalize the direction and multiply by radius to find the point on the circle
        Vector3 pointOnCircle = playerPositionSameY + direction.normalized * bringFishUpRadius;

        // Calculate total distance from lure to the point on circle
        float totalDistance = Vector3.Distance(lurePosition, pointOnCircle);

        // Calculate how much we should move toward the circle based on current reel progress
        float progressPerReel = 1.0f / numberOfReels;
        float currentProgress = progressPerReel * currentAmountOfReels;

        // Calculate the target position for this reel
        Vector3 targetPosition = Vector3.Lerp(lurePosition, pointOnCircle, currentProgress);

        // Use DOTween to animate the movement
        throwableLure.transform.DOMove(targetPosition, 1.0f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            if (currentAmountOfReels >= numberOfReels)
            {
                currentfish.transform.position = lure.transform.position;
                currentfish.transform.parent = lure.transform;
                currentfish.transform.localEulerAngles = new Vector3(-90, 0, 0);
                currentfish.SetFishShakingSpeed(3);
                currentfish.GetComponent<Fish>().enabled = false;
                hasFishHooked = true;
                hookedFish = currentfish.gameObject;
                bait.SetActive(false);
                UncastLine();
            }
        });

        Debug.Log($"Reeling fish: {currentAmountOfReels}/{numberOfReels}, Distance: {currentDistance} -> {Vector3.Distance(targetPosition, playerPositionSameY)}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, bringFishUpRadius);
    }
}