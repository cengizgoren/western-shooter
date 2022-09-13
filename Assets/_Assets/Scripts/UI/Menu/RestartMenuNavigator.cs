using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RestartMenuNavigator : MonoBehaviour
{
    public GameObject RestartMenuCanvas;
    public TextMeshProUGUI EndStateLabel;


    void Start()
    {
        GameManager.Instance.OnLost += ShowLose;
        GameManager.Instance.OnWon += ShowWin;
        GameManager.Instance.OnRestart += Hide;
    }

    private void ShowWin()
    {
        EndStateLabel.SetText("Victory!");
        RestartMenuCanvas.SetActive(true);
    }

    private void ShowLose()
    {
        EndStateLabel.SetText("Lost!");
        RestartMenuCanvas.SetActive(true);
    }

    private void Hide()
    {
        RestartMenuCanvas.SetActive(false);
    }

    public void RestartPressed()
    {
        GameManager.Instance.Restart();
    }

    public void QuitPressed()
    {
        GameManager.Instance.Quit();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnLost -= ShowLose;
        GameManager.Instance.OnWon -= ShowWin;
        GameManager.Instance.OnRestart -= Hide;
    }
}
