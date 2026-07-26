using UnityEngine;
using TMPro;

public class EndScreenLineItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private GameObject checkIcon;
    [SerializeField] private GameObject crossIcon;
    [SerializeField] private TextMeshProUGUI score;
 
    public void Initialize(CollectableItem source)
    {
        itemDescription.text = source.itemName;
        checkIcon.SetActive(source.collected);
        crossIcon.SetActive(!source.collected);
        score.text = source.collected ? source.pointValue + "" : "0";
    }
}
