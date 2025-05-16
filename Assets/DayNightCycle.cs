using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.Events;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private float dayDuration = 120f;
    private float currentTime = 0f;
    private float daySpeed = 1f;

    [SerializeField] private Transform sun; // Rotates from 50 to 250 degrees

    [SerializeField] private Transform boat;
    public bool dayStarted = false;
    [SerializeField] private Transform boatDayPosition;
    private Vector3 boatStartPosition;

    [SerializeField] private Image transitionScreen;
    private Material transitionMaterial;

    [SerializeField] private Transform player;
    [SerializeField] private Transform playerStartPosition;

    public int currentDay = 0;
    private GameManager gameManager;

    private bool firstDay = false;

    [SerializeField] private AudioClip transitionInClip;
    [SerializeField] private AudioClip transitionOutClip;
    private AudioSource audioSource;

    [SerializeField] private BucketInteractable bucketInteractable;
    private Vector3 bucketOriginalPos;

    // Custom curve for slow start then faster motion
    private AnimationCurve slowStartCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.2f, 0.05f),
        new Keyframe(0.5f, 0.3f),
        new Keyframe(0.8f, 0.9f),
        new Keyframe(1f, 1f)
    );

    private Vector3 playerInitialPosition; // Add this at the top with other fields

    void Start()
    {
        bucketOriginalPos = bucketInteractable.transform.position;


        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        gameManager = FindAnyObjectByType<GameManager>();
        boatStartPosition = boat.position;
        playerInitialPosition = player.position; // <--- Save actual start position

        if (transitionScreen != null)
        {
            transitionMaterial = transitionScreen.material;
            if (transitionMaterial == null)
                Debug.LogError("Transition Image has no material assigned.");
        }
    }


    void Update()
    {
        if (dayStarted)
            currentTime += Time.deltaTime * daySpeed;
        else
            return;

        sun.transform.localEulerAngles = new Vector3(
            Mathf.Lerp(0, 250, currentTime / dayDuration),
            sun.transform.localEulerAngles.y,
            sun.transform.localEulerAngles.z
        );

        if (Input.GetKeyDown(KeyCode.K))
        {
            daySpeed *= 5f;
        }

        if (currentTime >= dayDuration && dayStarted)
        {
            EndDay();
        }


    }

    public void TouchWheel()
    {
        if (!dayStarted)
            StartDay();
        else
            EndDay();
    }

    public async void StartNewDay()
    {
        Debug.Log("Starting new day with transition...");

        // Transition in
        await ToggleTransition(true).AsyncWaitForCompletion();
        await Task.Delay(1000); // Optional pause for smoother effect

        // Reset key variables
        currentTime = 0f;
        daySpeed = 1f;
        dayStarted = false;

        // Reset positions
        player.position = playerInitialPosition;
        boat.position = boatStartPosition;
        sun.transform.localEulerAngles = new Vector3(0f, sun.transform.localEulerAngles.y, sun.transform.localEulerAngles.z);

        // Transition out
        await ToggleTransition(false).AsyncWaitForCompletion();

        bucketInteractable.transform.position = bucketOriginalPos;
        bucketInteractable.DestroyAllFish();

        Debug.Log("New day initialized.");
    }



    public async void StartDay()
    {
        Debug.Log("started day");
        dayStarted = true;
        if (firstDay)
        {
            firstDay = false;

        }
        else
            currentDay++;

        await ToggleTransition(true).AsyncWaitForCompletion();
        await Task.Delay(1000);


        boat.transform.position = boatDayPosition.position;
        player.position = playerStartPosition.position;

        await ToggleTransition(false).AsyncWaitForCompletion();
    }

    public async void EndDay()
    {
        Debug.Log("ended day");
        dayStarted = false;
        await ToggleTransition(true).AsyncWaitForCompletion();

        await Task.Delay(1000);

        
        sun.transform.localEulerAngles = new Vector3(250, sun.transform.localEulerAngles.y, sun.transform.localEulerAngles.z);
        boat.transform.position = boatStartPosition;
        player.position = playerStartPosition.position;

        await ToggleTransition(false).AsyncWaitForCompletion();
    }

    public void DayOver()
    {
        // Optional: future use
    }

    public Tween ToggleTransition(bool show)
    {
        if (transitionScreen == null || transitionMaterial == null) return null;

        float endValue = show ? 0f : 25f;
        float duration = 1.5f;

        if (show)
            transitionScreen.gameObject.SetActive(true);

        Tween tween = DOTween.To(
            () => transitionMaterial.GetFloat("_DistortionStrength"),
            x => transitionMaterial.SetFloat("_DistortionStrength", x),
            endValue,
            duration)
            .SetEase(slowStartCurve)
            .OnComplete(() =>
            {
                if (!show)
                    transitionScreen.gameObject.SetActive(false);
            });

        // Only for transition in: schedule audio to play at third keyframe (~50% time)
        if (show && audioSource != null && transitionInClip != null)
        {
            float playTime = duration * 0.5f; // third keyframe is at 0.5
            DOVirtual.DelayedCall(playTime, () =>
            {
                audioSource.PlayOneShot(transitionInClip);
            });
        }

        // Always play transition out sound immediately
        if (!show && audioSource != null && transitionOutClip != null)
        {
            audioSource.PlayOneShot(transitionOutClip);
        }

        return tween;
    }


}
