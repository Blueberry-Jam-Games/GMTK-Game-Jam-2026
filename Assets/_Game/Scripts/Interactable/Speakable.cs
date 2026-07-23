using System;
using UnityEngine;

public class Speakable : MonoBehaviour
{
    public event Action OnInteractionEnd;
    public bool speechBubble = false;
    public DialogueSO dialogue;

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

    public DialogueSO Interact(out Speakable speakable)
    {
        Debug.Log($"Speak to {name}");
        speakable = this;
        return dialogue;
    }

    public void InteractionEnd()
    {
        OnInteractionEnd?.Invoke();
    }
}
