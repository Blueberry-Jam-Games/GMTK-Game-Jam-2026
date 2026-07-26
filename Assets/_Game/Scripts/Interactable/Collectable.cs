using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class Collectable : MonoBehaviour
{
    [SerializeField] private string collectionName;

    [SerializeField] private bool destroyOnSelect;

    private void Start()
    {
        Interactable interaction = GetComponent<Interactable>();
        interaction.OnInteraction += OnInteract;
    }

    private void OnInteract()
    {
        GameplayManager.Instance.MarkFound(collectionName);

        if (destroyOnSelect)
        {
            Destroy(this.gameObject);
        }
    }
}
