using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public event Action OnInteraction;

    public bool Highlighted = false;
    [SerializeField] private bool ActualState;

    private Renderer localRenderer;

    private void Start ()
    {
        localRenderer = GetComponent<Renderer>();
        localRenderer.sharedMaterial = new Material(localRenderer.sharedMaterial);
    }

    private void LateUpdate()
    {
        ActualState = Highlighted;
        if(Highlighted)
        {
            // Visual Code Here
            localRenderer.sharedMaterial.SetInt("_Selected", 1);
        }
        else
        {
            localRenderer.sharedMaterial.SetInt("_Selected", 0);
        }
        Highlighted = false;
    }

    public void Interact()
    {
        Debug.Log($"Interacted {name}");
        OnInteraction?.Invoke();
    }
}
