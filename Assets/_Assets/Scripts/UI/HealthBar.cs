using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthBar;
    public IntVariable Variable;
    public IntVariable Max;

    private void Update()
    {
        healthBar.value = Variable.RuntimeValue;
        healthBar.maxValue = Max.RuntimeValue;
    }
}
