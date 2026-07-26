using UnityEngine;

public class TimeStart : MonoBehaviour
{
    bool converse;

    private void Start()
    {
        GetComponent<Speakable>().OnInteractionEnd += CEOConversationEnded;
    }

    private void CEOConversationEnded()
    {
        
    }
}
