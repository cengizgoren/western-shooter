using System.Linq;
using UnityEngine;

public class HideLevelBounds : MonoBehaviour
{
    public MeshRenderer[] meshRenderers;

    private void Awake()
    {
        GetComponentsInChildren<MeshRenderer>().ToList().ForEach(meshRenderer => meshRenderer.enabled = false);
    }
}
