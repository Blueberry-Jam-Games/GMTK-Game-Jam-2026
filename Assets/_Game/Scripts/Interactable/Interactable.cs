using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public event Action OnInteraction;

    public bool Highlighted = false;
    [SerializeField] private bool ActualState;

    private Renderer[] childRenderers;

    private void Start ()
    {
        childRenderers = gameObject.GetComponentsInChildren<Renderer>();

        foreach(Renderer renderer in childRenderers)
        {
            Debug.Log($"Found renderer on {renderer.gameObject.name}");
            renderer.sharedMaterial = new Material(renderer.sharedMaterial);
            renderer.sharedMaterial.SetFloat("_InteractionEnabled", 1);
        }
    }

    private void LateUpdate()
    {
        ActualState = Highlighted;

        for (int i = 0; i < childRenderers.Length; i++)
        {
            childRenderers[i].sharedMaterial.SetInt("_Selected", Highlighted ? 1 : 0);
        }
        Highlighted = false;
    }

    public void Interact()
    {
        Debug.Log($"Interacted {name}");
        OnInteraction?.Invoke();
    }
}
