using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessing : MonoBehaviour
{
    public Volume volume;

    [Header("Vignette")]
    [Range(0f, 1f)] public float VignettePeakIntensity;
    [Range(0f, 2f)] public float VignetteAttackTime;

    public Color VignetteColor;

    private Vignette vignette;
    private PlayerHealth playerHP;

    private Tween attack;

    private void Start()
    {
        volume.profile.TryGet<Vignette>(out vignette);

        if (vignette)
        {
            vignette.intensity.value = 0f;
            vignette.color.value = VignetteColor;

            attack = DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, VignettePeakIntensity, VignetteAttackTime)
                .SetEase(Ease.InOutCubic)
                .SetAutoKill(false)
                .Pause();
            attack.OnComplete(() => attack.SmoothRewind());
        }
        else
        {
            Debug.LogErrorFormat("{0} - Vigniette not found", gameObject.name);
        }

        playerHP = FindObjectOfType<PlayerHealth>();

        if (playerHP)
        {
            playerHP.OnHpLost += HurtVignettePulse;
        }
        else
        {
            Debug.LogErrorFormat("{0} - PlayerHealth not found", gameObject.name);
        }
    }

    private void HurtVignettePulse()
    {
        if (attack.IsPlaying())
        {
            if (attack.IsBackwards())
            {
                attack.Flip();
            }
        }
        else
        {
            attack.Restart();
        }
    }
    private void OnDestroy()
    {
        if (playerHP)
            playerHP.OnHpLost -= HurtVignettePulse;

        attack.Kill();
    }
}
