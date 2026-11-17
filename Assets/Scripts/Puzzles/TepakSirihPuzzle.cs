
using UnityEngine;

public class TepakSirihPuzzle : MonoBehaviour
{
    [Header("Ruang Tengah Interaction")]
    [SerializeField] private RuangTengahInteraction roomController;

    [Header("Drop Zones")]
    [SerializeField] private TepakSirihDropZone[] dropZones;

    private bool daunSirihPlaced;
    private bool pinangPlaced;
    private bool gambirPlaced;
    private bool tutupPlaced;

    private bool puzzleCompleted;
    private bool canInteract;

    private void Start()
    {
        puzzleCompleted = NiskalaProgress.tepakSirihPuzzleCompleted;

        SetPuzzleInteractable(false);

        if (puzzleCompleted)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetPuzzleInteractable(bool state)
    {
        canInteract = state;

        TepakSirihDragItem[] dragItems =
            GetComponentsInChildren<TepakSirihDragItem>(true);

        foreach (TepakSirihDragItem item in dragItems)
        {
            item.SetCanDrag(state);
        }
    }

    // =========================================
    // DETEKSI AREA DROP
    // =========================================

    public void TryPlaceByOverlap(
        TepakSirihDragItem item,
        RectTransform draggedRect)
    {
        if (!canInteract || puzzleCompleted || item == null)
            return;

        TepakSirihDropZone bestZone = null;
        float highestOverlap = 0f;

        foreach (TepakSirihDropZone zone in dropZones)
        {
            if (zone == null)
                continue;

            float overlapArea =
                zone.GetOverlapArea(draggedRect);

            if (overlapArea > highestOverlap)
            {
                highestOverlap = overlapArea;
                bestZone = zone;
            }
        }

        if (bestZone != null)
        {
            TryPlaceItem(
                item,
                bestZone,
                bestZone.TargetRect
            );
        }
    }

    // =========================================
    // PENEMPATAN OBJEK
    // =========================================

    public void TryPlaceItem(
        TepakSirihDragItem item,
        TepakSirihDropZone dropZone,
        RectTransform target)
    {
        if (!canInteract || puzzleCompleted ||
            item == null || dropZone == null)
            return;

        if (item.IsPlaced)
            return;

        // Pastikan objek sesuai dengan target.
        if (item.ItemID != dropZone.AcceptedItemID)
        {
            Debug.Log("Objek tidak sesuai dengan target.");
            return;
        }

        // Tutup hanya dapat dipasang setelah
        // ketiga bahan berhasil ditempatkan.
        if (item.ItemID == "Tepak_Sirih_Tutup")
        {
            if (!daunSirihPlaced ||
                !pinangPlaced ||
                !gambirPlaced)
            {
                Debug.Log(
                    "Tepak Sirih Tutup belum dapat dipasang. " +
                    "Letakkan ketiga bahan terlebih dahulu."
                );

                return;
            }
        }

        item.PlaceAt(target);

        UpdatePlacedStatus(item.ItemID);

        Debug.Log(item.ItemID + " berhasil ditempatkan.");

        if (daunSirihPlaced &&
            pinangPlaced &&
            gambirPlaced &&
            tutupPlaced)
        {
            CompletePuzzle();
        }
    }

    private void UpdatePlacedStatus(string itemID)
    {
        switch (itemID)
        {
            case "Daun_Sirih":
                daunSirihPlaced = true;
                break;

            case "Pinang":
                pinangPlaced = true;
                break;

            case "Gambir":
                gambirPlaced = true;
                break;

            case "Tepak_Sirih_Tutup":
                tutupPlaced = true;
                break;
        }
    }

    // =========================================
    // PENYELESAIAN PUZZLE
    // =========================================

    private void CompletePuzzle()
    {
        if (puzzleCompleted)
            return;

        if (!daunSirihPlaced ||
            !pinangPlaced ||
            !gambirPlaced ||
            !tutupPlaced)
            return;

        puzzleCompleted = true;

        SetPuzzleInteractable(false);

        NiskalaProgress.tepakSirihPuzzleCompleted = true;

        Debug.Log("Puzzle Tepak Sirih berhasil diselesaikan!");

        if (roomController != null)
        {
            roomController.OnTepakSirihPuzzleCompleted();
        }
        else
        {
            Debug.LogError(
                "RuangTengahInteraction belum dihubungkan."
            );
        }
    }
}
