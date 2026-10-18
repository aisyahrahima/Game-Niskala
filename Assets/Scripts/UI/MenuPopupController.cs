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
    }

    public void OpenMenu()
    {
        CloseAllPopups();
        menuPopup.SetActive(true);
    }

    public void ContinueGame()
    {
        CloseAllPopups();
    }

    public void OpenSettings()
    {
        menuPopup.SetActive(false);
        settingsPopup.SetActive(true);
    }

    public void BackToMenu()
    {
        settingsPopup.SetActive(false);
        exitConfirmPopup.SetActive(false);
        menuPopup.SetActive(true);
    }

    public void OpenExitConfirmation()
    {
        menuPopup.SetActive(false);
        exitConfirmPopup.SetActive(true);
    }

    public void CancelExit()
    {
        exitConfirmPopup.SetActive(false);
        menuPopup.SetActive(true);
    }

    public void ConfirmExit()
    {
        Debug.Log("Pemain memilih keluar dari permainan.");

        Application.Quit();
    }

    private void CloseAllPopups()
    {
        menuPopup.SetActive(false);
        settingsPopup.SetActive(false);
        exitConfirmPopup.SetActive(false);
    }
}