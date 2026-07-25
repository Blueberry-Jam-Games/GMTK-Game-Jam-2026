using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Stairwell : MonoBehaviour
{
    [SerializeField]
    private StairwellTrigger upTrigger;
    [SerializeField]
    private StairwellTrigger downTrigger;

    [SerializeField]
    private Transform upstairsBottom;
    [SerializeField]
    private Transform upstairsTop;
    [SerializeField]
    private Transform downstairsBottom;
    [SerializeField]
    private Transform downstairsTop;

    [SerializeField]
    private GameObject upstairsBlock;
    [SerializeField]
    private GameObject downstairsBlock;

    [SerializeField]
    private float travelTime = 2.0f;

    private DoorToggle localDoor;
    bool playerMoving = false;

    private void Start()
    {
        upTrigger.OnInteraction += UpTriggerEvent;
        downTrigger.OnInteraction += DownTriggerEvent;
        localDoor = GetComponent<DoorToggle>();

        RefreshAccess();
    }

    private void RefreshAccess()
    {
        upstairsBlock.SetActive(!ElevatorManager.Instance.StairwellHasFloor(true));
        downstairsBlock.SetActive(!ElevatorManager.Instance.StairwellHasFloor(false));
    }

    private void UpTriggerEvent ()
    {
        if (!playerMoving && !upstairsBlock.activeInHierarchy)
        {
            StartCoroutine(UpStairs());
        }
    }

    private void DownTriggerEvent ()
    {
        if (!playerMoving && !downstairsBlock.activeInHierarchy)
        {
            StartCoroutine(DownStairs());
        }
    }

    private IEnumerator UpStairs ()
    {
        playerMoving = true;

        BJCharacterController player = GameObject.FindWithTag("Player").GetComponent<BJCharacterController>();
        player.enableMovement = false;
        yield return null;
        player.enableMovement = true;

        player.transform.position = upstairsBottom.position;

        localDoor.CloseDoor ();

        float progress = 0;
        while (progress < 1f)
        {
            progress += ProgressIncrease();
            player.transform.position = Vector3.Lerp(upstairsBottom.position, upstairsTop.position, progress);
            yield return null;
        }

        Debug.Log ("Try Change floor!");
        ElevatorManager.Instance.StairwellChangeFloor(true);

        progress = 0;
        while (progress < 1f)
        {
            progress += ProgressIncrease();
            player.transform.position = Vector3.Lerp(downstairsBottom.position, downstairsTop.position, progress);
            yield return null;
        }

        player.transform.position = downstairsTop.position;

        localDoor.OpenDoor();

        // Technically a race condition but not actually
        RefreshAccess();

        playerMoving = false;
    }

    private IEnumerator DownStairs ()
    {
        playerMoving = true;

        BJCharacterController player = GameObject.FindWithTag("Player").GetComponent<BJCharacterController>();
        player.enableMovement = false;
        yield return null;
        player.enableMovement = true;

        player.transform.position = downstairsTop.position;

        localDoor.CloseDoor ();

        float progress = 0;
        while (progress < 1f)
        {
            progress += ProgressIncrease();
            player.transform.position = Vector3.Lerp(downstairsTop.position, downstairsBottom.position, progress);
            yield return null;
        }

        ElevatorManager.Instance.StairwellChangeFloor(false);

        progress = 0;
        while (progress < 1f)
        {
            progress += ProgressIncrease();
            player.transform.position = Vector3.Lerp(upstairsTop.position, upstairsBottom.position, progress);
            yield return null;
        }

        player.transform.position = upstairsBottom.position;

        localDoor.OpenDoor();

        // Technically a race condition but not actually
        RefreshAccess();

        playerMoving = false;
    }

    private float ProgressIncrease ()
    {
        return 1.0f / travelTime * 2 * Time.deltaTime;
    }
}
