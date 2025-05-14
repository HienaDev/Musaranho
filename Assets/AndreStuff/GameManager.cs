using System;
using System.Runtime.InteropServices;
using AndreStuff;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField] private PlayerControl playerControl;
    [SerializeField] private Transform mainCanvas;
    
    private bool _canPlayerMove = false;
    public bool CanPlayerMove() => _canPlayerMove;
    public void SetPlayerMove(bool value) => _canPlayerMove = value;

    private float _playerCameraSense = 2f;
    public float GetPlayerCameraSensitivity() => _playerCameraSense;
    private float _crosshairSize = 10f;
    public float GetCrosshairSize() => _crosshairSize;

    private float[] _dailyWeightNeeded;
    /// <summary>
    /// Return value of weight needed for a day.
    /// </summary>
    /// <param name="day">counter of the day, starting at 0</param>
    /// <returns></returns>
    public float GetDailyWeightNeeded(int day = 0)
    {
        return _dailyWeightNeeded[day];
    }
    
    private void Start()
    {
        _canPlayerMove = true;
        canvasMenu.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        LoadJsonData();

        SoundManager.Instance.PlaySound(SoundType.SOUND1);
    }

    private void LoadJsonData()
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>("gameplayData");
        Debug.Log("Text: "+ jsonTextAsset.text);
        GameplayData data = JsonUtility.FromJson<GameplayData>(jsonTextAsset.text);
        _dailyWeightNeeded = data.values;
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CenterCursor();
    }

    private void CenterCursor()
    {
        Vector2 centerPosition = new Vector2(Screen.width / 2, Screen.height / 2);
        Mouse.current.WarpCursorPosition(centerPosition);
    }

    private void ClosePauseMenu()
    {
        _canPlayerMove = true;
        canvasMenu.gameObject.SetActive(!_canPlayerMove);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CenterCursor();
    }

    private void UpdateCrosshairSize()
    {
        if (mainCanvas == null) return;
        if (mainCanvas.Find("Crosshair").TryGetComponent(out RectTransform rectTransform))
        {
            rectTransform.sizeDelta = new Vector2(_crosshairSize, _crosshairSize);
        }
    }
    
    /*
     * MENU OPTIONS STUFF
     */
    [SerializeField] private Transform canvasMenu;
    [Space][Header("Menu Options Stuff")]
    [SerializeField] private TMP_InputField sensitivityInputField;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private float sensitivityMin = 0.01f;
    [SerializeField] private float sensitivityMax = 10f;
    [SerializeField] private TMP_InputField crosshairSizeInputField;
    [SerializeField] private Slider crosshairSizeSlider;
    [SerializeField] private float crosshairSizeMin = 1f;
    [SerializeField] private float crosshairSizeMax = 100f;

    public void ClickOptionsButton()
    {
        canvasMenu.transform.Find("Menu").gameObject.SetActive(false);
        canvasMenu.transform.Find("OptionsMenu").gameObject.SetActive(true);
    }

    public void ClickBackFromOptions()
    {
        canvasMenu.transform.Find("Menu").gameObject.SetActive(true);
        canvasMenu.transform.Find("OptionsMenu").gameObject.SetActive(false);
    }

    public void ChangedCrosshairSizeInput() => UpdateNewCrosshairSize(float.Parse(crosshairSizeInputField.text));

    public void ChangedCrosshairSizeSlider() => UpdateNewCrosshairSize(crosshairSizeSlider.value);
    private void UpdateCrosshairSizeInputFieldValue(string value) => crosshairSizeInputField.text = value;
    private void UpdateCrosshairSizeSliderValue(float value) => crosshairSizeSlider.value = value;
    private void UpdateNewCrosshairSize(float value)
    {
        value = Mathf.Max(value, crosshairSizeMin);
        value = Mathf.Min(value, crosshairSizeMax);
        value = Mathf.Round(value);
        UpdateCrosshairSizeInputFieldValue($"{value:F0}");
        UpdateCrosshairSizeSliderValue(value);
        _crosshairSize = value;
        UpdateCrosshairSize();
    }
    
    public void ChangedSenseInput() => UpdateNewSense(float.Parse(sensitivityInputField.text));
    public void ChangedSenseSlider() => UpdateNewSense(sensitivitySlider.value);
    private void UpdateSensitivityInputFieldValue(string value) => sensitivityInputField.text = value;
    private void UpdateSensitivitySliderValue(float value) => sensitivitySlider.value = value;

    private void UpdateNewSense(float value)
    {
        value = Mathf.Max(value, sensitivityMin);
        value = Mathf.Min(value, sensitivityMax);
        UpdateSensitivityInputFieldValue($"{value:F2}");
        UpdateSensitivitySliderValue(value);
        _playerCameraSense = value;
        playerControl.UpdateCamSensitivity();
    }

}

[Serializable]
public class GameplayData
{
    public float[] values;
}
