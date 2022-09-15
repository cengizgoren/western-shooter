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
    public int CurrentLevelID;

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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        OnPause?.Invoke();
    }

    public void Unpause()
    {
        UnfreezeTime();
        UpdateGameState(GameState.Active);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        OnUnpause?.Invoke();
    }

    public void LoadLevel(int level)
    {
        CurrentLevelID = level;
        UnityEngine.SceneManagement.SceneManager.LoadScene(level, UnityEngine.SceneManagement.LoadSceneMode.Single);
        UnfreezeTime();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        UpdateGameState(GameState.Active);
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(CurrentLevelID, UnityEngine.SceneManagement.LoadSceneMode.Single);
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void UnfreezeTime()
    {
        Time.timeScale = 1f;
        Controls.InputActions.Player.Enable();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
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

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            switch (GameState)
            {
                case GameState.MainMenu:
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
                case GameState.Active:
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = false;
                    break;
                case GameState.Paused:
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
                case GameState.Ended:
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
            }
        }
        if (!hasFocus)
        {
            //Pause();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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
