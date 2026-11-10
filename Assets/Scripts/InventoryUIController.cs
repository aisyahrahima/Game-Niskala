using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    [Header("Inventory Icons")]
    [SerializeField] private GameObject tepakSirihIcon;
    [SerializeField] private GameObject bakulIcon;
    [SerializeField] private GameObject catatanIcon;

    [Header("Room Controllers")]
    [SerializeField] private StorageRoomInteraction storageController;
    [SerializeField] private BedroomLockerController bedroomController;

    private void Start()
    {
        RefreshInventory();

        SetupBakulButton();
        SetupCatatanButton();
    }

    public void RefreshInventory()
    {
        if (tepakSirihIcon != null)
        {
            tepakSirihIcon.SetActive(
                NiskalaProgress.tepakSirihCollected
            );
        }

        if (bakulIcon != null)
        {
            bakulIcon.SetActive(
                NiskalaProgress.bakulCollected
            );

            Button button = bakulIcon.GetComponent<Button>();

            if (button != null)
            {
                button.interactable =
                    NiskalaProgress.bakulCollected &&
                    storageController != null;
            }
        }

        if (catatanIcon != null)
        {
            catatanIcon.SetActive(
                NiskalaProgress.catatanCikSimaCollected
            );

            Button button = catatanIcon.GetComponent<Button>();

            if (button != null)
            {
                button.interactable =
                    NiskalaProgress.catatanCikSimaCollected &&
                    bedroomController != null;
            }
        }
    }

    private void SetupBakulButton()
    {
        if (bakulIcon == null)
            return;

        Button button = bakulIcon.GetComponent<Button>();

        if (button == null)
        {
            Debug.LogWarning(
                "Bakul_Inventory belum memiliki Button."
            );
            return;
        }

        button.interactable =
            NiskalaProgress.bakulCollected &&
            storageController != null;

        if (storageController != null)
        {
            button.onClick.AddListener(
                storageController.ClickBakulInventory
            );
        }
    }

    private void SetupCatatanButton()
    {
        if (catatanIcon == null)
            return;

        Button button = catatanIcon.GetComponent<Button>();

        if (button == null)
        {
            Debug.LogWarning(
                "Catatan_CikSima_Inventory belum memiliki Button."
            );
            return;
        }

        button.interactable =
            NiskalaProgress.catatanCikSimaCollected &&
            bedroomController != null;

        if (bedroomController != null)
        {
            button.onClick.AddListener(
                bedroomController.ClickCatatanCikSimaInventory
            );
        }
    }
}