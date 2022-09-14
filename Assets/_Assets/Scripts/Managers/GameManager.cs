using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState GameState = GameState.MainMenu;
    public VictoryState VictoryState;

    public UnityAction OnLaunch;
    public UnityAction OnLost;
    public UnityAction OnWon;
    public UnityAction OnPause;
    public UnityAction OnUnpause;
    public UnityAction OnRestart;

    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy() runs one frame after
            DestroyImmediate(gameObject);
        }
    }

    void Start()
    {
        Controls.InputActions.UI.Escape.started += _ => OnEscape();
    }

    public void Pause()
    {
        FreezeTime();
        UpdateGameState(GameState.Paused);
        OnPause?.Invoke();
    }

    public void Unpause()
    {
        UnfreezeTime();
        UpdateGameState(GameState.Active);
        OnUnpause?.Invoke();
    }

    public void Play()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1, UnityEngine.SceneManagement.LoadSceneMode.Single);
        UnfreezeTime();
        UpdateGameState(GameState.Active);
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1, UnityEngine.SceneManagement.LoadSceneMode.Single);
        UnfreezeTime();
        UpdateGameState(GameState.Active);
        OnRestart?.Invoke();
    }

    public void MainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0, UnityEngine.SceneManagement.LoadSceneMode.Single);
        UpdateGameState(GameState.MainMenu);
    }

    public void PlayerHasDied()
    {
        UpdateVictoryCondition(VictoryState.Lost);
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    private void FreezeTime()
    {
        Time.timeScale = 0f;
        Controls.InputActions.Player.Disable();
    }

    private void UnfreezeTime()
    {
        Time.timeScale = 1f;
        Controls.InputActions.Player.Enable();
    }

    public void UpdateGameState(GameState newState)
    {
        GameState = newState;
    }

    private void OnEscape()
    {
        if (GameState == GameState.Paused)
        {
            Unpause();
        }
        else if (GameState == GameState.Active)
        {
            Pause();
        }
    }

    public void UpdateVictoryCondition(VictoryState newState)
    {
        VictoryState = newState;

        switch (VictoryState)
        {
            case VictoryState.ObjectiveInProgress:
                break;
            case VictoryState.ObjectiveCompleted:
                break;
            case VictoryState.Won:
                FreezeTime();
                UpdateGameState(GameState.Ended);
                OnWon?.Invoke();
                break;
            case VictoryState.Lost:
                FreezeTime();
                UpdateGameState(GameState.Ended);
                OnLost?.Invoke();
                break;
            default:
                break;
        }
    }
}

public enum GameState
{
    MainMenu,
    Active,
    Paused,
    Ended,
}

public enum VictoryState
{
    ObjectiveInProgress,
    ObjectiveCompleted,
    Won,
    Lost
}
