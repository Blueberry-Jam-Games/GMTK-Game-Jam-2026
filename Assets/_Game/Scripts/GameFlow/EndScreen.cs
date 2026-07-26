using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using BJ;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private GameObject lineItemPrefab;
    
    [SerializeField] private Transform lineItemRoot;

    [SerializeField] private TextMeshProUGUI totalText;

    [SerializeField] private Button playAgainButton;

    private float total = 0;
    private bool animDone = false;

    private void Start()
    {
        totalText.text = "0";

        playAgainButton.onClick.AddListener(OnPlayAgain);

        StartCoroutine(DisplayList());

        GameObject player = GameObject.FindWithTag("PlayerRoot");
        if (player != null)
        {
            GameObject.Destroy(player);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
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

        animDone = true;
    }

    private void OnPlayAgain()
    {
        if(animDone)
        {
            ElevatorManager.Instance.Reset();
        }
    }
}
