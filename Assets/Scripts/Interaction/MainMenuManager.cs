using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        LoadScene("ChapterMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void LoadScene(string sceneName)
    {
        if (SceneTransitionManager.Instance == null)
        {
            Debug.LogError(
                "SceneTransitionManager tidak ditemukan."
            );

            return;
        }

        SceneTransitionManager.Instance.LoadScene(sceneName);
    }
}