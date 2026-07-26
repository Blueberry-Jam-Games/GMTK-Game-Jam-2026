using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudLineItem : MonoBehaviour
{
    [SerializeField] private GameObject checkbox;
    [SerializeField] private TextMeshProUGUI screenText;

    private string localName;

    public void Initialize(string name)
    {
        checkbox.SetActive(false);
        screenText.text = name;

        localName = name;

        GameplayManager.Instance.OnItemCollected += OnItemCollected;
    }

    private void OnItemCollected(CollectableItem ci)
    {
        if (ci.itemName.Equals(localName))
        {
            checkbox.SetActive(true);
        }
    }
}
