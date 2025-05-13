using AndreStuff;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

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

    private void InteractedWithInteractable(GameObject objectInteractable)
    {
        if (objectInteractable.TryGetComponent(out MeasureFishInteractable measureFishInteractable))
        {
            // returns true if complete
            measureFishInteractable.MeasureAround();
        }
    }

    public void UpdateCamSensitivity()
    {
        _playerMovement.UpdateSensitivity(gameManager.GetPlayerCameraSensitivity());
    }
}
