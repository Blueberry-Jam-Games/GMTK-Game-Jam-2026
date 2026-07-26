using UnityEngine;

public class TimeStart : MonoBehaviour
{
    [SerializeField] private GameObject blockerRoot;

    bool talked = false;

    private void Start()
    {
        GetComponent<Speakable>().OnInteractionEnd += CEOConversationEnded;
    }

    private void CEOConversationEnded()
    {
        if (!talked)
        {
            talked = true;
            blockerRoot.SetActive(false);
            GameObject timer = GameObject.FindWithTag("Clock");
            TimeScript ts = timer.GetComponent<TimeScript>();

            GameplayManager.Instance.removeBarriers = true;

            ts.StartTime();
        }        
    }
}
