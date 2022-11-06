using DG.Tweening;
using FMODUnity;
using System;
using TMPro;
using UnityEngine;

public class MenuNavigator : MonoBehaviour
{
    [Range(0f, 1f)]
    public float PauseMenuFadeTime;

    [Range(0f, 1f)]
    public float GameEndMenuFadeTime;

    public GameObject MainMenuCanvas;
    public GameObject LevelSelectionCanvas;
    public GameObject PauseMenuCanvas;
    public GameObject OptionsCanvas;
    public GameObject GameEndedMenuCanvas;
    public TextMeshProUGUI EndStateLabel;
    [Header("Sounds")]
    public EventReference Click;
    public EventReference LesserClick;
    public EventReference BeginGame;
    public EventReference MenuUp;
    public EventReference MenuAway;

    private CanvasGroup PauseMenuCanvasGroup;
    private CanvasGroup GameEndedCanvasGroup;
    private Tween PauseTween;
    private Tween GameEndTween;

    private void Awake()
    {
        PauseMenuCanvasGroup = PauseMenuCanvas.GetComponent<CanvasGroup>();
        GameEndedCanvasGroup = GameEndedMenuCanvas.GetComponent<CanvasGroup>();

        PauseMenuCanvasGroup.alpha = 0f;
        GameEndedCanvasGroup.alpha = 0f;
    }
    private void Start()
    {
        GameManager.Instance.menuNavigator = this;
        GameManager.Instance.OnRestart += HideGameEndedMenu;
    }

    public void FadeInToPause(Action resume)
    {
        if (PauseTween.IsActive())
        {
            PauseTween.Flip();
        }
        else
        {
            PauseTween = DOTween.To(() => PauseMenuCanvasGroup.alpha, x => PauseMenuCanvasGroup.alpha = x, 1f, PauseMenuFadeTime)
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .OnStart(() => PauseMenuCanvas.SetActive(true))
                .OnRewind(() =>
                {
                    resume(); 
                    PauseTween.Kill();
                });
        }
    }

    public void FadeOutToGameplay(Action resume)
    {
        if (PauseTween.IsActive())
        {
            PauseTween.Flip();
        }
        else
        {
            PauseTween = DOTween.To(() => PauseMenuCanvasGroup.alpha, x => PauseMenuCanvasGroup.alpha = x, 0f, PauseMenuFadeTime)
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .OnRewind(() =>
                {
                    PauseTween.Kill();
                })
                .OnComplete(() =>
                {
                    PauseMenuCanvas.SetActive(false);
                    resume();
                });
        }
    }

    public void FadeInToGameEnd()
    {
        GameEndTween = DOTween.To(() => GameEndedCanvasGroup.alpha, x => GameEndedCanvasGroup.alpha = x, 1f, GameEndMenuFadeTime)
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .OnStart(() => GameEndedMenuCanvas.SetActive(true));
    }

    public void LevelSelectionPressed(int level)
    {
        RuntimeManager.PlayOneShot(BeginGame);
        GameManager.Instance.LoadLevel(level);
        gameObject.SetActive(false);
    }

    public void PlayPressed()
    {
        RuntimeManager.PlayOneShot(Click);
        MainMenuCanvas.SetActive(false);
        LevelSelectionCanvas.SetActive(true);
    }

    public void PlayBackPressed()
    {
        RuntimeManager.PlayOneShot(Click);
        LevelSelectionCanvas.SetActive(false);
        MainMenuCanvas.SetActive(true);
    }

    public void RestartPressed()
    {
        RuntimeManager.PlayOneShot(Click);
        GameManager.Instance.RestartLevel();
        gameObject.SetActive(false);
    }

    public void ContinuePressed()
    {
        RuntimeManager.PlayOneShot(Click);
        GameManager.Instance.UnpauseGame();
    }

    public void OptionsPressed()
    {
        RuntimeManager.PlayOneShot(Click);
        MainMenuCanvas.SetActive(false);
        OptionsCanvas.SetActive(true);
    }

    public void OptionsBackPressed()
    {
        RuntimeManager.PlayOneShot(Click);
        OptionsCanvas.SetActive(false);
        MainMenuCanvas.SetActive(true);
    }

    public void MainMenuPressed()
    {
        RuntimeManager.PlayOneShot(Click);
        GameManager.Instance.LoadMainMenu();
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
        if (PauseMenuCanvas)
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

    void OnDestroy()
    {
        GameManager.Instance.menuNavigator = null;
        GameManager.Instance.OnRestart -= HideGameEndedMenu;
    }
}
