
using System.Collections;
using UnityEngine;

public class BedroomSpecialInteractions : MonoBehaviour, IInteractionBlocker
{

    [Header("Bedroom Audio")]
    [SerializeField] private AudioSource mirrorJumpscareSource;
    [SerializeField] private AudioSource glassBreakSource;
    [SerializeField] private AudioSource footstepsSource;

    [Header("Cermin di Kamar")]
    [SerializeField] private GameObject mirrorRoom;
    [SerializeField] private GameObject mirrorBrokenRoom;

    [Header("Cermin Zoom")]
    [SerializeField] private GameObject mirrorZoom;
    [SerializeField] private GameObject mirrorZoomBroken;
    [SerializeField] private GameObject mirrorJumpscare;
    [SerializeField] private GameObject mirrorDimmer;

    [Header("Selendang")]
    [SerializeField] private GameObject scarfRoom;

    [Header("Common UI")]
    [SerializeField] private GameObject arrowBack;

    [Header("Pengaturan Waktu")]
    [SerializeField] private float jumpscareDelay = 2f;
    [SerializeField] private float jumpscareDuration = 1.65f;
    [SerializeField] private float fadeInDuration = 0.25f;
    [SerializeField] private float fadeOutDuration = 0.25f;
    [SerializeField] private float glassBreakDelay = 0.35f;
    [SerializeField] private float footstepsDelay = 0.1f;
    [SerializeField] private float mirrorDialogueDelay = 1f;

    [Header("Jumpscare Canvas Group")]
    [SerializeField] private CanvasGroup jumpscareCanvasGroup;

    [Header("Controller Lemari")]
    [SerializeField] private BedroomLockerController lockerController;

    [Header("Artifact Zoom")]
    [SerializeField] private ArtifactZoomController artifactZoomController;

    private bool scarfAlreadyClicked;
    private bool scarfSequenceActive;
    private bool mirrorClickedOnce;
    private bool mirrorIsBroken;
    private bool mirrorZoomOpen;
    private bool interactionBusy;

    private void OnDisable()
    {
        // Catat apakah urutan interaksi selendang terhenti.
        bool scarfWasInterrupted = scarfSequenceActive;

        // Hentikan coroutine milik script ini.
        StopAllCoroutines();

        // Akhiri event interaksi jika masih aktif.
        EndSpecialInteraction();

        // Izinkan selendang dicoba kembali jika urutannya terhenti.
        if (scarfWasInterrupted)
        {
            scarfAlreadyClicked = false;
        }

        scarfSequenceActive = false;

        // Bersihkan tampilan sementara.
        SetActiveSafe(mirrorJumpscare, false);
        SetActiveSafe(mirrorZoom, false);
        SetActiveSafe(mirrorZoomBroken, false);
        SetActiveSafe(mirrorDimmer, false);

        if (jumpscareCanvasGroup != null)
        {
            jumpscareCanvasGroup.alpha = 0f;
        }

        // Reset status zoom.
        mirrorZoomOpen = false;

        // Pulihkan tampilan objek sesuai kondisi terakhir.
        SetActiveSafe(mirrorRoom, !mirrorIsBroken);
        SetActiveSafe(mirrorBrokenRoom, mirrorIsBroken);
        SetActiveSafe(scarfRoom, true);

        SetArrowBack(true);

        Debug.Log(
            "BedroomSpecialInteractions: OnDisable dipanggil. " +
            "Coroutine dihentikan dan status interaksi dibersihkan."
        );
    }

    private void Start()
    {
        mirrorClickedOnce =
            NiskalaProgress.mirrorClickedOnce;

        mirrorIsBroken =
            NiskalaProgress.mirrorIsBroken;

        scarfAlreadyClicked =
            NiskalaProgress.scarfAlreadyClicked;

        SetActiveSafe(mirrorRoom, !mirrorIsBroken);
        SetActiveSafe(mirrorBrokenRoom, mirrorIsBroken);

        SetActiveSafe(mirrorZoom, false);
        SetActiveSafe(mirrorZoomBroken, false);
        SetActiveSafe(mirrorJumpscare, false);
        SetActiveSafe(mirrorDimmer, false);

        if (jumpscareCanvasGroup != null)
            jumpscareCanvasGroup.alpha = 0f;

        SetActiveSafe(scarfRoom, true);

        SetArrowBack(true);
    }

    // =========================================
    // CERMIN TUA
    // =========================================

    public void ClickMirror()
    {
        if (IsInteractionBlocked())
            return;

        if (DialogueSystem.Instance != null &&
            DialogueSystem.Instance.IsDialogueActive)
            return;

        mirrorZoomOpen = true;

        // Sembunyikan cermin di kamar saat zoom dibuka.
        SetActiveSafe(mirrorRoom, false);
        SetActiveSafe(mirrorBrokenRoom, false);

        SetActiveSafe(mirrorJumpscare, false);

        // Tampilkan dimmer di belakang zoom cermin.
        SetActiveSafe(mirrorDimmer, true);

        // Jika cermin sudah pecah, langsung tampilkan zoom pecah.
        if (mirrorIsBroken)
        {
            SetActiveSafe(mirrorZoom, false);
            SetActiveSafe(mirrorZoomBroken, true);

            SetArrowBack(true);

            Debug.Log("Cermin dibuka dalam kondisi pecah.");
            return;
        }

        // Tampilkan Zoom cermin normal.
        SetActiveSafe(mirrorZoom, true);
        SetActiveSafe(mirrorZoomBroken, false);

        BeginSpecialInteraction();

        // ArrowBack nonaktif selama seluruh rangkaian event.
        SetArrowBack(false);

        if (!mirrorClickedOnce)
        {
            mirrorClickedOnce = true;
            NiskalaProgress.mirrorClickedOnce = true;

            StartCoroutine(FirstMirrorSequence());
        }
        else
        {
            // Jika progres interaksi pernah terputus sebelum cermin pecah,
            // lanjutkan rangkaian hingga cermin menjadi pecah.
            StartCoroutine(BreakMirrorSequence());
        }
    }

    private IEnumerator FirstMirrorSequence()
    {
        // Tunggu sebelum jumpscare muncul.
        yield return new WaitForSeconds(jumpscareDelay);

        if (mirrorJumpscare != null)
            mirrorJumpscare.SetActive(true);

        // Putar SFX jumpscare.
        if (mirrorJumpscareSource != null)
            mirrorJumpscareSource.Play();

        if (jumpscareCanvasGroup != null)
            jumpscareCanvasGroup.alpha = 0f;

        Debug.Log("Cermin: jumpscare mulai fade in.");

        // Fade in: transparan menjadi terlihat penuh.
        yield return StartCoroutine(
            FadeJumpscare(0f, 1f, fadeInDuration)
        );

        // Tahan jumpscare.
        yield return new WaitForSeconds(jumpscareDuration);

        // Fade out: terlihat penuh menjadi transparan.
        yield return StartCoroutine(
            FadeJumpscare(1f, 0f, fadeOutDuration)
        );

        SetActiveSafe(mirrorJumpscare, false);

        // Jeda sebelum dialog pertama muncul.
        yield return new WaitForSeconds(mirrorDialogueDelay);

        // Dialog pertama.
        if (StartDialogue("Bayangan perempuan?"))
        {
            yield return WaitForDialogueToFinish();
        }
        else
        {
            Debug.LogWarning(
                "Dialog pertama cermin tidak berhasil dimulai."
            );
        }

        // Jeda 0.5 detik setelah dialog pertama selesai.
        yield return new WaitForSeconds(0.5f);

        // Putar SFX kaca pecah.
        if (glassBreakSource != null)
            glassBreakSource.Play();

        // Simpan status cermin pecah.
        mirrorIsBroken = true;
        NiskalaProgress.mirrorIsBroken = true;

        // Ganti visual cermin normal menjadi cermin pecah.
        SetActiveSafe(mirrorZoom, false);
        SetActiveSafe(mirrorZoomBroken, true);

        Debug.Log("Cermin berubah menjadi Cermin_Zoom_Broken.");

        // Jeda sebelum dialog setelah cermin pecah.
        yield return new WaitForSeconds(mirrorDialogueDelay);

        // Dialog setelah cermin pecah.
        if (StartDialogue("Aku tidak bisa berkata-kata"))
        {
            yield return WaitForDialogueToFinish();
        }
        else
        {
            Debug.LogWarning(
                "Dialog cermin pecah tidak berhasil dimulai."
            );
        }

        // Interaksi baru berakhir setelah dialog kedua selesai.
        EndSpecialInteraction();

        // ArrowBack baru aktif setelah seluruh event selesai.
        SetArrowBack(true);

        Debug.Log("Interaksi cermin selesai. Cermin sudah pecah.");
    }

    private IEnumerator FadeJumpscare(
    float startAlpha,
    float targetAlpha,
    float duration)
    {
        if (jumpscareCanvasGroup == null)
        {
            Debug.LogWarning(
                "CanvasGroup jumpscare belum dihubungkan di Inspector."
            );

            yield break;
        }

        jumpscareCanvasGroup.alpha = startAlpha;

        if (duration <= 0f)
        {
            jumpscareCanvasGroup.alpha = targetAlpha;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);

            jumpscareCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                progress
            );

            yield return null;
        }

        jumpscareCanvasGroup.alpha = targetAlpha;
    }

    private IEnumerator BreakMirrorSequence()
    {
        // Cermin utuh terlihat sebentar sebelum pecah.
        yield return new WaitForSeconds(glassBreakDelay);

        // Putar SFX kaca pecah.
        if (glassBreakSource != null)
            glassBreakSource.Play();

        mirrorIsBroken = true;
        NiskalaProgress.mirrorIsBroken = true;

        SetActiveSafe(mirrorZoom, false);
        SetActiveSafe(mirrorZoomBroken, true);

        if (StartDialogue("Aku tidak bisa berkata-kata"))
        {
            yield return WaitForDialogueToFinish();
        }
        else
        {
            Debug.LogWarning(
                "Dialog cermin pecah tidak berhasil dimulai."
            );
        }

        EndSpecialInteraction();
        SetArrowBack(true);

        Debug.Log("Cermin sekarang tersimpan dalam kondisi pecah.");
    }

    // =========================================
    // SELENDANG
    // =========================================

    public void ClickScarf()
    {
        if (IsInteractionBlocked())
            return;

        if (scarfAlreadyClicked)
            return;

        scarfAlreadyClicked = true;
        NiskalaProgress.scarfAlreadyClicked = true;

        scarfSequenceActive = true;

        BeginSpecialInteraction();

        // Sembunyikan ArrowBack selama event.
        SetArrowBack(false);

        Debug.Log("Selendang diklik.");

        StartCoroutine(ScarfSequence());
    }

    private IEnumerator ScarfSequence()
    {
        Debug.Log("Selendang diperiksa.");

        /// Tunggu sebelum suara langkah kaki.
        yield return new WaitForSeconds(footstepsDelay);

        // Putar SFX langkah kaki.
        if (footstepsSource != null)
            footstepsSource.Play();

        {
            footstepsSource.Play();

            // Tunggu sampai audio footsteps selesai.
            yield return new WaitWhile(
                () => footstepsSource.isPlaying
            );
        }

        // Mulai dialog setelah suara langkah kaki.
        if (StartDialogue("Siapa di sana?"))
        {
            // Tunggu sampai dialog benar-benar selesai.
            yield return WaitForDialogueToFinish();
        }
        else
        {
            Debug.LogWarning(
                "Dialog selendang tidak berhasil dimulai."
            );
        }

        scarfSequenceActive = false;

        EndSpecialInteraction();

        // Tampilkan kembali ArrowBack setelah dialog selesai.
        SetArrowBack(true);

        Debug.Log("Interaksi selendang selesai.");
    }

    // =========================================
    // ARROW BACK
    // =========================================

    public bool TryCloseSpecialZoom()
    {
        if (!mirrorZoomOpen)
            return false;

        // Jangan tutup zoom selama jumpscare atau dialog.
        if (interactionBusy)
        {
            Debug.Log("Selesaikan interaksi terlebih dahulu.");
            return true;
        }

        mirrorZoomOpen = false;

        SetActiveSafe(mirrorZoom, false);
        SetActiveSafe(mirrorZoomBroken, false);
        SetActiveSafe(mirrorJumpscare, false);
        SetActiveSafe(mirrorDimmer, false);

        // Tampilkan kembali gambar cermin yang sesuai.
        SetActiveSafe(mirrorRoom, !mirrorIsBroken);
        SetActiveSafe(mirrorBrokenRoom, mirrorIsBroken);

        SetArrowBack(true);

        Debug.Log(mirrorIsBroken
            ? "Kembali ke kamar: cermin sudah pecah."
            : "Kembali ke kamar: cermin masih utuh.");

        return true;
    }

    // =========================================
    // DIALOG
    // =========================================

    private bool StartDialogue(string line)
    {
        if (DialogueSystem.Instance == null)
        {
            Debug.LogError(
                "DialogueSystem tidak ditemukan di scene."
            );
            return false;
        }

        bool started = DialogueSystem.Instance.StartDialogue(
            "Arka",
            line
        );

        if (!started)
        {
            Debug.LogWarning(
                "Dialog tidak dimulai. Periksa apakah dialog lain aktif."
            );
        }

        return started;
    }

    private IEnumerator WaitForDialogueToFinish()
    {
        while (DialogueSystem.Instance != null &&
               DialogueSystem.Instance.IsDialogueActive)
        {
            yield return null;
        }
    }

    // =========================================
    // BANTUAN
    // =========================================

    private void BeginSpecialInteraction()
    {
        if (interactionBusy)
            return;

        interactionBusy = true;
        GameEvents.RaiseInteractionStarted();

        Debug.Log("BedroomSpecialInteractions: Interaksi dimulai.");
    }

    private void EndSpecialInteraction()
    {
        if (!interactionBusy)
            return;

        interactionBusy = false;
        GameEvents.RaiseInteractionFinished();

        Debug.Log("BedroomSpecialInteractions: Interaksi selesai.");
    }

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

    public bool IsInteractionBlocked()
    {
        if (artifactZoomController != null &&
            artifactZoomController.IsArtifactZoomOpen)
        {
            return true;
        }

        // Event atau dialog khusus masih berjalan.
        if (interactionBusy)
            return true;

        // Zoom cermin sedang terbuka.
        if (mirrorZoomOpen)
            return true;

        // Dialog dari sistem dialog bersama sedang berlangsung.
        if (DialogueSystem.Instance != null &&
            DialogueSystem.Instance.IsDialogueActive)
            return true;

        // Lemari atau zoom lemari sedang terbuka.
        if (lockerController != null &&
            lockerController.IsLockerViewOpen())
            return true;

        return false;
    }
}
