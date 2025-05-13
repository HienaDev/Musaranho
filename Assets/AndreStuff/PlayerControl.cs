using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    [SerializeField] private GameManager gameManager;
    public GameManager GetGameManager() => gameManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) gameManager.ClickEscapeButton();
    }
}
