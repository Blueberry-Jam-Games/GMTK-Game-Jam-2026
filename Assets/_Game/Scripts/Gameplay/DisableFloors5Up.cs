using UnityEngine;

public class DisableFloors5Up : MonoBehaviour
{
    public Speakable speakable;
    void Start()
    {
        speakable.OnInteractionEnd += DisableFloors;
    }

    public void DisableFloors()
    {
        ElevatorManager e = ElevatorManager.Instance;
        e.disabledFloors[9] = true;
        e.disabledFloors[8] = true;
        e.disabledFloors[7] = true;
        e.disabledFloors[6] = true;
        e.disabledFloors[5] = true;

        GameplayManager.Instance.MarkFound("Return Keycard");
    }
}
