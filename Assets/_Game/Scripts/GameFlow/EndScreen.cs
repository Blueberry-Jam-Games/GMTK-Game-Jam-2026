using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private GameObject lineItemPrefab;
    
    [SerializeField] private Transform lineItemRoot;

    [SerializeField] private TextMeshProUGUI totalText;

    private float total = 0;

    private void Start()
    {
        totalText.text = "0";

        StartCoroutine(DisplayList());
    }

    private IEnumerator DisplayList()
    {
        yield return new WaitForSeconds(0.5f);

        List<CollectableItem> items = GameplayManager.Instance.GetCollections();

        foreach (CollectableItem ci in items)
        {
            GameObject newItem = GameObject.Instantiate(lineItemPrefab, lineItemRoot);
            EndScreenLineItem lineItem = newItem.GetComponent<EndScreenLineItem>();
            lineItem.Initialize(ci);

            if (ci.collected)
            {
                total += ci.pointValue;
                totalText.text = "" + total;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
