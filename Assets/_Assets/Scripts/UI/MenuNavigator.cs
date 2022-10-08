using FMODUnity;
using TMPro;
using UnityEngine;

public class MenuNavigator : MonoBehaviour
{
    public GameObject PauseMenuCanvas;
    public GameObject GameEndedMenuCanvas;
    public TextMeshProUGUI EndStateLabel;
    [Header("Sounds")]
    public EventReference Click;
    public EventReference LesserClick;
    public EventReference BeginGame;
    public EventReference MenuUp;
    public EventReference MenuAway;

    public void LevelSelectionPressed(int level)
    {
        RuntimeManager.PlayOneShot(BeginGame);
        GameManager.Instance.LoadLevel(level);
        gameObject.SetActive(false);
    }

    public void RestartPressed()
    {
        RuntimeManager.PlayOneShot(Click);
        GameManager.Instance.Restart();
        gameObject.SetActive(false);
    }

    public void ContinuePressed()
    {
        RuntimeManager.PlayOneShot(Click);
        GameManager.Instance.Unpause();
    }

    public void MainMenuPressed()
    {
        RuntimeManager.PlayOneShot(Click);
        GameManager.Instance.MainMenu();
    }

    public void QuitPressed()
    {
        RuntimeManager.PlayOneShot(Click);
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
        {
            RuntimeManager.PlayOneShot(MenuUp);
            PauseMenuCanvas.SetActive(true);
        }
    }

    private void HidePauseMenu()
    {
        if (PauseMenuCanvas)
        {
            RuntimeManager.PlayOneShot(MenuAway);
            PauseMenuCanvas.SetActive(false);
        }
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
