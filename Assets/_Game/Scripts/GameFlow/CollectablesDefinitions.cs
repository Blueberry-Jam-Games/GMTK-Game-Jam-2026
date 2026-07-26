using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Elevators", menuName = "Elevators/CollectableDefinitions", order = 1)]
public class CollectablesDefinitions : ScriptableObject
{
    public List<CollectableItem> collectables;
}

[System.Serializable]
public class CollectableItem
{
    public string itemName;
    public int pointValue;
    public bool collected;

    public void MarkCollected()
    {
        collected = true;
    }
}
