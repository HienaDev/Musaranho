using AndreStuff;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    [SerializeField] private float throwForce = 10f;
    [SerializeField] private GameManager gameManager;
    public GameManager GetGameManager() => gameManager;

    private PlayerMovement _playerMovement;
    public void SetupPlayerMovement(PlayerMovement playerMovement) => _playerMovement = playerMovement;

    private Camera _cam;
    private void Start()
    {
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
            DropObject(throwDirection * throwForce);
        }
        
        // E = interact
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out hit, Mathf.Infinity))
            { 
                Debug.DrawRay(_cam.transform.position, _cam.transform.forward * hit.distance, Color.yellow, 0.1f);
                if (hit.transform.CompareTag("Interactable"))
                {
                    Debug.DrawRay(_cam.transform.position, _cam.transform.forward * hit.distance, Color.yellow, 0.5f);
                    InteractedWithInteractable(hit.transform.gameObject);
                }
            }
        }
    }
    
    private bool HasObjectInHand()
    {
        return transform.Find("Hand").childCount > 0;
    }

    private void InteractedWithInteractable(GameObject objectInteractable)
    {
        if (objectInteractable.TryGetComponent(out MeasureFishInteractable measureFishInteractable))
        {
            // returns true if complete
            measureFishInteractable.MeasureAround();
        }

        if (objectInteractable.TryGetComponent(out EquipInteractable equipInteractable))
        {
            equipInteractable.Equipped();
            equipInteractable.transform.parent = transform.Find("Hand");
            equipInteractable.transform.localPosition = Vector3.zero;
        }
    }

    public void UpdateCamSensitivity()
    {
        _playerMovement.UpdateSensitivity(gameManager.GetPlayerCameraSensitivity());
    }

    private void DropObject(Vector3 force)
    {
        if (!HasObjectInHand()) return;
        Transform heldObject = transform.Find("Hand").GetChild(0);
        heldObject.SetParent(null);
        heldObject.position += new Vector3(0f, 0.5f, 0f);
        if (heldObject.TryGetComponent(out EquipInteractable equipInteractable)) equipInteractable.Unequipped();
        
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        rb.AddForce(force, ForceMode.Impulse);
        
    }
}