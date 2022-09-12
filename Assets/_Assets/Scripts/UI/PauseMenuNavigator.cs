using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuNavigator : MonoBehaviour
{
    public GameObject PauseMenuCanvas;

    private InputActions inputActions;

    void Awake()
    {
        inputActions = new InputActions();
    }

    void Start()
    {
        inputActions.Player.Escape.started += _ => OnEscape();
    }

    private void OnEnable() => inputActions.Enable();

    private void OnDisable() => inputActions.Disable();

    public void OnEscape()
    {
        if (GameManager.Instance.State == GameState.Paused)
        {
            Unpause();
        }
        else
        {
            Pause();
        }
    }

    private void Pause()
    {
        Time.timeScale = 0f;
        PauseMenuCanvas.SetActive(true);
        GameManager.Instance.UpdateGameState(GameState.Paused);
    }

    private void Unpause()
    {
        Time.timeScale = 1f;
        PauseMenuCanvas.SetActive(false);
        GameManager.Instance.UpdateGameState(GameState.Playing);
    }

    public void OnContinue() => Unpause();

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
