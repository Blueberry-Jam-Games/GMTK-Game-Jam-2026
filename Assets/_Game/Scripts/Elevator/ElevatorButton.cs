using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ElevatorButton : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI floorText;

    [SerializeField]
    private Toggle buttonToggle;

    private void Start()
    {
        buttonToggle.onValueChanged.AddListener (OnToggle);
    }

    private void Update()
    {
        
    }

    private void OnToggle (bool toggle)
    {
        // Do something interesting
    }

    public void Initialize (string floor)
    {
        floorText.text = floor;
    }
}
