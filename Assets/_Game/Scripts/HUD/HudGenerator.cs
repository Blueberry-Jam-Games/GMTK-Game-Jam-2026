using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class HudGenerator : MonoBehaviour
{
    [SerializeField] private GameObject hudItemPrefab;

    [SerializeField] private Transform listRoot;

    private void Start()
    {
        StartCoroutine(DeferredStart());
    }

    private IEnumerator DeferredStart()
    {
        yield return null;

        List<CollectableItem> list = GameplayManager.Instance.GetCollections();

        for (int i = 0; i < list.Count; i++)
        {
            GameObject newItem = GameObject.Instantiate(hudItemPrefab, listRoot);
            HudLineItem hli = newItem.GetComponent<HudLineItem>();

            hli.Initialize(list[i].itemName);
        }
    }
}
