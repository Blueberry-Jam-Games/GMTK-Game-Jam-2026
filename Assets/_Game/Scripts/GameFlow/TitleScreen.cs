using BJ;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    [SerializeField]
    private Button playButton;

    private bool levelLoading = false;

    private void Start()
    {
        playButton.onClick.AddListener(PlayButtonPressed);
        LevelTransitionEffect.Templates.FadeTransition("FadeBlack", 1.0f, Color.black, false, Color.black, Color.black);
    }

    private void PlayButtonPressed()
    {
        if(!levelLoading)
        {
            levelLoading = true;
            LevelLoader.Instance.LoadLevel("ElevatorsMain", "FadeBlack");
        }
    }
}
