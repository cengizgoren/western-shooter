using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuNavigator : MonoBehaviour
{
    public void LoadGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1, UnityEngine.SceneManagement.LoadSceneMode.Single);
        GameManager.Instance.UpdateGameState(GameState.Playing);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
