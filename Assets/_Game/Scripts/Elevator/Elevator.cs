using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Elevator : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonPrefab;

    [SerializeField]
    private Transform buttonRoot;

    // list of buttons
    private List<ElevatorButton> buttons;

    private void Awake ()
    {
        buttons = new List<ElevatorButton> ();
    }

    public void Initialize (ElevatorEntry source)
    {
        foreach (string floor in source.floors)
        {
            GameObject newButton = GameObject.Instantiate (buttonPrefab, buttonRoot);
            ElevatorButton eb = newButton.GetComponent<ElevatorButton> ();
            eb.Initialize (floor);

            buttons.Add (eb);
        }
    }
}
