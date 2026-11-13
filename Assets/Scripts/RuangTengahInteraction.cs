using System.Collections;
using UnityEngine;

public class RuangTengahInteraction : MonoBehaviour, IInteractionBlocker
{
    [Header("Dialog")]
    [SerializeField] private DialogueSystem dialogueSystem;

    [Header("Arrow Back")]
    [SerializeField] private GameObject arrowBack;

    [Header("Artifact Zoom")]
    [SerializeField] private ArtifactZoomController artifactZoomController;

    [Header("Pengaturan Perpindahan Ruangan")]
    [SerializeField] private string previousSceneName = "RuangMakan";

    [Header("Patung")]
    [SerializeField] private GameObject patung;
    [SerializeField] private GameObject patungZoom;

    [Header("Kain")]
    [SerializeField] private GameObject kain;
    [SerializeField] private GameObject kainZoom;

    [Header("Tepak Sirih")]
    [SerializeField] private GameObject tepakSirih;
    [SerializeField] private GameObject messyTepakSirihZoom;
    [SerializeField] private GameObject tepakSirihZoom;
    [SerializeField] private GameObject tepakSirihInventory;
    [SerializeField] private GameObject buttonInvestigasiArtefak;
    [SerializeField] private InventoryUIController inventoryUIController;
    [SerializeField] private TepakSirihPuzzle tepakSirihPuzzle;

    [Header("Pengaturan Event")]
    [SerializeField] private float eventDelay = 1f;

    private bool interactionBusy;
    private bool zoomOpen;

    private bool patungClickedOnce;
    private bool kainClickedOnce;

    private void Awake()
    {
        // Pastikan seluruh tampilan Zoom tertutup saat scene dibuka.
        SetActiveSafe(patungZoom, false);
        SetActiveSafe(kainZoom, false);

        // Pulihkan status interaksi sebelumnya.
        patungClickedOnce = NiskalaProgress.patungClickedOnce;
        kainClickedOnce = NiskalaProgress.kainClickedOnce;

        // ArrowBack ditampilkan saat pertama masuk ruangan.
        SetArrowBack(true);

        if (NiskalaProgress.tepakSirihCollected)
        {
            SetActiveSafe(tepakSirih, false);
            SetActiveSafe(messyTepakSirihZoom, false);
            SetActiveSafe(tepakSirihZoom, false);
        }
        else
        {
            SetActiveSafe(tepakSirih, true);
            SetActiveSafe(messyTepakSirihZoom, false);
            SetActiveSafe(tepakSirihZoom, false);
        }
    }

    // =========================================
    // PATUNG
    // =========================================

    public void ClickPatung()
    {
        if (IsInteractionBlocked())
            return;

        bool firstClick = !patungClickedOnce;

        // Buka tampilan Zoom Patung.
        if (!OpenZoom(patung, patungZoom))
            return;

        if (firstClick)
        {
            // Simpan status bahwa Patung sudah pernah diklik.
            patungClickedOnce = true;
            NiskalaProgress.patungClickedOnce = true;

            interactionBusy = true;
            BeginInteraction();

            StartCoroutine(PlayPatungEvent());
        }
        else
        {
            // Jika bukan klik pertama, pemain langsung dapat
            // melihat Patung_Zoom tanpa mengulang dialog.
            SetArrowBack(true);
        }
    }

    private IEnumerator PlayPatungEvent()
    {
        // Jeda singkat sebelum dialog dimulai.
        yield return new WaitForSeconds(eventDelay);

        yield return StartDialogue(
            "Patung seorang ratu...?",
            "Siapa dia?"
        );

        // Event selesai.
        interactionBusy = false;
        EndInteraction();

        // Tampilkan tombol untuk menutup Zoom.
        SetArrowBack(true);

        Debug.Log("Event Patung Ruang Tengah selesai.");
    }

    // =========================================
    // KAIN
    // =========================================

    public void ClickKain()
    {
        if (IsInteractionBlocked())
            return;

        bool firstClick = !kainClickedOnce;

        // Buka tampilan Zoom Kain.
        if (!OpenZoom(kain, kainZoom))
            return;

        if (firstClick)
        {
            // Simpan status bahwa Kain sudah pernah diklik.
            kainClickedOnce = true;
            NiskalaProgress.kainClickedOnce = true;

            interactionBusy = true;
            BeginInteraction();

            StartCoroutine(PlayKainEvent());
        }
        else
        {
            // Klik berikutnya hanya membuka Zoom.
            SetArrowBack(true);
        }
    }

    private IEnumerator PlayKainEvent()
    {
        // Jeda sebelum suara bisikan.
        yield return new WaitForSeconds(eventDelay);

        // Sementara suara masih menggunakan Debug.Log.
        Debug.Log("SFX: Suara bisikan perempuan.");

        // Jeda agar pemain merasakan suasana sebelum dialog.
        yield return new WaitForSeconds(eventDelay);

        yield return StartDialogue(
            "Siapa yang bicara?"
        );

        // Event selesai.
        interactionBusy = false;
        EndInteraction();

        // Tampilkan tombol untuk menutup Zoom.
        SetArrowBack(true);

        Debug.Log("Event Kain Ruang Tengah selesai.");
    }

    // =====================================================
    // TEPAK SIRIH
    // =====================================================

    public void ClickTepakSirih()
    {
        if (IsInteractionBlocked())
            return;

        if (NiskalaProgress.tepakSirihCollected)
        {
            OpenTepakSirihZoom();
            return;
        }

        if (NiskalaProgress.tepakSirihPuzzleCompleted)
        {
            OnTepakSirihPuzzleCompleted();
            return;
        }

        if (messyTepakSirihZoom == null ||
            tepakSirih == null ||
            tepakSirihPuzzle == null)
        {
            Debug.LogError(
                "Referensi Tepak Sirih belum lengkap."
            );
            return;
        }

        tepakSirih.SetActive(false);
        messyTepakSirihZoom.SetActive(true);

        zoomOpen = true;

        // Nonaktifkan drag selama dialog berlangsung.
        tepakSirihPuzzle.SetPuzzleInteractable(false);

        if (!NiskalaProgress.tepakSirihIntroShown)
        {
            NiskalaProgress.tepakSirihIntroShown = true;

            interactionBusy = true;

            BeginInteraction();

            SetArrowBack(false);

            StartCoroutine(PlayTepakSirihIntro());
        }
        else
        {
            // Jika dialog sudah pernah ditampilkan,
            // puzzle langsung dapat dimainkan.
            tepakSirihPuzzle.SetPuzzleInteractable(true);

            SetArrowBack(true);
        }

        Debug.Log("Puzzle Tepak Sirih dibuka.");
    }

    private IEnumerator PlayTepakSirihIntro()
    {
        yield return StartDialogue(
            "Tepak Sirih?",
            "Seharusnya aku bisa menyusunnya"
        );

        // Dialog sudah selesai.
        interactionBusy = false;

        EndInteraction();

        // Sekarang objek puzzle dapat di-drag.
        if (tepakSirihPuzzle != null)
        {
            tepakSirihPuzzle.SetPuzzleInteractable(true);
        }

        // Tampilkan kembali ArrowBack.
        SetArrowBack(true);

        Debug.Log("Dialog pembuka Tepak Sirih selesai.");
    }

    public void OnTepakSirihPuzzleCompleted()
    {
        NiskalaProgress.tepakSirihPuzzleCompleted = true;
        NiskalaProgress.tepakSirihCollected = true;

        if (messyTepakSirihZoom != null)
            messyTepakSirihZoom.SetActive(false);

        if (tepakSirih != null)
            tepakSirih.SetActive(false);

        if (tepakSirihZoom != null)
            tepakSirihZoom.SetActive(true);

        if (tepakSirihInventory != null)
            tepakSirihInventory.SetActive(true);

        if (buttonInvestigasiArtefak != null)
            buttonInvestigasiArtefak.SetActive(true);

        if (inventoryUIController != null)
            inventoryUIController.RefreshInventory();

        zoomOpen = true;

        Debug.Log("Tepak Sirih berhasil diperoleh dan masuk inventory.");
    }

    public void ClickTepakSirihInventory()
    {
        if (!NiskalaProgress.tepakSirihCollected)
            return;

        if (IsInteractionBlocked())
            return;

        OpenTepakSirihZoom();
    }

    private void OpenTepakSirihZoom()
    {
        if (tepakSirihZoom != null)
            tepakSirihZoom.SetActive(true);

        if (buttonInvestigasiArtefak != null)
            buttonInvestigasiArtefak.SetActive(true);

        zoomOpen = true;
        SetArrowBack(true);

        Debug.Log("Tepak Sirih dibuka dari inventory.");
    }

    // =========================================
    // ZOOM
    // =========================================

    private bool OpenZoom(
        GameObject originalObject,
        GameObject zoomObject)
    {
        if (originalObject == null || zoomObject == null)
        {
            Debug.LogError(
                "Objek asli atau objek Zoom belum dihubungkan."
            );

            return false;
        }

        // Sembunyikan objek asli.
        originalObject.SetActive(false);

        // Tampilkan objek Zoom.
        zoomObject.SetActive(true);

        zoomOpen = true;

        // ArrowBack disembunyikan selama event berlangsung.
        SetArrowBack(false);

        Debug.Log("Zoom dibuka: " + zoomObject.name);

        return true;
    }

    private void CloseAllZoom()
    {
        // Tutup Zoom Patung.
        if (patungZoom != null && patungZoom.activeSelf)
        {
            patungZoom.SetActive(false);
            SetActiveSafe(patung, true);
        }

        // Tutup Zoom Kain.
        if (kainZoom != null && kainZoom.activeSelf)
        {
            kainZoom.SetActive(false);
            SetActiveSafe(kain, true);
        }

        if (messyTepakSirihZoom != null &&
            messyTepakSirihZoom.activeSelf)
        {
            messyTepakSirihZoom.SetActive(false);

            if (!NiskalaProgress.tepakSirihCollected)
                SetActiveSafe(tepakSirih, true);
        }

        if (tepakSirihZoom != null &&
            tepakSirihZoom.activeSelf)
        {
            tepakSirihZoom.SetActive(false);

            // Sembunyikan tombol investigasi
            // ketika tampilan Tepak Sirih ditutup.
            SetActiveSafe(buttonInvestigasiArtefak, false);
        }

        zoomOpen = false;

        // ArrowBack kembali ditampilkan.
        SetArrowBack(true);

        Debug.Log("Zoom Ruang Tengah ditutup.");
    }

    // =========================================
    // ARROW BACK
    // =========================================

    public void ClickArrowBack()
    {
        // Tidak dapat menekan ArrowBack saat event berlangsung.
        if (interactionBusy)
            return;

        // Tutup Zoom artefak inventory terlebih dahulu.
        if (artifactZoomController != null &&
            artifactZoomController.TryCloseArtifactZoom())
        {
            // Pastikan status Zoom Ruang Tengah ikut diperbarui.
            zoomOpen = false;

            // ArrowBack tetap ditampilkan untuk kembali ke ruangan sebelumnya.
            SetArrowBack(true);

            return;
        }

        // Jika Zoom objek ruangan masih terbuka, tutup Zoom terlebih dahulu.
        if (zoomOpen)
        {
            CloseAllZoom();
            return;
        }

        // Jika tidak ada Zoom yang terbuka, kembali ke Ruang Makan.
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(
                previousSceneName
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
    // DIALOG
    // =========================================

    private IEnumerator StartDialogue(params string[] lines)
    {
        if (dialogueSystem == null)
        {
            Debug.LogError(
                "DialogueSystem belum dihubungkan."
            );

            yield break;
        }

        bool started = dialogueSystem.StartDialogue(
            "Arka",
            lines
        );

        if (started)
            yield return WaitForDialogue();
    }

    private IEnumerator WaitForDialogue()
    {
        while (dialogueSystem != null &&
               dialogueSystem.IsDialogueActive)
        {
            yield return null;
        }
    }

    // =========================================
    // INTERACTION LOCK
    // =========================================

    private void BeginInteraction()
    {
        GameEvents.RaiseInteractionStarted();

        Debug.Log(
            "RuangTengahInteraction: Interaksi dimulai."
        );
    }

    private void EndInteraction()
    {
        GameEvents.RaiseInteractionFinished();

        Debug.Log(
            "RuangTengahInteraction: Interaksi selesai."
        );
    }

    public bool IsInteractionBlocked()
    {
        // Blokir saat event sedang berlangsung.
        if (interactionBusy)
            return true;

        // Blokir saat Zoom terbuka.
        if (zoomOpen)
            return true;

        // Blokir saat dialog masih berlangsung.
        if (dialogueSystem != null &&
            dialogueSystem.IsDialogueActive)
            return true;

        return false;
    }

    // =========================================
    // BANTUAN
    // =========================================

    private void SetArrowBack(bool visible)
    {
        if (arrowBack != null)
            arrowBack.SetActive(visible);
    }

    private void SetActiveSafe(GameObject target, bool state)
    {
        if (target != null)
            target.SetActive(state);
    }
}
