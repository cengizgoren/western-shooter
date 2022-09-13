using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState GameState = GameState.MainMenu;
    public GameState PrevGameState = GameState.MainMenu;
    
    public VictoryState VictoryState;
    public VictoryState PrevVictoryState;


    public UnityAction OnLaunch;
    public UnityAction OnLost;
    public UnityAction OnWon;
    public UnityAction OnPause;
    public UnityAction OnUnpause;
    public UnityAction OnRestart;

    //private readonly Dictionary<Tuple<GameState, GameState>, Action> gameTransitions = new Dictionary<Tuple<GameState, GameState>, Action>();
    //private readonly Dictionary<Tuple<VictoryState, VictoryState>, Action> victoryTransitions = new Dictionary<Tuple<VictoryState, VictoryState>, Action>();

    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            //gameTransitions.Add(Tuple.Create(GameState.Launch, GameState.MainMenu), () => Debug.Log("Launch"));
            //gameTransitions.Add(Tuple.Create(GameState.InProgress, GameState.Paused), Pause);
            //gameTransitions.Add(Tuple.Create(GameState.Paused, GameState.InProgress), Unpause);
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
        UpdateGameState(GameState.Paused);
        OnPause?.Invoke();
        Controls.InputActions.Player.Disable();
        Time.timeScale = 0f;
    }

    public void Unpause()
    {
        UpdateGameState(GameState.Active);
        OnUnpause?.Invoke();
        Controls.InputActions.Player.Enable();
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        Controls.InputActions.Player.Enable();
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(1, UnityEngine.SceneManagement.LoadSceneMode.Single);
        UpdateGameState(GameState.Active);
        OnRestart?.Invoke();
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

    public void UpdateGameState(GameState newState)
    {
        PrevGameState = GameState;
        GameState = newState;
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
                OnWon?.Invoke();
                Controls.InputActions.Player.Disable();
                Time.timeScale = 0f;
                UpdateGameState(GameState.Ended);
                break;
            case VictoryState.Lost:
                OnLost?.Invoke();
                Controls.InputActions.Player.Disable();
                Time.timeScale = 0f;
                UpdateGameState(GameState.Ended);
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
