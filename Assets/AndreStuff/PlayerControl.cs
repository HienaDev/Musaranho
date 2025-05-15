using AndreStuff;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private GameManager gameManager;

    [SerializeField] private Transform hand;
    [SerializeField] private FishingController fishingController;
    public GameManager GetGameManager() => gameManager;

    private PlayerMovement _playerMovement;

    private FishHolder _fishHolder;
    public void SetupPlayerMovement(PlayerMovement playerMovement) => _playerMovement = playerMovement;

    [SerializeField] private Transform bucket;

    private Camera _cam;
    private void Start()
    {
        _fishHolder = FindAnyObjectByType<FishHolder>();
        _cam = Camera.main;
        UpdateCamSensitivity();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) gameManager.ClickEscapeButton();

        // Q = drop
        if (Input.GetKeyDown(KeyCode.Q) && HasObjectInHand())
        {
            Vector3 throwDirection = _cam.transform.forward;
            Debug.DrawRay(_cam.transform.position, throwDirection * 10f, Color.yellow, 0.5f);
            DropObject(throwDirection * throwForce);
        }

        // E = interact
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit[] hits = Physics.RaycastAll(_cam.transform.position, _cam.transform.forward, 100.0F);
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.transform.gameObject == gameObject || !hit.transform.CompareTag("Interactable")) continue;
                Debug.DrawRay(_cam.transform.position, _cam.transform.forward * hit.distance, Color.yellow, 0.5f);
                InteractedWithInteractable(hit.transform.gameObject);
                break;
            }
        }

        // O = spawn bucket 2 units in front of player
        if (Input.GetKeyDown(KeyCode.O) && bucket != null)
        {
            Vector3 spawnPos = _cam.transform.position + _cam.transform.forward * 2f;
            bucket.position = spawnPos;
        }
    }

    private bool HasObjectInHand()
    {
        return hand.childCount > 0;
    }

    private void InteractedWithInteractable(GameObject objectInteractable)
    {
        if (objectInteractable.TryGetComponent(out Interactable interactable))
        {
            interactable.Interact();
        }

        if (objectInteractable.TryGetComponent(out MeasureFishInteractable measureFishInteractable))
        {
            measureFishInteractable.MeasureAround();
        }

        if (objectInteractable.TryGetComponent(out EquipInteractable equipInteractable))
        {
            fishingController.ToggleFishingRod(false);
            equipInteractable.Equipped();
            if (!equipInteractable.gameObject.activeSelf)
                equipInteractable.gameObject.SetActive(true);
            equipInteractable.transform.parent = hand;
            equipInteractable.transform.localPosition = Vector3.zero;
            equipInteractable.transform.localEulerAngles = Vector3.zero;

            if (objectInteractable.TryGetComponent(out Fish fishScript) && !fishingController.RodBusy)
            {
                fishScript.RotateFish(true);
            }

            if (objectInteractable.TryGetComponent(out RotateForHand rotate))
            {
                rotate.Rotate();
            }

            if (equipInteractable.TryGetComponent(out FishManager fishManager) && equipInteractable.TryGetComponent(out BucketChecker bucketChecker))
            {
                bucketChecker.enabled = false;
            }
        }
    }

    public void UpdateCamSensitivity()
    {
        _playerMovement.UpdateSensitivity(gameManager.GetPlayerCameraSensitivity());
    }

    private void DropObject(Vector3 force)
    {
        if (!HasObjectInHand()) return;
        Transform heldObject = hand.GetChild(0);
        heldObject.SetParent(_fishHolder.transform);
        if (heldObject.TryGetComponent(out EquipInteractable equipInteractable)) equipInteractable.Unequipped();

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        rb.AddForce(force, ForceMode.Impulse);
        if (heldObject.TryGetComponent(out FishManager fishManager))
        {
            if (heldObject.TryGetComponent(out BucketChecker bucketChecker))
            {
                bucketChecker.enabled = true;
                return;
            }
            heldObject.gameObject.AddComponent<BucketChecker>();
        }
    }
}
