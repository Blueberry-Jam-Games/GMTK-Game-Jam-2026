using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Elevators", menuName = "Elevators/Elevator Collection", order = 1)]
public class ElevatorDefinitions : ScriptableObject
{
    public Vector3 elevatorSize;
    public Vector3 doorSize;

    public List<ElevatorEntry> elevators;
    public List<string> levelMap;
}


[System.Serializable]
public struct ElevatorEntry
{
    public Vector3 position;
    public List<int> floors;
    public DoorSide doorSide;
}

[System.Serializable]
public enum DoorSide
{
    POS_X, POS_Z, NEG_X, NEG_Z
}

[System.Serializable]
public struct LevelMap
{
    public string SceneName;
}
