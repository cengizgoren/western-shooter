using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveUI : MonoBehaviour
{
    public TextMeshProUGUI ObjectiveText;
    public TextMeshProUGUI ObjectivesCurrent;
    public TextMeshProUGUI ObjectivesDivider;
    public TextMeshProUGUI ObjectivesTotal;
    public UIColorScheme ColorScheme;

    private Image background;

    private void Start()
    {
        if (TryGetComponent(out Image image))
        {
            background = image;
            background.color = ColorScheme.ObjectivesBackground;
        }
    }

    public void Setup(string objectiveText, string current, string divider, string total)
    {
        ObjectiveText.SetText(objectiveText);
        ObjectivesCurrent.SetText(current);
        ObjectivesDivider.SetText(divider);
        ObjectivesTotal.SetText(total);
    }

    public void MarkAsCompleted()
    {
        if (TryGetComponent(out Image background))
        {
            background.color = ColorScheme.ObjectivesBackgroundComplete;
        }
    }
}
