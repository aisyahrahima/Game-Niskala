using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoreOrderPuzzleController : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private DialogueSystem dialogueSystem;

    [Header("Drag Items")]
    [SerializeField] private LoreOrderDragItem[] dragItems;

    [Header("Drop Zones")]
    [SerializeField] private LoreOrderDropZone[] dropZones;

    [Header("Button")]
    [SerializeField] private GameObject buttonPeriksa;

    [Header("Scene")]
    [SerializeField] private string fullLoreSceneName = "FullLore";

    private bool canInteract;
    private bool isChecking;

    private void Start()
    {
        // Puzzle tidak dapat digunakan di awal scene.
        SetPuzzleInteractable(false);

        // Tombol pemeriksaan disembunyikan.
        if (buttonPeriksa != null)
            buttonPeriksa.SetActive(false);

        StartCoroutine(OpeningDialogue());
    }

    // =========================================
    // DIALOG PEMBUKA
    // =========================================

    private IEnumerator OpeningDialogue()
    {
        // Jeda 0,5 detik sebelum dialog.
        yield return new WaitForSecondsRealtime(0.5f);

        // Tunggu DialogueSystem tersedia.
        while (dialogueSystem == null)
        {
            dialogueSystem = DialogueSystem.Instance;

            if (dialogueSystem != null)
                break;

            yield return null;
        }

        string[] lines =
        {
            "Sepertinya aku perlu menyusun semua petunjuk ini untuk memahami apa yang sebenarnya terjadi."
        };

        bool started = dialogueSystem.StartDialogue(
            "Arka",
            lines
        );

        if (!started)
        {
            Debug.LogError(
                "Dialog pembuka LoreOrder gagal dimulai."
            );

            yield break;
        }

        yield return WaitForDialogue();

        // Puzzle baru dapat digunakan setelah dialog selesai.
        SetPuzzleInteractable(true);

        Debug.Log("Puzzle LoreOrder sudah dapat dimainkan.");
    }

    // =========================================
    // PENGATURAN INTERAKSI
    // =========================================

    public bool CanInteract()
    {
        return canInteract && !isChecking;
    }

    public void SetPuzzleInteractable(bool state)
    {
        canInteract = state;

        if (dragItems == null)
            return;

        foreach (LoreOrderDragItem item in dragItems)
        {
            if (item != null)
                item.SetCanDrag(state);
        }
    }

    // =========================================
    // DRAG AND DROP
    // =========================================

    public void TryPlaceItem(
        LoreOrderDragItem item,
        LoreOrderDropZone zone)
    {
        if (!CanInteract() || item == null || zone == null)
            return;

        // Jika Drop Zone sudah ditempati teks lain,
        // jangan timpa objek tersebut.
        if (zone.CurrentItem != null &&
            zone.CurrentItem != item)
        {
            return;
        }

        bool placed = item.PlaceAt(zone);

        if (!placed)
            return;

        UpdateCheckButton();
    }

    // =========================================
    // BUTTON PERIKSA
    // =========================================

    private void UpdateCheckButton()
    {
        if (buttonPeriksa == null)
            return;

        bool allFilled = true;

        foreach (LoreOrderDropZone zone in dropZones)
        {
            if (zone == null || zone.CurrentItem == null)
            {
                allFilled = false;
                break;
            }
        }

        buttonPeriksa.SetActive(allFilled);
    }

    public void CheckPuzzle()
    {
        if (!CanInteract())
            return;

        if (!AreAllZonesFilled())
            return;

        StartCoroutine(CheckPuzzleProcess());
    }

    private IEnumerator CheckPuzzleProcess()
    {
        isChecking = true;

        SetPuzzleInteractable(false);

        if (buttonPeriksa != null)
            buttonPeriksa.SetActive(false);

        bool isCorrect = CheckCorrectOrder();

        if (isCorrect)
        {
            yield return HandleCorrectAnswer();
        }
        else
        {
            yield return HandleWrongAnswer();
        }

        isChecking = false;
    }

    // =========================================
    // CEK KELENGKAPAN DROP ZONE
    // =========================================

    private bool AreAllZonesFilled()
    {
        if (dropZones == null || dropZones.Length != 3)
            return false;

        foreach (LoreOrderDropZone zone in dropZones)
        {
            if (zone == null || zone.CurrentItem == null)
                return false;
        }

        return true;
    }

    // =========================================
    // CEK URUTAN BENAR
    // =========================================

    private bool CheckCorrectOrder()
    {
        for (int i = 0; i < dropZones.Length; i++)
        {
            if (dropZones[i].CurrentItem == null)
                return false;

            if (dropZones[i].CurrentItem.ItemID !=
                dropZones[i].ExpectedItemID)
            {
                return false;
            }
        }

        return true;
    }

    // =========================================
    // JAWABAN SALAH
    // =========================================

    private IEnumerator HandleWrongAnswer()
    {
        string[] lines =
        {
            "Sepertinya masih kurang tepat..."
        };

        bool started = dialogueSystem.StartDialogue(
            "Arka",
            lines
        );

        if (!started)
        {
            Debug.LogError(
                "Dialog jawaban salah gagal dimulai."
            );

            ResetPuzzle();
            SetPuzzleInteractable(true);
            yield break;
        }

        yield return WaitForDialogue();

        // Kembalikan ketiga teks ke posisi awal.
        ResetPuzzle();

        // Puzzle dapat dimainkan kembali.
        SetPuzzleInteractable(true);

        Debug.Log("Jawaban salah. Puzzle diulang.");
    }

    private void ResetPuzzle()
    {
        // Kosongkan seluruh Drop Zone.
        foreach (LoreOrderDropZone zone in dropZones)
        {
            if (zone != null)
                zone.SetCurrentItem(null);
        }

        // Kembalikan seluruh teks ke posisi awal.
        foreach (LoreOrderDragItem item in dragItems)
        {
            if (item != null)
                item.ResetToOriginalPosition();
        }

        // Sembunyikan tombol pemeriksaan.
        if (buttonPeriksa != null)
            buttonPeriksa.SetActive(false);
    }

    // =========================================
    // JAWABAN BENAR
    // =========================================

    private IEnumerator HandleCorrectAnswer()
    {
        string[] lines =
        {
            "Kurasa ini sudah sesuai.",
            "Sekarang aku tahu apa yang telah terjadi."
        };

        bool started = dialogueSystem.StartDialogue(
            "Arka",
            lines
        );

        if (!started)
        {
            Debug.LogError(
                "Dialog jawaban benar gagal dimulai."
            );

            SetPuzzleInteractable(true);
            yield break;
        }

        yield return WaitForDialogue();

        // Jeda 1 detik setelah dialog selesai.
        yield return new WaitForSecondsRealtime(1f);

        Debug.Log("Puzzle LoreOrder berhasil diselesaikan.");

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(
                fullLoreSceneName
            );
        }
        else
        {
            Debug.LogError(
                "SceneTransitionManager tidak ditemukan."
            );
        }
    }

    // =========================================
    // MENUNGGU DIALOG SELESAI
    // =========================================

    private IEnumerator WaitForDialogue()
    {
        while (dialogueSystem != null &&
               dialogueSystem.IsDialogueActive)
        {
            yield return null;
        }
    }
}