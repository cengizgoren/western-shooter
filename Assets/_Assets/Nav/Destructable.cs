using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Destructable : MonoBehaviour
{
    [SerializeField] private float MaxHealthPoints = 100f;
    // [SerializeField] private AnimationCurve colorPhaseCurve;
    
    private float currentHealthPoints;

    private void Start()
    {
        currentHealthPoints = MaxHealthPoints;
    }

    public void Damage(int amount)
    {
        currentHealthPoints -= amount;
        if (currentHealthPoints <= 0) 
        {
            Destroy(gameObject);
        }

        Renderer renderer = GetComponent<Renderer>();
        if (renderer) 
        {
            float colorPhaseFactor = 1 - currentHealthPoints / MaxHealthPoints;
            foreach (Material mat in renderer.materials)
            {
                // mat.color = Color.Lerp(mat.color, Color.red, colorPhaseCurve.Evaluate(colorPhaseFactor));
                mat.color = Color.Lerp(mat.color, Color.red, colorPhaseFactor);
            }
        }
    }
}
