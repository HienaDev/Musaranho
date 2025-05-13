using UnityEngine;

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

    private float chargeTime;
    private float startedCharging;

    // LeftClickItem method to start casting the fishing line
    public void LeftClickItem()
    {
        if (!isCasting)
        {
            CastLine();
        }
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
        if(isCast)
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

 

    private void UncastLine()
    {
        isCasting = false;
        isReeling = false;
        isCast = false;
        // Logic to uncast the fishing line
        Debug.Log("Uncasting line...");
        // Add uncasting animation or effects here

        animator.SetTrigger("Uncast");

        lure.SetActive(true);
        throwableLure.SetActive(false);
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
        throwableLure.GetComponent<Rigidbody>().AddForce(fishingRod.forward * (castDistance), ForceMode.Impulse);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
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
    }
}
