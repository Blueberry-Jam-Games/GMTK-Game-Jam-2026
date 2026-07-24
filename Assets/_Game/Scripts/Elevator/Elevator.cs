using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using Unity.Mathematics;

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
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonRoot;
    [SerializeField] private Canvas buttonsCanvas;
    [SerializeField] private Animator animator;
    [SerializeField] private List<ElevatorButton> callButtons;

    
    [Header("Timing")]
    [SerializeField] private float secondsPerFloor = 1f;
    [SerializeField] private float doorAnimationDuration = 1f;
    [SerializeField] private float doorDwellDuration = 2f;

    public int currentFloorIndex = 0;
    // private int calledToFloor = -1;
    private bool doorOpen = true;
    private readonly List<ElevatorButton> buttons = new();
    private Dictionary<int, ElevatorDirection> selectedFloors = new();
    public List<int> floors;
    [SerializeField] private ElevatorDirection preferredDirection = ElevatorDirection.NEUTRAL;

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
            selectedFloors.Add(floor, ElevatorDirection.UNCALLED);
        }

        foreach(ElevatorButton eb in callButtons)
        {
            eb.isCallButton = true;
        }

        buttonsCanvas.worldCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();

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
                    Debug.Log("Valid floor NEUTRAL");
                    validFloor = true;
                }
            }

            yield return new WaitForSeconds(secondsPerFloor);

            if(validFloor) // Add player check here
            {
                Debug.Log("Change floor");
                yield return ElevatorManager.Instance.ChangeFloor(currentFloorIndex);
                selectedFloors[currentFloorIndex] = ElevatorDirection.UNCALLED;
                yield return DoorSequence();
            }
        }

        preferredDirection = ElevatorDirection.NEUTRAL;
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
        buttons[currentFloorIndex].Reset();
        yield return OpenDoor();
        yield return new WaitForSeconds(doorDwellDuration);
        selectedFloors[currentFloorIndex] = ElevatorDirection.UNCALLED;
        yield return CloseDoor();
    }

    private IEnumerator OpenDoor()
    {
        if (doorOpen) yield break;

        if(currentFloorIndex == ElevatorManager.Instance.activeFloor)
        {
            animator.SetBool("OpenDoors", true);
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

        animator.SetBool("OpenDoors", false);
        yield return new WaitForSeconds(doorAnimationDuration);

        doorOpen = false;
    }
}

