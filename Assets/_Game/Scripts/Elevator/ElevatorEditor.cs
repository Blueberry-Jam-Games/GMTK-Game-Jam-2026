#if UNITY_EDITOR
using UnityEngine;

[ExecuteAlways]
public class ElevatorEditor : MonoBehaviour
{
    [SerializeField]
    private string elevatorPath = "Elevators";

    [SerializeField]
    private int floor;

    private ElevatorDefinitions elevatorDefinitions;
    
    private readonly Color elevator = new Color (0, 1, 0, 0.25f);
    private readonly Color door = new Color (0.25f, 0.25f, 0.25f, 0.5f);
    private readonly Color noStop = new Color (1f, 0, 0, 0.25f);

    private readonly Vector3 offsetY = new Vector3(0, 1, 0);

    private void OnDrawGizmos ()
    {
        if (elevatorDefinitions != null)
        {
            foreach (ElevatorEntry ee in elevatorDefinitions.elevators)
            {
                Vector3 cubeSize;
                switch(ee.doorSide)
                {
                    case DoorSide.POS_Z:
                        cubeSize = new Vector3(elevatorDefinitions.elevatorSize.x, elevatorDefinitions.elevatorSize.y, elevatorDefinitions.elevatorSize.z);
                        break;
                    case DoorSide.NEG_Z:
                        cubeSize = new Vector3(elevatorDefinitions.elevatorSize.x, elevatorDefinitions.elevatorSize.y, elevatorDefinitions.elevatorSize.z);
                        break;
                    case DoorSide.POS_X:
                        cubeSize = new Vector3(elevatorDefinitions.elevatorSize.z, elevatorDefinitions.elevatorSize.y, elevatorDefinitions.elevatorSize.x);
                        break;
                    case DoorSide.NEG_X:
                        cubeSize = new Vector3(elevatorDefinitions.elevatorSize.z, elevatorDefinitions.elevatorSize.y, elevatorDefinitions.elevatorSize.x);
                        break;
                    default:
                        cubeSize = new Vector3(elevatorDefinitions.elevatorSize.x, elevatorDefinitions.elevatorSize.y, elevatorDefinitions.elevatorSize.z);
                        break;
                }

                Vector3 doorSize;
                switch(ee.doorSide)
                {
                    case DoorSide.POS_Z:
                        doorSize = new Vector3(elevatorDefinitions.doorSize.x, elevatorDefinitions.doorSize.y, elevatorDefinitions.doorSize.z);
                        break;
                    case DoorSide.NEG_Z:
                        doorSize = new Vector3(elevatorDefinitions.doorSize.x, elevatorDefinitions.doorSize.y, elevatorDefinitions.doorSize.z);
                        break;
                    case DoorSide.POS_X:
                        doorSize = new Vector3(elevatorDefinitions.doorSize.z, elevatorDefinitions.doorSize.y, elevatorDefinitions.doorSize.x);
                        break;
                    case DoorSide.NEG_X:
                        doorSize = new Vector3(elevatorDefinitions.doorSize.z, elevatorDefinitions.doorSize.y, elevatorDefinitions.doorSize.x);
                        break;
                    default:
                        doorSize = new Vector3(elevatorDefinitions.doorSize.x, elevatorDefinitions.doorSize.y, elevatorDefinitions.doorSize.z);
                        break;
                }
                if (ee.floors.Contains (floor))
                {
                    Gizmos.color = elevator;
                    Gizmos.DrawCube (ee.position + offsetY, cubeSize);

                    Gizmos.color = door;
                    Gizmos.DrawCube (ee.position + GetDoorOffset (ee.doorSide) + offsetY, doorSize);
                }
                else
                {
                    Gizmos.color = noStop;
                    Gizmos.DrawWireCube (ee.position + offsetY, elevatorDefinitions.elevatorSize);
                }
            }
        }
        else
        {
            elevatorDefinitions = Resources.Load<ElevatorDefinitions> (elevatorPath);
        }
    }

    private Vector3 GetDoorOffset (DoorSide doorside)
    {
        switch (doorside)
        {
            case DoorSide.POS_X:
                return new Vector3 (elevatorDefinitions.elevatorSize.z / 2.0f, 0, 0);
            case DoorSide.NEG_X:
                return new Vector3 (-elevatorDefinitions.elevatorSize.z / 2.0f, 0, 0);
            case DoorSide.POS_Z:
                return new Vector3 (0, 0, elevatorDefinitions.elevatorSize.z / 2.0f);
            case DoorSide.NEG_Z:
                return new Vector3 (0, 0, -elevatorDefinitions.elevatorSize.z / 2.0f);
            default:
                return Vector3.zero;
        }
    }
}
#endif
