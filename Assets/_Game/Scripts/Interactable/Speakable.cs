using System;
using UnityEngine;

public class Speakable : MonoBehaviour
{
    public event Action OnInteraction;
    public bool speechBubble = false;

    [SerializeField] private bool ActualState;

    void LateUpdate()
    {
        ActualState = speechBubble;
        if(speechBubble)
        {
            // Visual Code Here
        }
        
        speechBubble = false;
    }

    public void Interact()
    {
        Debug.Log($"Speak to {name}");
    }
}
