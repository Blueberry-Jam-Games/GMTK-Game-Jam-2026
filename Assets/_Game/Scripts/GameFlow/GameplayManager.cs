using UnityEngine;
using System.Collections.Generic;
using BJ;

public class GameplayManager : SingletonGameObject<GameplayManager>
{
    [SerializeField]
    private string elevatorPath = "Collectables";

    private Dictionary<string, CollectableItem> collection;

    private CollectablesDefinitions collectablesDefinitions;

    public delegate void ItemCollectEvent(CollectableItem item);

    public event ItemCollectEvent OnItemCollected;

    public bool removeBarriers = false;

    public void Start()
    {
        collectablesDefinitions = Resources.Load<CollectablesDefinitions> (elevatorPath);
        collection = new Dictionary<string, CollectableItem>();

        foreach (CollectableItem entry in collectablesDefinitions.collectables)
        {
            collection[entry.itemName] = entry;
        }

        removeBarriers = false;
    }

    public void MarkFound(string item)
    {
        if (collection.ContainsKey(item))
        {
            collection[item].MarkCollected();
            OnItemCollected?.Invoke(collection[item]);
        }
        else
        {
            Debug.LogError($"Tried to mark {item} collected but it is not defined");
        }
    }

    public List<CollectableItem> GetCollections()
    {
        return collectablesDefinitions.collectables;
    }
}
