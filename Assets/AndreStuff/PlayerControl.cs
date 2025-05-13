using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    [SerializeField] private GameManager gameManager;
    public GameManager GetGameManager() => gameManager;

    private PlayerMovement _playerMovement;
    public void SetupPlayerMovement(PlayerMovement playerMovement) => _playerMovement = playerMovement;

    private void Start()
    {
        UpdateCamSensitivity();
    }

    private void Update()
    {
        // cause of editor, changed to SPACE
        if (Input.GetKeyDown(KeyCode.Space)) gameManager.ClickEscapeButton();
    }

    public void UpdateCamSensitivity()
    {
        _playerMovement.UpdateSensitivity(gameManager.GetPlayerCameraSensitivity());
    }
}
