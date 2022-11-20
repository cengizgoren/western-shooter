using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class VignettePulse : MonoBehaviour
{
    [SerializeField] private Volume GlobalVolume;
    [SerializeField] private Health playerHP;
    [SerializeField] private Color VignetteColor;
    [SerializeField][Range(0f, 1f)] private float VignettePeakIntensity = 0.25f;
    [SerializeField][Range(0f, 2f)] private float VignetteAttackTime = 0.1f;

    private Vignette vignette;
    private Tween vignettePulse;

    private void Start()
    {
        GlobalVolume.profile.TryGet<Vignette>(out vignette);

        if (vignette)
        {
            vignette.intensity.value = 0f;
            vignette.color.value = VignetteColor;

            // Cannot easly change values from this premade tween, it works, but maybe I should make this tween at runtime
            vignettePulse = DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, VignettePeakIntensity, VignetteAttackTime)
                .SetEase(Ease.InOutCubic)
                .SetAutoKill(false)
                .Pause();
            vignettePulse.OnComplete(() => vignettePulse.SmoothRewind());
        }
        else
        {
            Debug.LogErrorFormat("{0} - Vigniette not found", gameObject.name);
        }

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
        if (vignettePulse.IsPlaying())
        {
            if (vignettePulse.IsBackwards())
            {
                vignettePulse.Flip();
            }
        }
        else
        {
            vignettePulse.Restart();
        }
    }

    private void OnDestroy()
    {
        if (playerHP)
            playerHP.OnHpLost -= HurtVignettePulse;

        vignettePulse.Kill();
    }
}
