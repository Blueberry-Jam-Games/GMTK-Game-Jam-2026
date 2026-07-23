using UnityEngine;
using System.Collections.Generic;

public class ElevatorManager : MonoBehaviour
{
    [SerializeField]
    private string elevatorPath = "Elevators";

    [SerializeField]
    private GameObject elevatorPrefab;

    private ElevatorDefinitions elevatorDefinitions;
    private List<Elevator> elevators;

    private void Awake ()
    {
        elevators = new List<Elevator> ();
    }

    private void Start()
    {
        elevatorDefinitions = Resources.Load<ElevatorDefinitions> (elevatorPath);

        foreach (ElevatorEntry ee in elevatorDefinitions.elevators)
        {
            GameObject newElevator = GameObject.Instantiate (elevatorPrefab);
            newElevator.transform.position = ee.position;
            newElevator.transform.Rotate (new Vector3 (0, GetRotation (ee.doorSide), 0));
            Elevator elev = newElevator.GetComponent <Elevator> ();
            elev.Initialize (ee);
            
            elevators.Add (elev);
        }
    }

    private float GetRotation (DoorSide doorside)
    {
        switch (doorside)
        {
            case DoorSide.POS_X:
                return 0;
            case DoorSide.NEG_X:
                return 90;
            case DoorSide.POS_Z:
                return 180;
            case DoorSide.NEG_Z:
                return 270;
            default:
                return 0;
        }
    }
}
