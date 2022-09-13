using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Destructable : MonoBehaviour, IDamagable
{
    [SerializeField] private int MaxHealthPoints = 100;
    // [SerializeField] private AnimationCurve colorPhaseCurve;

    [Space(10)]
    [SerializeField] private GameObject DeathVfx;
    [SerializeField] private float DeathVfxSpawnOffset = 0f;
    [SerializeField] private float DeathVfxLifetime = 5f;
    // Is this setting meaningful? If so which rotation to choose?
    [SerializeField] private Quaternion DeathVfxRotation;

    [SerializeField] private int currentHealthPoints;

    private Renderer _renderer;
    private Dictionary<int, Color> originalColors;

    public UnityAction OnHealthDepleted;

    private void Start()
    {
        currentHealthPoints = MaxHealthPoints;
        _renderer = GetComponent<Renderer>();

        if (_renderer) 
        {
            originalColors = new Dictionary<int, Color>();
            foreach (Material mat in _renderer.materials)
            {
                originalColors[mat.GetInstanceID()] = mat.color;
            }
        }
    }

    public int GetCurrentHP()
    {
        return currentHealthPoints;
    }

    public void DealDamage(int amount)
    {
        currentHealthPoints -= amount;
        if (currentHealthPoints <= 0) 
        {
            OnHealthDepleted?.Invoke();
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

        if (_renderer) 
        {
            float colorPhaseFactor = 1f - ((float)currentHealthPoints / (float)MaxHealthPoints);
            foreach (Material mat in _renderer.materials)
            {
                // TODO: test color change curve
                // mat.color = Color.Lerp(mat.color, Color.red, colorPhaseCurve.Evaluate(colorPhaseFactor));
                mat.color = Color.Lerp(originalColors[mat.GetInstanceID()], Color.red, colorPhaseFactor);
            }
        }
    }


}
