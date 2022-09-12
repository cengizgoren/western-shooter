using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State;

    public UnityAction OnLost;


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

    // Awful...
    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (State)
        {
            case GameState.MainMenu:
                break;
            case GameState.Playing:
                if (GameObject.FindGameObjectWithTag("Player"))
                {
                    GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().OnHealthDepleted += PlayerLost;
                }
                Controls.InputActions.Player.Enable();
                break;
            case GameState.Paused:
                break;
            case GameState.Victory:
                break;
            case GameState.Lost:
                if (GameObject.FindGameObjectWithTag("Player"))
                {
                    GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().OnHealthDepleted -= PlayerLost;
                }
                Controls.InputActions.Player.Disable();
                break;
            default:
                break;
        }
    }

    private void PlayerLost()
    {
        UpdateGameState(GameState.Lost);
        OnLost?.Invoke();
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
