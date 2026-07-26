using BJ;
using UnityEngine;

public class EndGameDoors : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        GetComponent<Interactable>().OnInteraction += OnInteract;
    }

    private void OnInteract()
    {
        LevelLoader.Instance.LoadLevel("ResultsScreen", "FadeBlack");
    }
}
