using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using System.Collections;

public class ElevatorButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI floorText;
    [SerializeField] private Toggle buttonToggle;

    private int floorMeaning = -1;
    public Elevator parent;

    public Elevator.ElevatorDirection requestedDirection = Elevator.ElevatorDirection.NEUTRAL;
    public bool isCallButton = false;

    private void Start()
    {
        buttonToggle.onValueChanged.AddListener(OnToggle);
    }

    private void OnToggle(bool toggle)
    {
        if (!toggle) return;

        if (isCallButton && ElevatorManager.Instance.TryGetFloorName(ElevatorManager.Instance.activeScene, out int currentFloor))
        {
            floorMeaning = currentFloor;
        }

        if(floorMeaning == -1)
        {
            Debug.LogError("Bad floor number");
            return;
        }

        Debug.Log($"Button {floorMeaning} pressed");

        if(parent.AddDestination(floorMeaning, requestedDirection) != Elevator.DestinationResult.SUCCESS)
        {
            StartCoroutine(DelayedButtonOff());
            return;
        }
        buttonToggle.interactable = false;
    }

    private IEnumerator DelayedButtonOff()
    {
        yield return new WaitForSeconds(0.5f);
        buttonToggle.SetIsOnWithoutNotify(false);
    }

    public void Initialize(Elevator parent, int floor)
    {
        floorMeaning = floor;
        int floorActual = floor + 1;
        if(floorActual > 1)
        {
            floorText.text = floorActual.ToString();
        }
        else
        {
            floorText.text = "G";
        }
        
        this.parent = parent;
    }

    public void Reset()
    {
        buttonToggle.interactable = true;
        buttonToggle.SetIsOnWithoutNotify(false);
    }
}