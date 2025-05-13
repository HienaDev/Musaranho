using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField] private PlayerControl playerControl;
    
    private bool _canPlayerMove = false;
    public bool CanPlayerMove() => _canPlayerMove;
    public void SetPlayerMove(bool value) => _canPlayerMove = value;

    private float _playerCameraSense = 2f;
    public float GetPlayerCameraSensitivity() => _playerCameraSense;

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
    }

    private void ClosePauseMenu()
    {
        _canPlayerMove = true;
        canvasMenu.gameObject.SetActive(!_canPlayerMove);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    /*
     * MENU OPTIONS STUFF
     */
    [SerializeField] private Transform canvasMenu;
    [Space][Header("Menu Options Stuff")]
    [SerializeField] private TMP_InputField sensitivityInputField;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private float minSensitivity = 0.01f;
    [SerializeField] private float maxSensitivity = 10f;

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
        UpdateInputFieldValue($"{value:F2}");
        UpdateSliderValue(value);
        _playerCameraSense = value;
        playerControl.UpdateCamSensitivity();
    }

}

[Serializable]
public class GameplayData
{
    public float[] values;
}
