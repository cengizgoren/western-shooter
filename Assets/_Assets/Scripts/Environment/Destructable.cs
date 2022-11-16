using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshObstacle))]
public class Destructable : MonoBehaviour
{
    public UnityAction OnHealthDepleted;

    public DestructionEffect DestructionEffect;

    public MaxHP MaxHP;
    public bool GodMode = true;
    public int currentHealthPoints;

    private Renderer _renderer;
    private Dictionary<int, Color> originalColors;
    private int maxHealthPoints;

    private void Start()
    {
        maxHealthPoints = MaxHP.Value;
        currentHealthPoints = MaxHP.Value;
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

    public  int GetCurrentHP()
    {
        return currentHealthPoints;
    }

    public  void DealDamage(int amount)
    {
        if (!GodMode)
        {
            currentHealthPoints -= amount;
            if (currentHealthPoints <= 0)
            {
                OnHealthDepleted?.Invoke();
                if (DestructionEffect)
                {
                    Instantiate(DestructionEffect.DestructionVFX, transform.position, transform.rotation);
                    RuntimeManager.PlayOneShot(DestructionEffect.DestructionSFX, transform.position);
                }
                Destroy(gameObject);
            }

            if (_renderer)
            {
                float colorPhaseFactor = 1f - ((float)currentHealthPoints / (float)maxHealthPoints);
                foreach (Material mat in _renderer.materials)
                {
                    // TODO: test color change curve
                    // mat.color = Color.Lerp(mat.color, Color.red, colorPhaseCurve.Evaluate(colorPhaseFactor));
                    mat.color = Color.Lerp(originalColors[mat.GetInstanceID()], Color.red, colorPhaseFactor);
                }
            }
        }
    }
}
