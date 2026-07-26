using BJ;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScreen : MonoBehaviour
{
    [SerializeField] private Button mainMenu;

    bool levelLoading = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        mainMenu.onClick.AddListener(ToMainMenu);
    }

    private void ToMainMenu()
    {
        if(!levelLoading)
        {
            levelLoading = true;
            LevelLoader.Instance.LoadLevel("TitleScreen", "FadeBlack");
        }
    }
}
