using System.Collections;
using Unity.VisualScripting;
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
        Vector3 playerPosition = player.transform.position;
        player.enableMovement = false;
        yield return null;

        float distanceStairs1 = Vector3.Distance(playerPosition, upstairsTop.position);
        float distanceStairs2 = Vector2.Distance(new Vector2(downstairsBottom.position.x, downstairsBottom.position.z), new Vector2(upstairsTop.position.x, upstairsTop.position.z));
        float distanceStairs3 = Vector3.Distance(downstairsBottom.position, downstairsTop.position);

        float totalDistance = distanceStairs1 + distanceStairs2 + distanceStairs3;

        float distance1Completion = distanceStairs1 / totalDistance;
        float distance2Completion = distanceStairs2 / totalDistance;
        float distance3Completion = distanceStairs3 / totalDistance;

        player.transform.position = upstairsBottom.position;

        localDoor.CloseDoor ();

        float progress = 0;
        while (progress < distance1Completion)
        {
            progress += ProgressIncrease();
            float actualProgress = progress / distance1Completion;
            player.transform.position = Vector3.Lerp(playerPosition, upstairsTop.position, actualProgress);
            yield return null;
        }

        Debug.Log ("Try Change floor!");
        ElevatorManager.Instance.StairwellChangeFloor(true);

        Vector3 downstairsBottomPositionAltered = downstairsBottom.position;
        downstairsBottomPositionAltered.y = upstairsTop.position.y;

        while (progress < distance2Completion + distance1Completion)
        {
            progress += ProgressIncrease();
            float actualProgress = (progress - distance1Completion) / distance2Completion;
            player.transform.position = Vector3.Lerp(upstairsTop.position, downstairsBottomPositionAltered, actualProgress);
            yield return null;
        }
        
        while (progress < 1f)
        {
            progress += ProgressIncrease();
            float actualProgress = (progress - distance1Completion - distance2Completion) / distance3Completion;
            player.transform.position = Vector3.Lerp(downstairsBottom.position, downstairsTop.position, actualProgress);
            yield return null;
        }

        player.transform.position = downstairsTop.position;

        localDoor.OpenDoor();

        // Technically a race condition but not actually
        RefreshAccess();

        playerMoving = false;
        player.enableMovement = true;
    }

    private IEnumerator DownStairs ()
    {
        playerMoving = true;

        BJCharacterController player = GameObject.FindWithTag("Player").GetComponent<BJCharacterController>();
        Vector3 playerPosition = player.transform.position;
        player.enableMovement = false;
        yield return null;

        float distanceStairs1 = Vector3.Distance(playerPosition, downstairsBottom.position);
        float distanceStairs2 = Vector2.Distance(new Vector2(downstairsBottom.position.x, downstairsBottom.position.z), new Vector2(upstairsTop.position.x, upstairsTop.position.z));
        float distanceStairs3 = Vector3.Distance(upstairsTop.position, upstairsBottom.position);

        float totalDistance = distanceStairs1 + distanceStairs2 + distanceStairs3;

        float distance1Completion = distanceStairs1 / totalDistance;
        float distance2Completion = distanceStairs2 / totalDistance;
        float distance3Completion = distanceStairs3 / totalDistance;

        localDoor.CloseDoor ();

        float progress = 0;
        while (progress < distance1Completion)
        {
            progress += ProgressIncrease();
            float actualProgress = progress / distance1Completion;
            player.transform.position = Vector3.Lerp(playerPosition, downstairsBottom.position, actualProgress);
            yield return null;
        }

        ElevatorManager.Instance.StairwellChangeFloor(false);

        Vector3 upstairsTopPositionAltered = upstairsTop.position;
        upstairsTopPositionAltered.y = downstairsBottom.position.y;

        while (progress < distance2Completion + distance1Completion)
        {
            progress += ProgressIncrease();
            float actualProgress = (progress - distance1Completion) / distance2Completion;
            player.transform.position = Vector3.Lerp(downstairsBottom.position, upstairsTopPositionAltered, actualProgress);
            yield return null;
        }

        while (progress < 1f)
        {
            float actualProgress = (progress - distance1Completion - distance2Completion) / distance3Completion;
            progress += ProgressIncrease();
            player.transform.position = Vector3.Lerp(upstairsTop.position, upstairsBottom.position, actualProgress);
            yield return null;
        }

        player.transform.position = upstairsBottom.position;

        localDoor.OpenDoor();

        // Technically a race condition but not actually
        RefreshAccess();

        playerMoving = false;
        player.enableMovement = true;
    }

    private float ProgressIncrease ()
    {
        return 1.0f / travelTime * Time.deltaTime;
    }
}
