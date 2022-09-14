using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuNavigator : MonoBehaviour
{
    public GameObject PauseMenuCanvas;

    void Start()
    {
        GameManager.Instance.OnPause += Show;
        GameManager.Instance.OnUnpause += Hide;
        GameManager.Instance.OnRestart += Hide;
    }

    private void Show()
    {
        PauseMenuCanvas.SetActive(true);
    }

    private void Hide()
    {
        PauseMenuCanvas.SetActive(false);
    }

    public void ContinuePressed()
    {
        GameManager.Instance.Unpause();
    }

    public void RestartPressed()
    {
        GameManager.Instance.Restart();
    }

    public void MainMenuPressed()
    {
        GameManager.Instance.MainMenu();
    }

    public void QuitPressed()
    {
        GameManager.Instance.Quit();
    }

    void OnDestroy()
    {
        GameManager.Instance.OnPause -= Show;
        GameManager.Instance.OnUnpause -= Hide;
        GameManager.Instance.OnRestart -= Hide;
    }
}
