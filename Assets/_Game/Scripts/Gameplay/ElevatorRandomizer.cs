using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class ElevatorRandomizer : MonoBehaviour
{
    public int delayBetweenAnnoyance = 10;
    void Start()
    {
        StartCoroutine(WaitToStart());
    }

    void Update()
    {
        
    }

    private IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(7);
        StartCoroutine(RandomizeElevators());
    }

    private IEnumerator RandomizeElevators()
    {
        while(true)
        {
            int selectedElevator = UnityEngine.Random.Range(0, 5);

            Elevator e = ElevatorManager.Instance.elevators[selectedElevator];

            int floorCount = e.floors.Count;

            int randValue = UnityEngine.Random.Range(0, floorCount);

            int selectedFloor = e.floors[randValue];

            Debug.Log($"{selectedElevator}  {selectedFloor}");

            e.AddDestination(selectedFloor, Elevator.ElevatorDirection.NEUTRAL);

            yield return new WaitForSeconds(delayBetweenAnnoyance);
        }
    }
}
