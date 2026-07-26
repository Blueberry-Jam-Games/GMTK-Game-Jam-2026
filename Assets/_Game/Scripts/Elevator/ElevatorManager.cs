using UnityEngine;
using System.Collections.Generic;
using BJ;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Data;

public class ElevatorManager : SingletonGameObject<ElevatorManager>
{
    private readonly string loadReasonElevators = "ELEVATORS";

    [SerializeField]
    private string elevatorPath = "Elevators";

    [SerializeField]
    private string startingFloor = "GroundFloor";

    [SerializeField]
    private GameObject elevatorPrefab;
    [SerializeField]
    private GameObject stairwellPrefab;

    private ElevatorDefinitions elevatorDefinitions;
    private List<Elevator> elevators;

    public List<bool> disabledFloors;
    private List<Stairwell> stairwells;

    public string activeScene;
    public int activeFloor;

    protected override void Awake ()
    {
        base.Awake ();
        elevators = new List<Elevator> ();
        stairwells = new List<Stairwell> ();
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

        foreach(string s in elevatorDefinitions.levelMap)
        {
            disabledFloors.Add(false);
        }
        foreach (StairwellEntry se in elevatorDefinitions.stairwells)
        {
            GameObject newStairwell = GameObject.Instantiate (stairwellPrefab, this.transform);
            newStairwell.transform.position = se.position;
            newStairwell.transform.Rotate (new Vector3 (0, GetStairwellRotation (se.doorSide), 0));
            Stairwell swell = newStairwell.GetComponent <Stairwell> ();
            //swell.Initialize (ee);
            
            stairwells.Add (swell);
        }

        bool editorMode = false;

        #if UNITY_EDITOR
        GameObject editor = GameObject.FindWithTag("DebugEditor");
        if (editor != null)
        {
            editorMode = true;
            ElevatorEditor elevedit = editor.GetComponent<ElevatorEditor>();
            startingFloor = elevedit.currentFloor;
        }
        #endif

        // First, see if we need to hold the screen black for any period of time
        if(LevelLoader.Instance != null && !editorMode)
        {
            LevelLoader.Instance.HoldLevelLoad(loadReasonElevators);
            GameObject player = GameObject.FindWithTag("Player");
            BJCharacterController controller = player.GetComponent<BJCharacterController>();
            controller.enableMovement = false;
            controller.enableMouse = false;
            controller.enableInteraction = false;
            StartCoroutine(LoadReasonClear());
        }

        SceneManager.LoadScene (startingFloor, LoadSceneMode.Additive);
        activeScene = startingFloor;
        TryGetFloorName(startingFloor, out int floorNumber);
        activeFloor = floorNumber;
    }

    public void Reset()
    {
        LevelLoader.Instance.OnSceneLoaded += LevelLoaded;
        LevelLoader.Instance.LoadLevel("ElevatorsMain", "FadeBlack", "textIn");
    }

    private void LevelLoaded(string levelName)
    {
        if (levelName.Equals("ElevatorsMain"))
        {
            LevelLoader.Instance.OnSceneLoaded -= LevelLoaded;

            LevelLoader.Instance.HoldLevelLoad(loadReasonElevators);
            GameObject player = GameObject.FindWithTag("Player");
            BJCharacterController controller = player.GetComponent<BJCharacterController>();
            controller.enableMovement = false;
            controller.enableMouse = false;
            controller.enableInteraction = false;
            StartCoroutine(LoadReasonClear());

            SceneManager.LoadScene (startingFloor, LoadSceneMode.Additive);
            activeScene = startingFloor;
            TryGetFloorName(startingFloor, out int floorNumber);
            activeFloor = floorNumber;
        }
    }

    private IEnumerator LoadReasonClear()
    {
        yield return BJ.Coroutines.WaitforSeconds(2);
        LevelLoader.Instance.ResolveLevelLoad(loadReasonElevators);
        GameObject player = GameObject.FindWithTag("Player");
        BJCharacterController controller = player.GetComponent<BJCharacterController>();
        controller.enableMovement = true;
        controller.enableMouse = true;
        controller.enableInteraction = true;
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

    private float GetStairwellRotation (DoorSide doorside)
    {
        switch (doorside)
        {
            case DoorSide.POS_X:
                return 0;
            case DoorSide.NEG_X:
                return 180;
            case DoorSide.POS_Z:
                return 270;
            case DoorSide.NEG_Z:
                return 90;
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

        foreach(Elevator e in elevators)
        {
            if(e.floors.Contains(activeFloor))
            {
                e.visibleLayer.SetActive(true);
            }
            else
            {
                e.visibleLayer.SetActive(false);
            }
        }
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

        foreach(Elevator e in elevators)
        {
            if(e.floors.Contains(activeFloor))
            {
                e.visibleLayer.SetActive(true);
            }
            else
            {
                e.visibleLayer.SetActive(false);
            }
        }
    }

    public bool StairwellHasFloor (bool up)
    {
        if(up)
        {
            return TryGetSceneName(activeFloor + 1, out string _);
        }
        else
        {
            return TryGetSceneName(activeFloor - 1, out string _);
        }
    }

    public void StairwellChangeFloor (bool upwards)
    {
        StartCoroutine (ChangeFloor(upwards ? activeFloor + 1 : activeFloor - 1));
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

    public bool FloorDisabled(int floorNumber)
    {
        return disabledFloors[floorNumber];
    }
}
