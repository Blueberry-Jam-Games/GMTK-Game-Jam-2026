using UnityEngine;

[RequireComponent(typeof(Speakable))]
public class Returnable : MonoBehaviour
{
    [SerializeField] private string collectionName;

    [SerializeField] private bool destroyOnSelect;

    private void Start()
    {
        Speakable interaction = GetComponent<Speakable>();
        interaction.OnInteractionEnd += OnInteract;
    }

    private void OnInteract()
    {
        GameplayManager.Instance.MarkFound(collectionName);
    }
}
