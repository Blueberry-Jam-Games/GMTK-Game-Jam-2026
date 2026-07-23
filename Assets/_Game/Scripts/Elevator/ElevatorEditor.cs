#if UNITY_EDITOR
using UnityEngine;

[ExecuteAlways]
public class ElevatorEditor : MonoBehaviour
{
    [SerializeField]
    private string elevatorPath = "Elevators";

    [SerializeField]
    private string floor;

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
                if (ee.floors.Contains (floor))
                {
                    Gizmos.color = elevator;
                    Gizmos.DrawCube (ee.position + offsetY, elevatorDefinitions.elevatorSize);

                    Gizmos.color = door;
                    Gizmos.DrawCube (ee.position + GetDoorOffset (ee.doorSide) + offsetY, elevatorDefinitions.doorSize);
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
