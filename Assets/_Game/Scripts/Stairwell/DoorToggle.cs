using UnityEngine;
using System.Collections;
using BJ;

public class DoorToggle : MonoBehaviour
{
    [SerializeField]
    private Interactable doorInteraction;

    private Animator animator;

    private bool doorOpen = false;
    private bool doorMoving = false;

    private void Start()
    {
        doorInteraction.OnInteraction += DoorInteract;
        animator = GetComponent<Animator>();
    }

    private void DoorInteract()
    {
        if (!doorMoving)
        {
            StartCoroutine(ToggleDoor());
        }
    }

    public void CloseDoor ()
    {
        if (doorOpen && !doorMoving)
        {
            StartCoroutine(ToggleDoor());
        }
    }

    public void OpenDoor ()
    {
        if (!doorOpen && !doorMoving)
        {
            StartCoroutine(ToggleDoor());
        }
    }

    private IEnumerator ToggleDoor()
    {
        doorMoving = true;

        gameObject.GetComponent<SoundManager>().PlaySound("DoorSound");

        if(doorOpen)
        {
            animator.Play("StairwellDoorClose");
        }
        else
        {
            animator.Play("StairwellDoorOpen");
        }

        yield return BJ.Coroutines.WaitforSeconds(0.5f);

        doorOpen = !doorOpen;
        doorMoving = false;
    }
}
