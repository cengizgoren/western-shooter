using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ObjectiveTracker : MonoBehaviour
{
    public UIColorScheme ColorScheme;
    public TextMeshProUGUI ObjectiveText;
    public RectTransform ObjectivePrefab;

    List<RectTransform> objectives = new List<RectTransform>();
    private VictoryConditions victoryConditions;

    private void Start()
    {
        victoryConditions = FindObjectOfType<VictoryConditions>();
        AddKillObjective();
        victoryConditions.OnTargetKilled += UpdateKillObjective;
    }

    public void AddKillObjective()
    {
        RectTransform objective = Instantiate(ObjectivePrefab, gameObject.transform);
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
}
