using FMODUnity;
using System.Collections.Generic;
using UnityEngine;


public class Destructable : Health
{
    [SerializeField] private string ColorPropertyName = "_BaseColor";
    [SerializeField] private Color LowHealthColor = Color.red;

    private Renderer _renderer;
    private Dictionary<int, Color> originalColors;

    private void Start()
    {
        if (TryGetComponent<Renderer>(out _renderer))
        {
            originalColors = new Dictionary<int, Color>();
            foreach (Material mat in _renderer.materials)
            {
                originalColors[mat.GetInstanceID()] = mat.GetColor(ColorPropertyName);
            }
        }
        else
        {
            Debug.LogWarningFormat("{} - Destrucatble object could not get a renderer component");
        }
    }

    public override void Damage(int amount)
    {
        base.Damage(amount);
        if (_renderer)
        {
            float colorPhaseFactor = 1f - ((float)HP / (float)MaxHP);
            foreach (Material mat in _renderer.materials)
            {
                // TODO: test color change curve
                // mat.color = Color.Lerp(mat.color, Color.red, colorPhaseCurve.Evaluate(colorPhaseFactor));
                mat.SetColor(ColorPropertyName, Color.Lerp(originalColors[mat.GetInstanceID()], LowHealthColor, colorPhaseFactor));
            }
        }
    }
}
