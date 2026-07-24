using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

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

        parent.AddDestination(floorMeaning, requestedDirection);
        buttonToggle.interactable = false;
    }

    public void Initialize(Elevator parent, int floor)
    {
        floorMeaning = floor;
        floorText.text = floor.ToString();
        this.parent = parent;
    }

    public void Reset()
    {
        buttonToggle.interactable = true;
        buttonToggle.SetIsOnWithoutNotify(false);
    }
}