using UnityEngine;
using System.Collections.Generic;
using BJ;
using UnityEngine.SceneManagement;
using System.Collections;

public class ElevatorManager : SingletonGameObject<ElevatorManager>
{
    [SerializeField]
    private string elevatorPath = "Elevators";

    [SerializeField]
    private string startingFloor = "GroundFloor";

    [SerializeField]
    private GameObject elevatorPrefab;

    private ElevatorDefinitions elevatorDefinitions;
    private List<Elevator> elevators;

    private string activeScene;

    protected override void Awake ()
    {
        base.Awake ();
        elevators = new List<Elevator> ();
    }

    private void Start()
    {
        elevatorDefinitions = Resources.Load<ElevatorDefinitions> (elevatorPath);

        foreach (ElevatorEntry ee in elevatorDefinitions.elevators)
        {
            GameObject newElevator = GameObject.Instantiate (elevatorPrefab, this.transform);
            newElevator.transform.position = ee.position;
            newElevator.transform.Rotate (new Vector3 (0, GetRotation (ee.doorSide), 0));
            Elevator elev = newElevator.GetComponent <Elevator> ();
            elev.Initialize (ee);
            
            elevators.Add (elev);
        }

        SceneManager.LoadScene (startingFloor, LoadSceneMode.Additive);
        activeScene = startingFloor;
    }

    private float GetRotation (DoorSide doorside)
    {
        switch (doorside)
        {
            case DoorSide.POS_X:
                return 180;
            case DoorSide.NEG_X:
                return 270;
            case DoorSide.POS_Z:
                return 0;
            case DoorSide.NEG_Z:
                return 90;
            default:
                return 0;
        }
    }

    private void ChangeFloor (string nextFloor)
    {
        StartCoroutine (ChangeFloorSequence (nextFloor));
    }

    private IEnumerator ChangeFloorSequence (string nextFloor)
    {
        yield return new WaitForSeconds (1);

        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync (activeScene);

        // start elevator animation

        while (!asyncUnload.isDone)
        {
            yield return null;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync (nextFloor, LoadSceneMode.Additive);
        activeScene = nextFloor;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds (1);
        // Elevator doors open
    }
}
