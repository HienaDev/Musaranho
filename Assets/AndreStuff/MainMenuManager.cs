using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private void Start()
    {
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = false;
    }

    private bool _hasClickedSomething = false;
    
    public void ClickPlayButton()
    {
        if (_hasClickedSomething) return;
        _hasClickedSomething = true;
        SceneManager.LoadSceneAsync("SampleScene");
    }

    public void ClickQuitButton()
    {
        if (_hasClickedSomething) return;
        _hasClickedSomething = true;
        Application.Quit();
    }
    
}
