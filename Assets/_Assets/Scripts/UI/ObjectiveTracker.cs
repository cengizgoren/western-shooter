using FMODUnity;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveTracker : MonoBehaviour
{
    public UIColorScheme ColorScheme;
    public TextMeshProUGUI ObjectiveText;
    public RectTransform ObjectivePrefab;
    public EventReference ObjectiveComplete;

    List<RectTransform> objectives = new List<RectTransform>();
    private VictoryConditions victoryConditions;

    private void Start()
    {
        victoryConditions = FindObjectOfType<VictoryConditions>();
        AddKillObjective();
        victoryConditions.OnTargetKilled += UpdateKillObjective;
        victoryConditions.OnObjectiveComplete += CompleteKillObjective;
    }

    public void AddKillObjective()
    {
        RectTransform objective = Instantiate(ObjectivePrefab, gameObject.transform);

        if (objective.TryGetComponent(out Image background))
        {
            background.color = ColorScheme.ObjectivesBackground;
        }

        TextMeshProUGUI text = objective.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.SetText(new StringBuilder("Eliminate targets ")
            .Append("0/")
            .Append(victoryConditions.KillTargetsTotal)
            .ToString());
        objectives.Add(objective);
    }

    public void UpdateKillObjective()
    {
        TextMeshProUGUI text = objectives[0].GetChild(0).GetComponent<TextMeshProUGUI>();
        text.SetText(new StringBuilder("Eliminate targets ")
            .Append(victoryConditions.KillTargetsTotal - victoryConditions.KillTargetsCurrent)
            .Append("/")
            .Append(victoryConditions.KillTargetsTotal)
            .ToString());
    }

    public void CompleteKillObjective()
    {
        var objective = objectives[0];
        RuntimeManager.PlayOneShot(ObjectiveComplete);
        if (objective.TryGetComponent(out Image background))
        {
            background.color = ColorScheme.ObjectivesBackgroundComplete;
        }
    }
}
