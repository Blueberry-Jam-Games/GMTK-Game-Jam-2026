using UnityEngine;
using TMPro;
using BJ;
using System.Collections;

public class TimeScript : MonoBehaviour
{
    [SerializeField] private GameObject timeSource;
    [SerializeField] private GameObject todolist;
    [SerializeField] private TextMeshProUGUI clockface;

    [SerializeField] private GameObject timesup;

    private float totalTime = 600;
    private float startTime = 0;

    private void Start()
    {
        clockface.text = "10:00";
        timeSource.gameObject.SetActive(false);
        todolist.SetActive(false);
        timesup.SetActive(false);
    }

    public void StartTime()
    {
        startTime = Time.time;
        timeSource.gameObject.SetActive(true);
        todolist.SetActive(true);
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

            if (timeDelta < 0)
            {
                timeSource.gameObject.SetActive(false);
                startTime = 0;

                StartCoroutine(TimesUpNotice());
            }
        }
    }

    private IEnumerator TimesUpNotice()
    {
        timesup.SetActive(true);
        yield return new WaitForSeconds(2);
        LevelLoader.Instance.LoadLevel("ResultsScreen", "FadeBlack");
    }
}
