using UnityEngine;

public class MenuPopupController : MonoBehaviour
{
    [Header("Panel Menu")]
    [SerializeField] private GameObject menuPopup;
    [SerializeField] private GameObject settingsPopup;
    [SerializeField] private GameObject exitConfirmPopup;

    private void Awake()
    {
        // Semua popup tertutup saat permainan dimulai.
        CloseAllPopups();

        // Pastikan gameplay berjalan saat scene dimulai.
        UpdatePauseState();
    }

    public void OpenMenu()
    {
        CloseAllPopups();

        if (menuPopup != null)
            menuPopup.SetActive(true);

        UpdatePauseState();
    }

    public void ContinueGame()
    {
        CloseAllPopups();

        UpdatePauseState();
    }

    public void OpenSettings()
    {
        if (menuPopup != null)
            menuPopup.SetActive(false);

        if (settingsPopup != null)
            settingsPopup.SetActive(true);

        UpdatePauseState();
    }

    public void BackToMenu()
    {
        if (settingsPopup != null)
            settingsPopup.SetActive(false);

        if (exitConfirmPopup != null)
            exitConfirmPopup.SetActive(false);

        if (menuPopup != null)
            menuPopup.SetActive(true);

        UpdatePauseState();
    }

    public void OpenExitConfirmation()
    {
        if (menuPopup != null)
            menuPopup.SetActive(false);

        if (exitConfirmPopup != null)
            exitConfirmPopup.SetActive(true);

        UpdatePauseState();
    }

    public void CancelExit()
    {
        if (exitConfirmPopup != null)
            exitConfirmPopup.SetActive(false);

        if (menuPopup != null)
            menuPopup.SetActive(true);

        UpdatePauseState();
    }

    public void ConfirmExit()
    {
        Debug.Log(
            "Pemain memilih kembali ke MainMenu tanpa menyimpan progress."
        );

        // Pastikan gameplay dan audio tidak lagi dalam kondisi pause.
        Time.timeScale = 1f;
        AudioListener.pause = false;

        // Hentikan ambience misterius.
        if (AmbienceManager.Instance != null)
        {
            AmbienceManager.Instance.StopAmbience();
        }

        // Hentikan ambience hujan.
        if (StorageRainAmbience.Instance != null)
        {
            StorageRainAmbience.Instance.StopRain();
        }

        // Reset seluruh status dan audio pengejaran Mayang.
        if (MayangChaseManager.Instance != null)
        {
            MayangChaseManager.Instance.ResetChaseState();
        }

        // Hentikan BGM Ending jika sedang aktif.
        if (EndingMelancholicBGM.Instance != null)
        {
            EndingMelancholicBGM.Instance.StopBGM();
        }

        // Hapus seluruh progres sesi permainan.
        NiskalaProgress.ResetAll();

        // Bersihkan alur investigasi artefak.
        ArtifactLoreFlow.Clear();
        LoreOrderFlow.Clear();

        // Kembali ke MainMenu.
        if (SceneTransitionManager.Instance == null)
        {
            Debug.LogError(
                "SceneTransitionManager tidak ditemukan."
            );
            return;
        }

        SceneTransitionManager.Instance.LoadScene("MainMenu");
    }

    private void CloseAllPopups()
    {
        if (menuPopup != null)
            menuPopup.SetActive(false);

        if (settingsPopup != null)
            settingsPopup.SetActive(false);

        if (exitConfirmPopup != null)
            exitConfirmPopup.SetActive(false);
    }

    private void UpdatePauseState()
    {
        // Periksa apakah minimal satu popup sedang aktif.
        bool isPopupOpen =
            (menuPopup != null && menuPopup.activeSelf) ||
            (settingsPopup != null && settingsPopup.activeSelf) ||
            (exitConfirmPopup != null && exitConfirmPopup.activeSelf);

        // Hentikan atau jalankan kembali gameplay.
        Time.timeScale = isPopupOpen ? 0f : 1f;

        // Hentikan atau jalankan kembali seluruh audio.
        AudioListener.pause = isPopupOpen;

        Debug.Log(
            isPopupOpen
                ? "GAMEPLAY DAN AUDIO PAUSED"
                : "GAMEPLAY DAN AUDIO RESUMED"
        );
    }
}