using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuNavigator : MonoBehaviour
{
    public GameObject PauseMenuCanvas;
    public GameObject GameEndedMenuCanvas;
    public TextMeshProUGUI EndStateLabel;

    public void LevelSelectionPressed(int level)
    {
        GameManager.Instance.LoadLevel(level);
    }

    public void RestartPressed()
    {
        GameManager.Instance.Restart();
        gameObject.SetActive(false);
    }

    public void ContinuePressed()
    {
        GameManager.Instance.Unpause();
    }

    public void MainMenuPressed()
    {
        GameManager.Instance.MainMenu();
    }

    public void QuitPressed()
    {
        GameManager.Instance.Quit();
    }

    private void ShowGameEndedVictoryScreen()
    {
        EndStateLabel.SetText("Victory");
        GameEndedMenuCanvas.SetActive(true);
    }

    private void ShowGameEndedFailureScreen()
    {
        EndStateLabel.SetText("Lost");
        GameEndedMenuCanvas.SetActive(true);
    }

    private void HideGameEndedMenu()
    {
        GameEndedMenuCanvas.SetActive(false);
    }

    private void ShowPauseMenu()
    {
        if(PauseMenuCanvas) 
            PauseMenuCanvas.SetActive(true);
    }

    private void HidePauseMenu()
    {
        if (PauseMenuCanvas)
            PauseMenuCanvas.SetActive(false);
    }

    void Start()
    {
        GameManager.Instance.OnPause += ShowPauseMenu;
        GameManager.Instance.OnUnpause += HidePauseMenu;
        GameManager.Instance.OnRestart += HideGameEndedMenu;
        GameManager.Instance.OnLost += ShowGameEndedFailureScreen;
        GameManager.Instance.OnWon += ShowGameEndedVictoryScreen;
    }

    void OnDestroy()
    {
        GameManager.Instance.OnPause -= ShowPauseMenu;
        GameManager.Instance.OnUnpause -= HidePauseMenu;
        GameManager.Instance.OnRestart -= HideGameEndedMenu;
        GameManager.Instance.OnLost -= ShowGameEndedFailureScreen;
        GameManager.Instance.OnWon -= ShowGameEndedVictoryScreen;
    }
}
