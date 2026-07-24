using System;
using UnityEngine;

public class Speakable : MonoBehaviour
{
    public event Action OnInteractionEnd;
    public bool speechBubble = false;
    public DialogueSO dialogue;

    public SpriteRenderer commsIndicator;

    [SerializeField] private bool ActualState;

    void Start()
    {
        commsIndicator.enabled = false;
    }

    void LateUpdate()
    {
        ActualState = speechBubble;
        if(speechBubble)
        {
            commsIndicator.enabled = true;
        } else
        {
            commsIndicator.enabled = false;
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
