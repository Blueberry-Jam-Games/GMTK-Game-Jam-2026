using UnityEngine;
using TMPro;
using BJ;

public class TimeScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clockface;

    private float totalTime = 600;
    private float startTime = 0;

    private void Start()
    {
        clockface.text = "10:00";

        LevelLoader.Instance.OnCurtainsLifted += OnCurtainsLifted;
    }

    private void OnCurtainsLifted(string scene)
    {
        startTime = Time.time;
    }

    private void Update()
    {
        if (startTime != 0)
        {
            float timeDelta = totalTime - (Time.time - startTime);

            int minutes = Mathf.FloorToInt(timeDelta / 60);
            int seconds = Mathf.FloorToInt(timeDelta) % 60;

            // Debug.Log($"Time remaining {timeDelta} min: {minutes} sec: {seconds} totalTimeInt {Mathf.FloorToInt(timeDelta)}");
            clockface.text = $"{minutes:D2}:{seconds:D2}";
        }
    }
}
