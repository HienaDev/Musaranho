using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Transform canvasMenu;
    
    private bool _canPlayerMove = false;
    public bool CanPlayerMove() => _canPlayerMove;
    public void SetPlayerMove(bool value) => _canPlayerMove = value;
    
    private void Start()
    {
        _canPlayerMove = true;
        canvasMenu.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ClickEscapeButton()
    {
        if (_canPlayerMove) OpenPauseMenu();
        else ClosePauseMenu();
    }
    
    private void OpenPauseMenu()
    {
        _canPlayerMove = false;
        canvasMenu.gameObject.SetActive(!_canPlayerMove);
        Cursor.visible = true;
    }

    private void ClosePauseMenu()
    {
        _canPlayerMove = true;
        canvasMenu.gameObject.SetActive(!_canPlayerMove);
        Cursor.visible = false;
    }
}
