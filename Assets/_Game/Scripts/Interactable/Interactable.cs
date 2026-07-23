using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public event Action OnInteraction;

    public bool Highlighted = false;
    [SerializeField] private bool ActualState;

    void LateUpdate()
    {
        ActualState = Highlighted;
        if(Highlighted)
        {
            // Visual Code Here
        }
        Highlighted = false;
    }

    public void Interact()
    {
        Debug.Log($"Interacted {name}");
        OnInteraction?.Invoke();
    }
}
