using UnityEngine;

public class ChapterMenuManager : MonoBehaviour
{
    public void SelectChapter1()
    {
        if (SceneTransitionManager.Instance == null)
        {
            Debug.LogError(
                "SceneTransitionManager tidak ditemukan."
            );

            return;
        }

        SceneTransitionManager.Instance.LoadScene("Opening");
    }
}