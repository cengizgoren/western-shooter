using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuNavigator : MonoBehaviour
{
    public void PlayPressed()
    {
        GameManager.Instance.Play();
    }

    public void QuitPressed()
    {
        GameManager.Instance.Quit();
    }
}
