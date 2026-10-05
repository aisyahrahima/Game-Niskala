using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BedroomLockerController : MonoBehaviour
{
    [Header("Lemari di Room")]
    [SerializeField] private GameObject lemariClosed;
    [SerializeField] private GameObject lemariOpened;

    [Header("Locker Dimmer")]
    [SerializeField] private GameObject lockerDimmer;

    [Header("Locker Puzzle")]
    [SerializeField] private GameObject lockerPanel;
    [SerializeField] private TMP_Text[] digitTexts;

    [Header("Lemari Zoom")]
    [SerializeField] private GameObject lemariZoom;
    [SerializeField] private GameObject catatanCikSima;

    [Header("Common UI")]
    [SerializeField] private GameObject arrowBack;

    [Header("Dialogue")]
    [SerializeField] private DialogueUI dialogueUI;

    [Header("View Buttons")]
    [SerializeField] private Button viewLeftButton;
    [SerializeField] private Button viewRightButton;

    [Header("Locker Digit Buttons")]
    [SerializeField] private Button upButton1;
    [SerializeField] private Button downButton1;
    [SerializeField] private Button upButton2;
    [SerializeField] private Button downButton2;
    [SerializeField] private Button upButton3;
    [SerializeField] private Button downButton3;
    [SerializeField] private Button upButton4;
    [SerializeField] private Button downButton4;

    private readonly int[] correctCode = { 7, 3, 1, 8 };

    private int[] currentCode = { 1, 1, 1, 1 };

    private bool lockerDialogueShown = false;
    private bool lockerOpen = false;
    private bool lockerUnlocked = false;
    private bool zoomOpen = false;

    private int dialogueIndex = 0;

    private readonly string[] lockerDialogue =
    {
        "Terkunci...",
        "Aku harus mencari petunjuk untuk membukanya"
    };

    private void Awake()
    {
        SetInitialState();
    }

    private void SetInitialState()
    {
        if (lemariClosed != null)
            lemariClosed.SetActive(true);

        if (lemariOpened != null)
            lemariOpened.SetActive(false);

        if (lockerPanel != null)
            lockerPanel.SetActive(false);

        if (lemariZoom != null)
            lemariZoom.SetActive(false);

        if (catatanCikSima != null)
            catatanCikSima.SetActive(false);

        if (lockerDimmer != null)
            lockerDimmer.SetActive(false);

        SetArrowBack(true);
        SetViewButtons(true);
        SetDigitButtons(true);

        UpdateDigitDisplay();
    }

    // =========================================================
    // KLIK LEMARI
    // =========================================================

    public void ClickLemari()
    {
        // Kalau sudah terbuka, langsung buka zoom.
        if (lockerUnlocked)
        {
            OpenLemariZoom();
            return;
        }

        // Kalau locker sedang terbuka, jangan lakukan apa-apa.
        if (lockerOpen)
            return;

        OpenLocker();
    }

    // =========================================================
    // BUKA LOCKER
    // =========================================================

    private void OpenLocker()
    {
        lockerOpen = true;

        if (lockerPanel != null) lockerPanel.SetActive(true);
        if (lemariClosed != null) lemariClosed.SetActive(true);
        if (lemariOpened != null) lemariOpened.SetActive(false);
        if (lemariZoom != null) lemariZoom.SetActive(false);
        if (catatanCikSima != null) catatanCikSima.SetActive(false);

        if (lockerDimmer != null)
            lockerDimmer.SetActive(true);

        SetViewButtons(false);

        if (!lockerDialogueShown)
        {
            lockerDialogueShown = true;
            dialogueIndex = 0;

            // ArrowBack dan tombol angka disembunyikan/dimatikan selama dialog
            SetArrowBack(false);
            SetDigitButtons(false);

            ShowCurrentDialogue();
        }
        else
        {
            // Dialog sudah pernah selesai,
            // jadi tombol angka dan ArrowBack boleh digunakan
            SetArrowBack(true);
            SetDigitButtons(true);
        }
    }

    // =========================================================
    // DIALOG
    // =========================================================

    private void ShowCurrentDialogue()
    {
        Debug.Log("BEDROOM: ShowCurrentDialogue dipanggil.");

        if (dialogueUI == null)
        {
            Debug.LogError(
                "BEDROOM: DialogueUI masih NULL!"
            );

            return;
        }

        Debug.Log(
            "BEDROOM: DialogueUI ditemukan pada object: " +
            dialogueUI.gameObject.name
        );

        Debug.Log(
            "BEDROOM: Mengirim dialog: " +
            lockerDialogue[dialogueIndex]
        );

        dialogueUI.ShowLine(
            "Arka",
            lockerDialogue[dialogueIndex]
        );
    }

    public void NextLockerDialogue()
    {
        if (!lockerOpen) return;

        if (dialogueUI != null && dialogueUI.IsTyping)
        {
            dialogueUI.TryCompleteTyping();
            return;
        }

        dialogueIndex++;

        if (dialogueIndex < lockerDialogue.Length)
        {
            ShowCurrentDialogue();
            return;
        }

        if (dialogueUI != null)
            dialogueUI.Hide();

        // Dialog selesai, ArrowBack boleh digunakan lagi
        SetArrowBack(true);
        SetDigitButtons(true);
    }

    // =========================================================
    // PUTAR ANGKA
    // =========================================================

    public void ChangeDigit(int slotIndex, int direction)
    {
        if (!lockerOpen)
            return;

        if (lockerUnlocked)
            return;

        if (slotIndex < 0 || slotIndex >= currentCode.Length)
            return;

        currentCode[slotIndex] += direction;

        // Kalau lebih dari 9, kembali ke 1.
        if (currentCode[slotIndex] > 9)
            currentCode[slotIndex] = 1;

        // Kalau kurang dari 1, kembali ke 9.
        if (currentCode[slotIndex] < 1)
            currentCode[slotIndex] = 9;

        UpdateDigitDisplay();

        CheckCode();
    }

    public void UpSlot1()
    {
        ChangeDigit(0, 1);
    }

    public void DownSlot1()
    {
        ChangeDigit(0, -1);
    }

    public void UpSlot2()
    {
        ChangeDigit(1, 1);
    }

    public void DownSlot2()
    {
        ChangeDigit(1, -1);
    }

    public void UpSlot3()
    {
        ChangeDigit(2, 1);
    }

    public void DownSlot3()
    {
        ChangeDigit(2, -1);
    }

    public void UpSlot4()
    {
        ChangeDigit(3, 1);
    }

    public void DownSlot4()
    {
        ChangeDigit(3, -1);
    }

    // =========================================================
    // UPDATE ANGKA
    // =========================================================

    private void UpdateDigitDisplay()
    {
        if (digitTexts == null)
            return;

        for (int i = 0; i < digitTexts.Length; i++)
        {
            if (digitTexts[i] == null)
                continue;

            if (i < currentCode.Length)
                digitTexts[i].text = currentCode[i].ToString();
        }
    }

    // =========================================================
    // CEK KODE
    // =========================================================

    private void CheckCode()
    {
        for (int i = 0; i < correctCode.Length; i++)
        {
            if (currentCode[i] != correctCode[i])
                return;
        }

        // Kalau sampai sini berarti 7318.
        UnlockLocker();
    }

    // =========================================================
    // LOCKER TERBUKA
    // =========================================================

    private void UnlockLocker()
    {
        lockerUnlocked = true;
        lockerOpen = false;

        if (dialogueUI != null)
            dialogueUI.Hide();

        if (lockerPanel != null)
            lockerPanel.SetActive(false);

        SetDigitButtons(false);

        OpenLemariZoom();
    }

    // =========================================================
    // BUKA LEMARI ZOOM
    // =========================================================

    private void OpenLemariZoom()
    {
        zoomOpen = true;

        if (lockerPanel != null)
            lockerPanel.SetActive(false);

        if (lemariClosed != null)
            lemariClosed.SetActive(false);

        if (lemariOpened != null)
            lemariOpened.SetActive(false);

        if (lemariZoom != null)
            lemariZoom.SetActive(true);

        if (catatanCikSima != null)
            catatanCikSima.SetActive(true);

        if (lockerDimmer != null)
            lockerDimmer.SetActive(true);

        SetArrowBack(true);
        SetViewButtons(false);
    }

    // =========================================================
    // ARROW BACK
    // =========================================================

    public void ClickArrowBack()
    {
        // Prioritas 1:
        // kalau Lemari Zoom terbuka, tutup zoom.
        if (zoomOpen)
        {
            CloseLemariZoom();
            return;
        }

        // Prioritas 2:
        // kalau Locker sedang terbuka, tutup Locker.
        if (lockerOpen)
        {
            CloseLocker();
            return;
        }

        // Prioritas 3:
        // kalau tidak ada tampilan khusus,
        // kembali ke RuangTamu.
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene("RuangTamu");
        }
        else
        {
            Debug.LogError(
                "SceneTransitionManager tidak ditemukan."
            );
        }
    }

    // =========================================================
    // TUTUP LOCKER
    // =========================================================

    private void CloseLocker()
    {
        lockerOpen = false;

        if (lockerPanel != null)
            lockerPanel.SetActive(false);

        if (dialogueUI != null)
            dialogueUI.Hide();

        if (lockerDimmer != null)
            lockerDimmer.SetActive(false);

        SetArrowBack(true);
        SetViewButtons(true);
        SetDigitButtons(true);

        // Kode TIDAK di-reset.
        // currentCode tetap menyimpan angka terakhir.
    }

    // =========================================================
    // TUTUP LEMARI ZOOM
    // =========================================================

    private void CloseLemariZoom()
    {
        zoomOpen = false;

        if (lemariZoom != null)
            lemariZoom.SetActive(false);

        if (catatanCikSima != null)
            catatanCikSima.SetActive(false);

        if (lemariClosed != null)
            lemariClosed.SetActive(false);

        if (lemariOpened != null)
            lemariOpened.SetActive(true);

        if (lockerDimmer != null)
            lockerDimmer.SetActive(false);

        SetArrowBack(true);
        SetViewButtons(true);
    }

    // =========================================================
    // ARROW BACK
    // =========================================================

    private void SetArrowBack(bool enabled)
    {
        if (arrowBack != null)
            arrowBack.SetActive(enabled);
    }

    // =========================================================
    // VIEW BUTTON
    // =========================================================

    private void SetViewButtons(bool enabled)
    {
        if (viewLeftButton != null)
            viewLeftButton.interactable = enabled;

        if (viewRightButton != null)
            viewRightButton.interactable = enabled;
    }

    private void SetDigitButtons(bool enabled)
    {
        if (upButton1 != null)
            upButton1.interactable = enabled;

        if (downButton1 != null)
            downButton1.interactable = enabled;

        if (upButton2 != null)
            upButton2.interactable = enabled;

        if (downButton2 != null)
            downButton2.interactable = enabled;

        if (upButton3 != null)
            upButton3.interactable = enabled;

        if (downButton3 != null)
            downButton3.interactable = enabled;

        if (upButton4 != null)
            upButton4.interactable = enabled;

        if (downButton4 != null)
            downButton4.interactable = enabled;
    }
}