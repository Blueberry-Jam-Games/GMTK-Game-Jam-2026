using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using Unity.Mathematics;
using TMPro;

public class Elevator : MonoBehaviour
{
    public enum ElevatorDirection
    {
        UP,
        NEUTRAL,
        DOWN,
        UNCALLED
    }

    [Header("References")]
    public GameObject visibleLayer;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonRoot;
    [SerializeField] private Canvas buttonsCanvas;
    [SerializeField] private Animator animator;
    [SerializeField] private List<ElevatorButton> callButtons;
    [SerializeField] private BoxCollider occupationCollider;
    [SerializeField] private BoxCollider doorSensorCollider;
    [SerializeField] private LayerMask playerLayer;

    [Header("Display Icons")]
    [SerializeField] private List<TextMeshProUGUI> floorNumberDisplays;
    [SerializeField] private Image upArrowDisplay;
    [SerializeField] private Image downArrowDisplay;

    public Sprite arrowDownOff;
    public Sprite arrowDownOn;
    public Sprite ArrowUpOff;
    public Sprite ArrowUpOn;

    
    [Header("Timing")]
    [SerializeField] private float secondsPerFloor = 1f;
    [SerializeField] private float doorAnimationDuration = 1f;
    [SerializeField] private float doorDwellDuration = 2f;

    public int currentFloorIndex = 0;
    // private int calledToFloor = -1;
    private bool doorOpen = true;
    private readonly List<ElevatorButton> buttons = new();
    private Dictionary<int, ElevatorDirection> selectedFloors = new();
    public Dictionary<int, int> floorToButton = new();
    public List<int> floors;
    [SerializeField] private ElevatorDirection preferredDirection = ElevatorDirection.NEUTRAL;
    [SerializeField] private ElevatorDirection previousDirection = ElevatorDirection.NEUTRAL;

    private bool initialized = false;

    private void Awake ()
    {
        
    }

    private void Start ()
    {
        animator.SetBool("OpenDoors", true);
        StartCoroutine (DoElevator ());
    }

    public void Initialize (ElevatorEntry source)
    {
        floors = source.floors;
        selectedFloors.Clear();
        preferredDirection = ElevatorDirection.NEUTRAL;
        currentFloorIndex = 0;

        foreach (int floor in source.floors)
        {
            GameObject newButton = GameObject.Instantiate (buttonPrefab, buttonRoot);
            ElevatorButton eb = newButton.GetComponent<ElevatorButton> ();
            eb.Initialize (this, floor);

            buttons.Add (eb);
            floorToButton.Add(floor, buttons.Count - 1);
            selectedFloors.Add(floor, ElevatorDirection.UNCALLED);
        }

        foreach(ElevatorButton eb in callButtons)
        {
            eb.isCallButton = true;
        }

        buttonsCanvas.worldCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();

        string number = currentFloorIndex.ToString("D2");
        if(number == "00")
        {
            number = "G";
        }
        foreach(TextMeshProUGUI t in floorNumberDisplays)
        {
            t.text = number;
        }

        upArrowDisplay.sprite = ArrowUpOff;
        downArrowDisplay.sprite = arrowDownOff;

        initialized = true;
    }

    private IEnumerator DoElevator ()
    {
        yield return new WaitUntil(() => initialized);

        // The elevator begins with its doors open.
        yield return CloseDoor();

        while (true)
        {
            // Check current floor
            if(currentFloorIndex >= 0 && selectedFloors[currentFloorIndex] != ElevatorDirection.UNCALLED)
            {
                Debug.Log("Current Floor Called");
                yield return DoorSequence();
                continue;
            }

            if(preferredDirection != ElevatorDirection.UNCALLED)
            {
                int nearestDistance = 20000;
                int nextFloorIndex = -1;
                foreach(var (selectedFloor, floorstate) in selectedFloors)
                {
                    if(floorstate != ElevatorDirection.UNCALLED)
                    {
                        int distance = selectedFloor - currentFloorIndex;

                        if(Mathf.Abs(selectedFloor - currentFloorIndex) < nearestDistance)
                        {
                            nearestDistance = Mathf.Abs(selectedFloor - currentFloorIndex);
                            nextFloorIndex = selectedFloor;

                            if(selectedFloor > currentFloorIndex)
                            {
                                preferredDirection = ElevatorDirection.UP;
                            }
                            else
                            {
                                preferredDirection = ElevatorDirection.DOWN;
                            }
                        }
                    }
                }

                if(nextFloorIndex > -1)
                {
                    yield return TravelFloors(nextFloorIndex);
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public void AddDestination(int floorNumber, ElevatorDirection requestDirection)
    {
        if (!initialized || !selectedFloors.ContainsKey(floorNumber))
        {
            return;
        }

        Debug.Log($"Added Destination {floorNumber}");

        int requestedFloorIndex = floors.IndexOf(floorNumber);

        if (requestedFloorIndex < 0) return;

        selectedFloors[floorNumber] = requestDirection;

        if (requestDirection == ElevatorDirection.NEUTRAL && preferredDirection == ElevatorDirection.NEUTRAL)
        {
            if (requestedFloorIndex > currentFloorIndex) 
            {
                preferredDirection = ElevatorDirection.UP;
            }
            else if (requestedFloorIndex < currentFloorIndex)
            {
                preferredDirection = ElevatorDirection.DOWN;
            }
        }
    }

    private IEnumerator TravelFloors(int destinationFloor)
    {
        int traversal = destinationFloor - currentFloorIndex;
        int iteration = Mathf.Abs(traversal);

        if(doorOpen)
        {
            yield return new WaitForSeconds(1);
            yield return CloseDoor();
        }

        for(int i = 0; i < iteration; i++)
        {
            bool validFloor = false;
            if(traversal < 0) currentFloorIndex --; else currentFloorIndex ++;

            if(selectedFloors.TryGetValue(currentFloorIndex, out ElevatorDirection floorState))
            {
                if(floorState == ElevatorDirection.DOWN && preferredDirection == ElevatorDirection.DOWN)
                {
                    Debug.Log("Valid floor DOWN");
                    validFloor = true;
                }
                else if(floorState == ElevatorDirection.UP && preferredDirection == ElevatorDirection.UP)
                {
                    Debug.Log("Valid floor UP");
                    validFloor = true;
                }
                else if(floorState == ElevatorDirection.NEUTRAL)
                {
                    validFloor = true;
                }
            }

            yield return new WaitForSeconds(secondsPerFloor / 2);

            string number = currentFloorIndex.ToString("D2");
            if(number == "00")
            {
                number = "G";
            }
            foreach(TextMeshProUGUI t in floorNumberDisplays)
            {
                t.text = number;
            }

            yield return new WaitForSeconds(secondsPerFloor / 2);

            if(validFloor && IsAreaOccupied(occupationCollider))
            {
                Debug.Log("Change floor");
                yield return ElevatorManager.Instance.ChangeFloor(currentFloorIndex);              
                selectedFloors[currentFloorIndex] = ElevatorDirection.UNCALLED;
                yield return DoorSequence();
            }
        }
    }

    private IEnumerator DoorSequence()
    {
        Debug.Log("Door Sequence");
        if(currentFloorIndex == ElevatorManager.Instance.activeFloor)
        {
            foreach(ElevatorButton e in callButtons)
            {
                e.Reset();
            }
        }
        buttons[floorToButton[currentFloorIndex]].Reset();
        previousDirection = selectedFloors[currentFloorIndex];
        selectedFloors[currentFloorIndex] = ElevatorDirection.UNCALLED;

        yield return OpenDoor();
        yield return new WaitForSeconds(doorDwellDuration);
        yield return CloseDoor();

        upArrowDisplay.sprite = ArrowUpOff;
        downArrowDisplay.sprite = arrowDownOff;
    }

    private IEnumerator OpenDoor()
    {
        if (doorOpen) yield break;

        if(currentFloorIndex == ElevatorManager.Instance.activeFloor)
        {
            animator.SetBool("OpenDoors", true);
            if(previousDirection == ElevatorDirection.UP)
            {
                upArrowDisplay.sprite = ArrowUpOn;    
            }
            else if (previousDirection == ElevatorDirection.DOWN)
            {
                downArrowDisplay.sprite = arrowDownOn;
            }
        }
        else
        {
            Debug.Log("Not Active Floor");
        }
        yield return new WaitForSeconds(doorAnimationDuration);

        doorOpen = true;
    }

    private IEnumerator CloseDoor()
    {
        if (!doorOpen) yield break;

        while (true)
        {
            animator.SetBool("OpenDoors", false);

            float elapsed = 0f;
            bool interrupted = false;

            while (elapsed < doorAnimationDuration)
            {
                if (IsAreaOccupied(doorSensorCollider))
                {
                    interrupted = true;
                    break;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            if (!interrupted)
            {
                doorOpen = false;
                yield break;
            }

            animator.SetBool("OpenDoors", true);
            yield return new WaitForSeconds(doorAnimationDuration);

            while (IsAreaOccupied(doorSensorCollider)) yield return null;

            yield return null;
        }
    }

    private bool IsAreaOccupied(BoxCollider area)
    {
        if (area == null) return false;

        Vector3 scale = area.transform.lossyScale;
        Vector3 absoluteScale = new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
        Vector3 halfExtents = Vector3.Scale(area.size * 0.5f, absoluteScale);
        Vector3 center = area.transform.TransformPoint(area.center);

        return Physics.CheckBox(center, halfExtents, area.transform.rotation, playerLayer, QueryTriggerInteraction.Collide);
    }
}

