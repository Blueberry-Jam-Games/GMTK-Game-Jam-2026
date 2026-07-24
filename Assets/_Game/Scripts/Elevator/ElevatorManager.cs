using UnityEngine;
using System.Collections.Generic;
using BJ;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Data;

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

    public string activeScene;
    public int activeFloor;

    protected override void Awake ()
    {
        base.Awake ();
        elevators = new List<Elevator> ();
        elevatorDefinitions = Resources.Load<ElevatorDefinitions> (elevatorPath);
    }

    private void Start()
    {
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
        TryGetFloorName(startingFloor, out int floorNumber);
        activeFloor = floorNumber;
    }

    private float GetRotation (DoorSide doorside)
    {
        switch (doorside)
        {
            case DoorSide.POS_X:
                return 90;
            case DoorSide.NEG_X:
                return 270;
            case DoorSide.POS_Z:
                return 0;
            case DoorSide.NEG_Z:
                return 180;
            default:
                return 0;
        }
    }

    public IEnumerator ChangeFloor(int floorNumber)
    {
        if (!TryGetSceneName(floorNumber, out string nextScene))
        {
            Debug.LogError($"No scene is mapped to elevator floor '{floorNumber}'.");
            yield break;
        }

        if (nextScene == activeScene) yield break;

        yield return ChangeFloorSequence(nextScene, floorNumber);
    }

    private IEnumerator ChangeFloorSequence(string nextScene, int floorNumber)
    {
        Scene oldScene = SceneManager.GetSceneByName(activeScene);

        if (oldScene.IsValid() && oldScene.isLoaded)
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(oldScene);
            // Wait for scene to unload
            while (asyncUnload != null && !asyncUnload.isDone) yield return null;
        }

        activeScene = nextScene;
        activeFloor = floorNumber;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
        while (asyncLoad != null && !asyncLoad.isDone) yield return null;

        Scene loadedScene = SceneManager.GetSceneByName(nextScene);
        if (loadedScene.IsValid()) SceneManager.SetActiveScene(loadedScene);
    }

    public bool TryGetFloorName(string sceneName, out int floorNumber)
    {
        for (int i = 0; i < elevatorDefinitions.levelMap.Count; i++)
        {
            string map = elevatorDefinitions.levelMap[i];
            if (map != sceneName) continue;

            floorNumber = i;
            return true;
        }

        floorNumber = -1;
        return false;
    }

    private bool TryGetSceneName(int floorNumber, out string sceneName)
    {
        sceneName = null;
        if(floorNumber < 0 || floorNumber >= elevatorDefinitions.levelMap.Count) return false;
        
        sceneName = elevatorDefinitions.levelMap[floorNumber];
        return true;
    }
}
