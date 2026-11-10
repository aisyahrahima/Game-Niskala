using UnityEngine;

public class ChapterMenuManager : MonoBehaviour
{
    public void SelectChapter1()
    {
        // Memulai permainan baru dengan progres kosong.
        NiskalaProgress.ResetAll();

        if (SceneTransitionManager.Instance == null)
        {
            Debug.LogError(
                "SceneTransitionManager tidak ditemukan."
            );

            return;
        }

        SceneTransitionManager.Instance.LoadScene("Opening");
    }

    public void BackToMainMenu()
    {
        if (SceneTransitionManager.Instance == null)
        {
            Debug.LogError(
                "SceneTransitionManager tidak ditemukan."
            );

            return;
        }

        SceneTransitionManager.Instance.LoadScene("MainMenu");
    }
}