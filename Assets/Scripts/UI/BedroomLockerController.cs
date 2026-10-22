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
    [SerializeField] private GameObject catatanCikSimaZoom;

    [Header("Inventory Catatan")]
    [SerializeField] private GameObject catatanCikSimaInventory;

    [Header("Investigasi Artefak")]
    [SerializeField] private GameObject buttonInvestigasiArtefak;

    [Header("Common UI")]
    [SerializeField] private GameObject arrowBack;

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

    [Header("Interaksi Tambahan Kamar")]
    [SerializeField] private BedroomSpecialInteractions specialInteractions;

    private readonly int[] correctCode = { 7, 3, 1, 8 };

    private int[] currentCode = { 1, 1, 1, 1 };

    private bool lockerDialogueShown = false;
    private bool lockerDialoguePending = false;
    private bool lockerOpen = false;
    private bool lockerUnlocked = false;
    private bool zoomOpen = false;
    private bool catatanCollected = false;

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

        if (catatanCikSimaZoom != null)
            catatanCikSimaZoom.SetActive(false);

        if (catatanCikSimaInventory != null)
            catatanCikSimaInventory.SetActive(false);

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
        // Jika catatan sudah diambil, lemari tidak bisa dibuka lagi.
        if (catatanCollected)
            return;

        // Jangan membuka lemari jika interaksi objek lain sedang berjalan.
        if (specialInteractions != null &&
            specialInteractions.IsInteractionBlocked())
            return;

        if (lockerUnlocked)
        {
            OpenLemariZoom();
            return;
        }

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

        if (lockerPanel != null)
            lockerPanel.SetActive(true);

        if (lemariClosed != null)
            lemariClosed.SetActive(true);

        if (lemariOpened != null)
            lemariOpened.SetActive(false);

        if (lemariZoom != null)
            lemariZoom.SetActive(false);

        if (catatanCikSima != null)
            catatanCikSima.SetActive(false);

        if (lockerDimmer != null)
            lockerDimmer.SetActive(true);

        SetViewButtons(false);

        if (!lockerDialogueShown)
        {
            lockerDialogueShown = true;

            SetArrowBack(false);
            SetDigitButtons(false);

            ShowCurrentDialogue();
        }
        else
        {
            SetArrowBack(true);
            SetDigitButtons(true);
        }
    }

    // =========================================================
    // DIALOG
    // =========================================================

    private void ShowCurrentDialogue()
    {
        if (DialogueSystem.Instance == null)
        {
            Debug.LogError(
                "DialogueSystem tidak ditemukan di scene KamarTidur."
            );

            lockerDialoguePending = false;
            SetArrowBack(true);
            SetDigitButtons(true);
            return;
        }

        lockerDialoguePending = true;

        bool started = DialogueSystem.Instance.StartDialogue(
            "Arka",
            lockerDialogue
        );

        if (!started)
        {
            lockerDialoguePending = false;

            Debug.LogWarning(
                "Dialog Lemari tidak dapat dimulai."
            );

            SetArrowBack(true);
            SetDigitButtons(true);
        }
    }

    private void OnEnable()
    {
        GameEvents.DialogueFinished += OnDialogueFinished;
    }

    private void OnDisable()
    {
        GameEvents.DialogueFinished -= OnDialogueFinished;
    }

    private void OnDialogueFinished()
    {
        if (!lockerOpen || !lockerDialoguePending)
            return;

        lockerDialoguePending = false;

        SetArrowBack(true);
        SetDigitButtons(true);

        Debug.Log(
            "Dialog lemari selesai. Tombol angka diaktifkan."
        );
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

        // Angka tidak boleh diubah selama dialog berlangsung.
        if (DialogueSystem.Instance != null &&
            DialogueSystem.Instance.IsDialogueActive)
            return;

        if (slotIndex < 0 || slotIndex >= currentCode.Length)
            return;

        currentCode[slotIndex] += direction;

        if (currentCode[slotIndex] > 9)
            currentCode[slotIndex] = 1;

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

    public void ClickCatatanCikSima()
    {
        // Catatan hanya bisa diambil saat zoom lemari terbuka.
        if (!zoomOpen)
            return;

        // Catatan hanya bisa diambil satu kali.
        if (catatanCollected)
            return;

        catatanCollected = true;

        // Sembunyikan catatan dari tampilan lemari.
        if (catatanCikSima != null)
            catatanCikSima.SetActive(false);

        // Simpan catatan ke inventory.
        if (catatanCikSimaInventory != null)
            catatanCikSimaInventory.SetActive(true);

        // Tutup tampilan lemari secara permanen
        // untuk sisa sesi permainan ini.
        zoomOpen = false;

        if (lemariZoom != null)
            lemariZoom.SetActive(false);

        if (catatanCikSima != null)
            catatanCikSima.SetActive(false);

        // Tampilkan kondisi lemari yang sudah terbuka.
        if (lemariClosed != null)
            lemariClosed.SetActive(false);

        if (lemariOpened != null)
            lemariOpened.SetActive(true);

        if (lockerDimmer != null)
            lockerDimmer.SetActive(false);

        // Tampilkan zoom catatan dan tombol investigasi.
        OpenCatatanCikSimaZoom();

        Debug.Log("Catatan Cik Sima masuk ke inventory.");
    }

    public void ClickCatatanCikSimaInventory()
    {
        // Catatan harus sudah diambil terlebih dahulu.
        if (!catatanCollected)
            return;

        // Jangan membuka zoom inventory jika zoom lemari
        // atau puzzle lemari masih terbuka.
        if (zoomOpen || lockerOpen)
            return;

        // Jangan membuka zoom saat dialog berlangsung.
        if (DialogueSystem.Instance != null &&
            DialogueSystem.Instance.IsDialogueActive)
            return;

        OpenCatatanCikSimaZoom();

        Debug.Log("Catatan Cik Sima di inventory diklik.");
    }

    private void OpenCatatanCikSimaZoom()
    {
        if (catatanCikSimaZoom != null)
        {
            catatanCikSimaZoom.SetActive(true);
        }
        else
        {
            Debug.LogError(
                "Catatan Cik Sima Zoom belum dihubungkan di Inspector."
            );
        }

        if (buttonInvestigasiArtefak != null)
        {
            buttonInvestigasiArtefak.SetActive(true);
        }
        else
        {
            Debug.LogError(
                "Button Investigasi Artefak belum dihubungkan di Inspector."
            );
        }

        SetArrowBack(true);
        SetViewButtons(false);
    }

    // =========================================================
    // ARROW BACK
    // =========================================================

    public void ClickArrowBack()
    {
        if (specialInteractions != null &&
            specialInteractions.TryCloseSpecialZoom())
        {
            return;
        }

        if (catatanCikSimaZoom != null &&
            catatanCikSimaZoom.activeSelf &&
            !zoomOpen)
        {
            catatanCikSimaZoom.SetActive(false);

            if (buttonInvestigasiArtefak != null)
                buttonInvestigasiArtefak.SetActive(false);

            SetArrowBack(true);
            SetViewButtons(true);
            return;
        }

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

        if (catatanCikSimaZoom != null)
            catatanCikSimaZoom.SetActive(false);

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

    public bool IsLockerViewOpen()
    {
        return lockerOpen ||
               zoomOpen ||
               (catatanCikSimaZoom != null &&
                catatanCikSimaZoom.activeSelf);
    }
}