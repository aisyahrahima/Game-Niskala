using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Settings Popup")]
    [SerializeField] private GameObject settingsPopup;

    [Header("Exit Confirmation Popup")]
    [SerializeField] private GameObject exitConfirmPopup;

    private void Awake()
    {
        // Pastikan popup tidak aktif saat MainMenu dibuka.
        if (settingsPopup != null)
            settingsPopup.SetActive(false);

        if (exitConfirmPopup != null)
            exitConfirmPopup.SetActive(false);
    }

    public void StartGame()
    {
        LoadScene("ChapterMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {
        if (settingsPopup != null)
            settingsPopup.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPopup != null)
            settingsPopup.SetActive(false);
    }

    public void OpenExitConfirm()
    {
        if (exitConfirmPopup != null)
            exitConfirmPopup.SetActive(true);
    }

    public void CancelExit()
    {
        if (exitConfirmPopup != null)
            exitConfirmPopup.SetActive(false);
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