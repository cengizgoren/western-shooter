using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Destructable : MonoBehaviour
{
    [SerializeField] private float MaxHealthPoints = 100f;
    // [SerializeField] private AnimationCurve colorPhaseCurve;

    [Space(10)]
    [SerializeField] private GameObject DeathVfx;
    [SerializeField] private float DeathVfxSpawnOffset = 0f;
    [SerializeField] private float DeathVfxLifetime = 5f;
    // Is this setting meaningful? If so which rotation to choose?
    [SerializeField] private Quaternion DeathVfxRotation;

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
            if (DeathVfx)
            {
                GameObject impactVfxInstance = Instantiate(DeathVfx, transform.position + (transform.forward * DeathVfxSpawnOffset), DeathVfxRotation);
                if (DeathVfxLifetime > 0)
                {
                    Destroy(impactVfxInstance, DeathVfxLifetime);
                }
            }
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
