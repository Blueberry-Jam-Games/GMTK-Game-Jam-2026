using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class Elevator : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonPrefab;

    [SerializeField]
    private Transform buttonRoot;

    [SerializeField]
    private Canvas buttonsCanvas;

    [SerializeField]
    private Animator animator;

    // list of buttons
    private List<ElevatorButton> buttons;
    private float speed = 1; // Floors per second

    public float floor = 0.0f; // base 0
    private Queue<int> destinations;
    private int calledToFloor = -1;
    private bool doorOpen = true;

    private void Awake ()
    {
        buttons = new List<ElevatorButton> ();
        destinations = new Queue<int> ();
    }

    private void Start ()
    {
        StartCoroutine (DoElevator ());
    }

    public void Initialize (ElevatorEntry source)
    {
        foreach (string floor in source.floors)
        {
            GameObject newButton = GameObject.Instantiate (buttonPrefab, buttonRoot);
            ElevatorButton eb = newButton.GetComponent<ElevatorButton> ();
            eb.Initialize (this, floor);

            buttons.Add (eb);
        }

        buttonsCanvas.worldCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
    }

    private IEnumerator DoElevator ()
    {
        while (true)
        {
            if (destinations.Count != 0)
            {
                Debug.Log ($"Has new destination {destinations.Peek ()}");
                if (Mathf.Abs (destinations.Peek () - floor) > Mathf.Epsilon && !doorOpen)
                {
                    Debug.Log ("Doors closed, moving");

                    floor = Mathf.MoveTowards (floor, destinations.Peek (), speed * Time.deltaTime);
                }
                else if (Mathf.Abs (floor - destinations.Peek ()) < Mathf.Epsilon)
                {
                    Debug.Log ($"At destination {destinations.Peek ()}, doors opening");

                    buttons[destinations.Peek ()].Reset ();

                    if (calledToFloor == destinations.Peek ())
                    {
                        yield return OpenDoor ();
                    }
                    else
                    {
                        yield return BJ.Coroutines.WaitforSeconds (2); // Dwell times
                    }

                    // Done after that logic
                    destinations.Dequeue ();
                }
                else if (doorOpen)
                {
                    Debug.Log ("Ready to go, closing door");
                    yield return CloseDoor ();
                }
            }
            yield return null;
        }
    }

    public void AddDestination (string floor)
    {
        int newDest = 0;

        if (floor.Equals ("G"))
        {
            newDest = 0;      
        }
        else
        {
            newDest = int.Parse (floor) - 1;
        }

        destinations.Enqueue (newDest);
    }

    private IEnumerator OpenDoor ()
    {
        doorOpen = true;
        // play animation
        animator.Play ("DoorOpen");
        yield return new WaitForSeconds (1);
    }

    private IEnumerator CloseDoor ()
    {
        // play animation
        animator.Play ("DoorClose");
        yield return new WaitForSeconds (1);
        doorOpen = false;
    }
}
