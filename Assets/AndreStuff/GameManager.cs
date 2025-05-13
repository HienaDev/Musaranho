using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Transform canvasMenu;
    [SerializeField] private PlayerControl playerControl;
    
    private bool _canPlayerMove = false;
    public bool CanPlayerMove() => _canPlayerMove;
    public void SetPlayerMove(bool value) => _canPlayerMove = value;

    private float _playerCameraSense = 2f;
    public float GetPlayerCameraSensitivity() => _playerCameraSense;
    
    private void Start()
    {
        _canPlayerMove = true;
        canvasMenu.gameObject.SetActive(false);
        //Cursor.lockState = CursorLockMode.Locked;
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
    
    /*
     * MENU OPTIONS STUFF
     */
    [Space][Header("Menu Options Stuff")]
    [SerializeField] private TMP_InputField sensitivityInputField;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private float minSensitivity = 0.01f;
    [SerializeField] private float maxSensitivity = 10f;
    
    public void ChangedSenseInput()
    {
        UpdateNewSense(float.Parse(sensitivityInputField.text));
    }
    public void ChangedSenseSlider()
    {
        UpdateNewSense(sensitivitySlider.value);
    }
    private void UpdateInputFieldValue(string value) => sensitivityInputField.text = value;
    private void UpdateSliderValue(float value) => sensitivitySlider.value = value;

    private void UpdateNewSense(float value)
    {
        value = Mathf.Max(value, minSensitivity);
        value = Mathf.Min(value, maxSensitivity);
        UpdateInputFieldValue(value.ToString());
        UpdateSliderValue(value);
        _playerCameraSense = value;
        playerControl.UpdateCamSensitivity();
    }

}
