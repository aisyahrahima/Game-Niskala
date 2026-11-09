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
        Debug.Log("Pemain memilih keluar dari permainan.");

        Application.Quit();
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

        Debug.Log(
            isPopupOpen
                ? "GAMEPLAY PAUSED"
                : "GAMEPLAY RESUMED"
        );
    }
}