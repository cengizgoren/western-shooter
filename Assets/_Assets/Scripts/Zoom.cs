using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;


public class Zoom : MonoBehaviour
{
    public float MinOrthoSize = 1f;
    public float MaxOrthoSize = 100f;
    [Range(0f, 10f)]
    public float OrthoSizeDelta = 1f;

    private CinemachineVirtualCamera[] vcams;
    
    private void Awake()
    {
        vcams = FindObjectsOfType<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        Controls.InputActions.Player.Zoom.performed += ctx => ChangeZoom(ctx.ReadValue<float>());
        Controls.InputActions.Player.SwitchCamera.performed += _ => SwitchCameraTarget();
    }

    private void ChangeZoom(float y)
    {
        if (y >= 1)
        {
            foreach(var vcam in vcams)
            {
                vcam.m_Lens.OrthographicSize = Mathf.Clamp(vcam.m_Lens.OrthographicSize - OrthoSizeDelta, MinOrthoSize, MaxOrthoSize);
            }
        }
        else
        {
            foreach (var vcam in vcams)
            {
                vcam.m_Lens.OrthographicSize = Mathf.Clamp(vcam.m_Lens.OrthographicSize + OrthoSizeDelta, MinOrthoSize, MaxOrthoSize);
            }
        }
    }

    private void SwitchCameraTarget()
    {
        int temp = vcams[0].Priority;
        vcams[0].Priority = vcams[1].Priority;
        vcams[1].Priority = temp;
    }
}
