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

    private void Start()
    {
        // Atur tampilan inventory sesuai progres.
        RefreshInventory();

        // Hubungkan ketiga tombol inventory.
        SetupTepakSirihButton();
        SetupBakulButton();
        SetupCatatanButton();
    }

    // =========================================
    // REFRESH INVENTORY
    // =========================================

    public void RefreshInventory()
    {
        if (tepakSirihIcon != null)
        {
            tepakSirihIcon.SetActive(
                NiskalaProgress.tepakSirihCollected
            );

            Button button =
                tepakSirihIcon.GetComponent<Button>();

            if (button != null)
            {
                button.interactable =
                    NiskalaProgress.tepakSirihCollected &&
                    artifactZoomController != null;
            }
        }

        if (bakulIcon != null)
        {
            bakulIcon.SetActive(
                NiskalaProgress.bakulCollected
            );

            Button button =
                bakulIcon.GetComponent<Button>();

            if (button != null)
            {
                button.interactable =
                    NiskalaProgress.bakulCollected &&
                    artifactZoomController != null;
            }
        }

        if (catatanIcon != null)
        {
            catatanIcon.SetActive(
                NiskalaProgress.catatanCikSimaCollected
            );

            Button button =
                catatanIcon.GetComponent<Button>();

            if (button != null)
            {
                button.interactable =
                    NiskalaProgress.catatanCikSimaCollected &&
                    artifactZoomController != null;
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

        Button button =
            tepakSirihIcon.GetComponent<Button>();

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

        Button button =
            bakulIcon.GetComponent<Button>();

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

        Button button =
            catatanIcon.GetComponent<Button>();

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