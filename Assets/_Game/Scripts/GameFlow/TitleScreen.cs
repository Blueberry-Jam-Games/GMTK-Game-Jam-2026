using BJ;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    [SerializeField]
    private Button playButton;
    [SerializeField]
    private Button creditsButton;
    [SerializeField]
    private Button quitButton;

    [SerializeField]
    private LevelTransitionEffect textInEffect;

    private bool levelLoading = false;

    static bool runOnce = false;

    private void Start()
    {
        playButton.onClick.AddListener(PlayButtonPressed);

        if(!runOnce)
        {
            runOnce = true;
            LevelTransitionEffect.Templates.FadeTransition("FadeBlack", 1.0f, Color.black, false, Color.black, Color.black);
            LevelLoader.Instance.RegisterTransition("textIn", textInEffect);
        }
        else
        {
            Destroy(textInEffect.gameObject);   
        }

        quitButton.onClick.AddListener(Quit);

        creditsButton.onClick.AddListener(Credits);
    }

    private void PlayButtonPressed()
    {
        if(!levelLoading)
        {
            levelLoading = true;
            LevelLoader.Instance.LoadLevel("ElevatorsMain", "FadeBlack", "textIn");
        }
    }

    private void Quit()
    {
        Application.Quit();
    }

    private void Credits()
    {
        if (!levelLoading)
        {
            levelLoading = true;
            LevelLoader.Instance.LoadLevel("CreditsScreen", "FadeBlack");
        }
    }
}
