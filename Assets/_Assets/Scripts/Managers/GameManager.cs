using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

public static class TimeFlow
{
    public static void FreezeTime()
    {
        Time.timeScale = 0f;
        Controls.InputActions.Player.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void UnfreezeTime()
    {
        Time.timeScale = 1f;
        Controls.InputActions.Player.Enable();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState GameState = GameState.MainMenu;
    public VictoryState VictoryState;
    public int CurrentLevelID;
    public MenuNavigator menuNavigator;

    public UnityAction OnFreeze;
    public UnityAction OnUnfreeze;
    public UnityAction OnRestart;

    private Player p;
    private PlayerHealth c;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            if (GameState == GameState.Active)
            {
                p = FindObjectOfType<Player>();
                c = p.GetComponent<PlayerHealth>();
                c.OnHpDepleted += Lose;
            }
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy() runs one frame after
            DestroyImmediate(gameObject);
        }
    }

    private void Start()
    {
        Controls.InputActions.UI.Escape.started += Escape;
    }

    //
    // Subscribed to events
    //

    private void Lose()
    {
        UpdateVictoryCondition(VictoryState.Lost);
    }

    private void Escape(InputAction.CallbackContext context)
    {
        if (GameState == GameState.Paused)
        {
            if (menuNavigator)
            {
                menuNavigator.FadeOutToGameplay(() => UpdateGameState(GameState.Active, false));
            }
            else
            {
                UpdateGameState(GameState.Active, false);
            }
        }
        else if (GameState == GameState.Active)
        {
            UpdateGameState(GameState.Paused, true);
            if (menuNavigator)
            {
                menuNavigator.FadeInToPause(() => UpdateGameState(GameState.Active, false));
            }
        }
    }

    //
    // Update states
    //

    public void UpdateGameState(GameState newState)
    {
        GameState = newState;
    }

    public void UpdateGameState(GameState newState, bool freezeTime)
    {
        GameState = newState;

        if (freezeTime)
        {
            TimeFlow.FreezeTime();
            OnFreeze?.Invoke();
        }
        else
        {
            TimeFlow.UnfreezeTime();
            OnUnfreeze?.Invoke();
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
                UpdateGameState(GameState.Ended, true);
                if (menuNavigator)
                    menuNavigator.FadeInToGameEnd();
                break;
            case VictoryState.Lost:
                UpdateGameState(GameState.Ended, true);
                if (menuNavigator)
                    menuNavigator.FadeInToGameEnd();
                break;
            default:
                break;
        }
    }

    //
    // Load a scene
    //

    private IEnumerator LoadSceneAsync(int level)
    {
        Fader fader = FindObjectOfType<Fader>();
        if (fader)
        {
            fader.FadeOut();
        }

        yield return new WaitForSecondsRealtime(1);

        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
            yield return null;

        yield return new WaitForEndOfFrame();

        CurrentLevelID = level;
        p = FindObjectOfType<Player>();
        c = p.GetComponent<PlayerHealth>();
        c.OnHpDepleted += Lose;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        TimeFlow.UnfreezeTime();
        UpdateGameState(GameState.Active);
    }

    //
    // Called in buttons
    //

    // Has duplicated code
    public void UnpauseGame()
    {
        if (menuNavigator)
        {
            menuNavigator.FadeOutToGameplay(() => UpdateGameState(GameState.Active, false));
        }
        else
        {
            UpdateGameState(GameState.Active, false);
        }
    }

    public void LoadLevel(int level)
    {
        StartCoroutine(LoadSceneAsync(level));
    }

    public void RestartLevel()
    {
        StartCoroutine(LoadSceneAsync(CurrentLevelID));
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UpdateGameState(GameState.MainMenu);
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    //
    // Unity specific
    //

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

    private void OnDestroy()
    {
        if (c != null)
        {
            c.OnHpDepleted -= Lose;
        }
        Controls.InputActions.UI.Escape.started -= Escape;
    }
}