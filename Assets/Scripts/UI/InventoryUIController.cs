
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    [Header("Inventory Icons")]
    [SerializeField] private GameObject tepakSirihIcon;
    [SerializeField] private GameObject bakulIcon;
    [SerializeField] private GameObject catatanIcon;

    [Header("Artifact Zoom Controller")]
    [SerializeField] private ArtifactZoomController artifactZoomController;

    private bool lastChaseStarted;

    private void Start()
    {
        RefreshInventory();

        SetupTepakSirihButton();
        SetupBakulButton();
        SetupCatatanButton();

        lastChaseStarted = NiskalaProgress.chaseStarted;
    }

    private void Update()
    {
        // Perbarui inventory ketika status pengejaran berubah.
        if (lastChaseStarted != NiskalaProgress.chaseStarted)
        {
            lastChaseStarted = NiskalaProgress.chaseStarted;
            RefreshInventory();
        }
    }

    // =========================================
    // REFRESH INVENTORY
    // =========================================

    public void RefreshInventory()
    {
        if (tepakSirihIcon != null)
        {
            tepakSirihIcon.SetActive(
                NiskalaProgress.tepakSirihCollected &&
                !NiskalaProgress.tepakSirihReturned
            );

            Button button = tepakSirihIcon.GetComponent<Button>();

            if (button != null)
            {
                button.interactable =
                    NiskalaProgress.tepakSirihCollected &&
                    !NiskalaProgress.tepakSirihReturned &&
                    artifactZoomController != null &&
                    !NiskalaProgress.chaseStarted;
            }
        }

        if (bakulIcon != null)
        {
            bakulIcon.SetActive(
                NiskalaProgress.bakulCollected &&
                !NiskalaProgress.bakulReturned
            );

            Button button = bakulIcon.GetComponent<Button>();

            if (button != null)
            {
                button.interactable =
                    NiskalaProgress.bakulCollected &&
                    !NiskalaProgress.bakulReturned &&
                    artifactZoomController != null &&
                    !NiskalaProgress.chaseStarted;
            }
        }

        if (catatanIcon != null)
        {
            catatanIcon.SetActive(
                NiskalaProgress.catatanCikSimaCollected &&
                !NiskalaProgress.catatanCikSimaReturned
            );

            Button button = catatanIcon.GetComponent<Button>();

            if (button != null)
            {
                button.interactable =
                    NiskalaProgress.catatanCikSimaCollected &&
                    !NiskalaProgress.catatanCikSimaReturned &&
                    artifactZoomController != null &&
                    !NiskalaProgress.chaseStarted;
            }
        }
    }

    // =========================================
    // SETUP BUTTON TEPAK SIRIH
    // =========================================

    private void SetupTepakSirihButton()
    {
        if (tepakSirihIcon == null ||
            artifactZoomController == null)
        {
            return;
        }

        Button button = tepakSirihIcon.GetComponent<Button>();

        if (button == null)
        {
            Debug.LogWarning(
                "Tepak_Sirih_Inventory belum memiliki Button."
            );
            return;
        }

        button.onClick.RemoveListener(
            artifactZoomController.OpenTepakSirihZoom
        );

        button.onClick.AddListener(
            artifactZoomController.OpenTepakSirihZoom
        );
    }

    // =========================================
    // SETUP BUTTON BAKUL
    // =========================================

    private void SetupBakulButton()
    {
        if (bakulIcon == null ||
            artifactZoomController == null)
        {
            return;
        }

        Button button = bakulIcon.GetComponent<Button>();

        if (button == null)
        {
            Debug.LogWarning(
                "Bakul_Inventory belum memiliki Button."
            );
            return;
        }

        button.onClick.RemoveListener(
            artifactZoomController.OpenBakulZoom
        );

        button.onClick.AddListener(
            artifactZoomController.OpenBakulZoom
        );
    }

    // =========================================
    // SETUP BUTTON CATATAN CIK SIMA
    // =========================================

    private void SetupCatatanButton()
    {
        if (catatanIcon == null ||
            artifactZoomController == null)
        {
            return;
        }

        Button button = catatanIcon.GetComponent<Button>();

        if (button == null)
        {
            Debug.LogWarning(
                "Catatan_CikSima_Inventory belum memiliki Button."
            );
            return;
        }

        button.onClick.RemoveListener(
            artifactZoomController.OpenCatatanCikSimaZoom
        );

        button.onClick.AddListener(
            artifactZoomController.OpenCatatanCikSimaZoom
        );
    }
}
