using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartMenuNavigator : MonoBehaviour
{
    public GameObject RestartMenuCanvas;

    void Start()
    {
        GameManager.Instance.OnLost += Activate;
    }

    private void Activate()
    {
        RestartMenuCanvas.SetActive(true);
    }

    public void LoadGame()
    {
        RestartMenuCanvas.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1, UnityEngine.SceneManagement.LoadSceneMode.Single);
        GameManager.Instance.UpdateGameState(GameState.Playing);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    void OnDestroy()
    {
        GameManager.Instance.OnLost -= Activate;
    }
}
