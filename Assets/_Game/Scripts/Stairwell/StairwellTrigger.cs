using System;
using UnityEngine;

public class StairwellTrigger : MonoBehaviour
{
    public event Action OnInteraction;

    private void OnTriggerEnter(Collider other)
    {
        OnInteraction?.Invoke ();
    }
}
