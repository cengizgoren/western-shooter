using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Help : MonoBehaviour
{
    public RectTransform HelpPopupText;

    private bool isAway = false;

    private void Start()
    {
        Controls.InputActions.Player.Help.performed += ToggleHelp;
    }

    private void OnDestroy()
    {
        Controls.InputActions.Player.Help.performed -= ToggleHelp;
    }

    private void ToggleHelp(InputAction.CallbackContext context)
    {
        if (DOTween.IsTweening(HelpPopupText, true))
        {
            HelpPopupText.DOFlip();
        }
        else
        {
            if (isAway)
                DoSlideIn();
            else
                DoSlideOut();
        }
    }

    private void DoSlideOut()
    {
        HelpPopupText.DOAnchorPosX(HelpPopupText.rect.width, 1f)
            .SetEase(Ease.InCubic)
            .OnComplete(() => isAway = true);
    }

    private void DoSlideIn()
    {
        HelpPopupText.DOAnchorPosX(0f, 1f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => isAway = false);
    }
}