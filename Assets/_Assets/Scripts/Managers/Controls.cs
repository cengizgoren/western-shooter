using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    public static InputActions InputActions;

    void Awake()
    {
        if (InputActions == null)
        {
            InputActions = new InputActions();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy() runs one frame after
            DestroyImmediate(gameObject);
        }
    }

    void OnEnable()
    {
        InputActions.Enable();
    }

    void OnDisable()
    {
        InputActions.Disable();
    }
}
