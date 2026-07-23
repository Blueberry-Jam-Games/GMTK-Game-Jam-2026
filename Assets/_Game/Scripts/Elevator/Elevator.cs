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

    private float floor = 0.0f; // base 0
    private Queue<int> destinations;
    private int calledToFloor = -1;
    private bool doorOpen = false;

    private void Awake ()
    {
        buttons = new List<ElevatorButton> ();
        destinations = new Queue<int> ();
    }

    public void Initialize (ElevatorEntry source)
    {
        foreach (string floor in source.floors)
        {
            GameObject newButton = GameObject.Instantiate (buttonPrefab, buttonRoot);
            ElevatorButton eb = newButton.GetComponent<ElevatorButton> ();
            eb.Initialize (floor);

            buttons.Add (eb);
        }

        buttonsCanvas.worldCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
    }

    private IEnumerator DoElevator ()
    {
        while (true)
        {
            if (destinations.Peek () - floor > Mathf.Epsilon && !doorOpen)
            {
                floor = Mathf.MoveTowards (floor, destinations.Peek (), speed / Time.deltaTime);
            }
            else if (floor - destinations.Peek () < Mathf.Epsilon)
            {
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
        }
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
