using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State;

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
        UpdateGameState(GameState.MainMenu);
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (State)
        {
            case GameState.MainMenu:
                break;
            case GameState.Playing:
                break;
            case GameState.Paused:
                break;
            case GameState.Victory:
                break;
            case GameState.Lost:
                break;
            default:
                break;
        }


    }
}

public enum GameState 
{
    MainMenu,
    Playing,
    Paused,
    Victory,
    Lost
}
