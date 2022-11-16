using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerArrow : MonoBehaviour
{
    [SerializeField] private Transform DirectionArrow;

    private PlayerInput input;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        DirectionArrow.SetPositionAndRotation(new Vector3(input.MouseWorldPosition.x, DirectionArrow.position.y, input.MouseWorldPosition.z), Quaternion.LookRotation(input.DirectionToMouse));
    }
}
