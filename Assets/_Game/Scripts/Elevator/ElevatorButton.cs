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

    private string floorMeaning;

    private Elevator parent;

    private void Start()
    {
        buttonToggle.onValueChanged.AddListener (OnToggle);
    }

    private void Update()
    {
        
    }

    private void OnToggle (bool toggle)
    {
        Debug.Log ($"Button {floorMeaning} pressed");
        if (!toggle)
        {
            buttonToggle.isOn = true;
            // do nothing
        }
        else
        {
            parent.AddDestination (floorMeaning);
        }
        // Do something interesting
    }

    public void Initialize (Elevator parent, string floor)
    {
        floorText.text = floor;
        floorMeaning = floor;
        this.parent = parent;
    }

    public void Reset ()
    {
        buttonToggle.isOn = false;
    }
}
