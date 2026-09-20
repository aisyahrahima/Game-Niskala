using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        FadeManager.Instance.LoadScene("ChapterMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}