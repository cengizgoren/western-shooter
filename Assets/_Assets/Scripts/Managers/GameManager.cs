using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
            if (GameState == GameState.Active)
            {
                var p = FindObjectOfType<Player>();
                var c = p.GetComponent<PlayerHealth>();
                c.OnHpDepleted += () => UpdateVictoryCondition(VictoryState.Lost);
            }
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
        StartCoroutine(LoadSceneAsync(level));
    }

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
        var p = FindObjectOfType<Player>();
        var c = p.GetComponent<PlayerHealth>();
        c.OnHpDepleted += () => UpdateVictoryCondition(VictoryState.Lost);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        UnfreezeTime();
        UpdateGameState(GameState.Active);
    }

    public void Restart()
    {
        StartCoroutine(LoadSceneAsync(CurrentLevelID));
    }

    //private IEnumerator FadeThenDoAction(float seconds, Action action)
    //{
    //    var f = FindObjectOfType<Fader>();
    //    if (f)
    //    {
    //        f.FadeOut();
    //    }
    //    yield return new WaitForSeconds(seconds);
    //}

    //private IEnumerator WaitCoroutine(int seconds)
    //{
    //    Debug.Log("Started Coroutine at timestamp : " + Time.time);

    //    //yield on a new YieldInstruction that waits for 5 seconds.
    //    yield return new WaitForSeconds(seconds);

    //    //After we have waited 5 seconds print the time again.
    //    Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    //}

    public void MainMenu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
