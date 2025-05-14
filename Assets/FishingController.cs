using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    private bool isCast = false;
    private bool isReeling = false;

    [Header("Fishing Effects")]
    [SerializeField] private GameObject lure;
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
    [SerializeField] private Transform bar;
    [SerializeField] private Image hitMarker;
    [SerializeField] private Vector2 barHeighLimits = new Vector2(-230, 230);
    private float currentBarPosition;
    private bool isFishing = false;

    [SerializeField] private float barAcceleration = 400f; // Acceleration when space is pressed
    [SerializeField] private float barGravity = 200f; // Downward acceleration when space is released
    [SerializeField] private float directionChangeMultiplier = 2.0f; // Multiplier when changing direction
    private float barVelocity = 0f; // Current velocity of the bar
    private Vector3 barStartPosition; // Initial position of the bar

    [SerializeField] private float timeNeededToReel = 4f;
    private float currentTimeReeled = 0f;
    private int numberOfReels = 2;

    [SerializeField] private GameManager gameManager;

    // LeftClickItem method to start casting the fishing line
    public void LeftClickItem()
    {
        if (!isCasting)
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

    private void StartFishingMinigame()
    {
        isFishing = true;
        UI.SetActive(true);
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
        if (isCast)
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

        isCasting = true;
        // Logic to cast the fishing line
        Debug.Log("Casting line...");
        // Add casting animation or effects here
        animator.SetTrigger("Cast");

        startedCharging = Time.time;
    }

    public void TogglePullingLine(bool toggle)
    {
        particleSystemRipplesLure.SetActive(toggle);
        fishBiting = toggle;
    }

    private void UncastLine()
    {
        isCasting = false;
        isReeling = false;
        isCast = false;
        isFishing = false;
        // Logic to uncast the fishing line
        Debug.Log("Uncasting line...");
        // Add uncasting animation or effects here

        animator.SetTrigger("Uncast");

        lure.SetActive(true);
        throwableLure.SetActive(false);
        UI.SetActive(false);
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
            UI.SetActive(false);
        }

        if (isFishing)
            UI.SetActive(true);

        if (bar != null)
        {
            barStartPosition = bar.localPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
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

        

        if(currentBarPosition < hitMarker.transform.localPosition.y + hitMarker.rectTransform.sizeDelta.y / 2 && 
            currentBarPosition > hitMarker.transform.localPosition.y - hitMarker.rectTransform.sizeDelta.y / 2)
        {
            // Hit marker logic
            hitMarker.color = Color.green; // Change color to red when hit
        }
        else
        {
            hitMarker.color = Color.red; // Reset color when not hit
        }

        // Debug information
        Debug.Log($"Bar Velocity: {barVelocity}, Position: {currentBarPosition}");
    }
}