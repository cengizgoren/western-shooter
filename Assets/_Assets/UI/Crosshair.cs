using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Crosshair : MonoBehaviour
{
    InputActions inputActions;

    private void Awake() 
    {
        inputActions = new InputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        Cursor.visible = !hasFocus;
    }

    private void Start() 
    {
        inputActions.Player.Look.performed += ctx =>
        {
            OnLook(ctx);
        };
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 mousePos =  context.ReadValue<Vector2>();
        transform.position = new Vector3(mousePos.x, mousePos.y, 0);
    }
}
