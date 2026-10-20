
using System.Collections;
using UnityEngine;

public class BedroomSpecialInteractions : MonoBehaviour
{
    [Header("Cermin di Kamar")]
    [SerializeField] private GameObject mirrorRoom;
    [SerializeField] private GameObject mirrorBrokenRoom;

    [Header("Cermin Zoom")]
    [SerializeField] private GameObject mirrorZoom;
    [SerializeField] private GameObject mirrorZoomBroken;
    [SerializeField] private GameObject mirrorJumpscare;

    [Header("Selendang")]
    [SerializeField] private GameObject scarfRoom;
    [SerializeField] private GameObject scarfZoom;

    [Header("Common UI")]
    [SerializeField] private GameObject arrowBack;

    [Header("Pengaturan Waktu")]
    [SerializeField] private float jumpscareDelay = 2f;
    [SerializeField] private float jumpscareDuration = 1.65f;
    [SerializeField] private float fadeInDuration = 0.25f;
    [SerializeField] private float fadeOutDuration = 0.25f;
    [SerializeField] private float glassBreakDelay = 0.35f;
    [SerializeField] private float footstepsDelay = 2f;

    [Header("Jumpscare Canvas Group")]
    [SerializeField] private CanvasGroup jumpscareCanvasGroup;

    [Header("Controller Lemari")]
    [SerializeField] private BedroomLockerController lockerController;

    private bool scarfAlreadyClicked;
    private bool mirrorClickedOnce;
    private bool mirrorIsBroken;
    private bool mirrorZoomOpen;
    private bool scarfZoomOpen;
    private bool interactionBusy;

    private void Start()
    {
        SetActiveSafe(mirrorRoom, true);
        SetActiveSafe(mirrorBrokenRoom, false);

        SetActiveSafe(mirrorZoom, false);
        SetActiveSafe(mirrorZoomBroken, false);
        SetActiveSafe(mirrorJumpscare, false);

        if (jumpscareCanvasGroup != null)
        {
            jumpscareCanvasGroup.alpha = 0f;
        }

        SetActiveSafe(scarfRoom, true);
        SetActiveSafe(scarfZoom, false);

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

        if (mirrorIsBroken)
        {
            SetActiveSafe(mirrorZoom, false);
            SetActiveSafe(mirrorZoomBroken, true);
            SetArrowBack(true);

            Debug.Log("Cermin dibuka dalam kondisi pecah.");
            return;
        }

        SetActiveSafe(mirrorZoom, true);
        SetActiveSafe(mirrorZoomBroken, false);

        interactionBusy = true;
        SetArrowBack(false);

        if (!mirrorClickedOnce)
        {
            mirrorClickedOnce = true;
            StartCoroutine(FirstMirrorSequence());
        }
        else
        {
            StartCoroutine(BreakMirrorSequence());
        }
    }

    private IEnumerator FirstMirrorSequence()
    {
        // Tunggu sebelum jumpscare muncul.
        yield return new WaitForSeconds(jumpscareDelay);

        if (mirrorJumpscare != null)
            mirrorJumpscare.SetActive(true);

        if (jumpscareCanvasGroup != null)
            jumpscareCanvasGroup.alpha = 0f;

        Debug.Log("Cermin: jumpscare mulai fade in.");

        // Fade in: transparan menjadi terlihat penuh.
        yield return StartCoroutine(
            FadeJumpscare(0f, 1f, fadeInDuration)
        );

        // Tahan jumpscare selama 1.65 detik.
        yield return new WaitForSeconds(jumpscareDuration);

        // Fade out: terlihat penuh menjadi transparan.
        yield return StartCoroutine(
            FadeJumpscare(1f, 0f, fadeOutDuration)
        );

        SetActiveSafe(mirrorJumpscare, false);

        StartDialogue("Bayangan perempuan?");

        yield return WaitForDialogueToFinish();

        interactionBusy = false;
        SetArrowBack(true);

        Debug.Log("Interaksi pertama cermin selesai.");
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

        mirrorIsBroken = true;

        SetActiveSafe(mirrorZoom, false);
        SetActiveSafe(mirrorZoomBroken, true);

        Debug.Log("SFX: suara kaca pecah (belum ada file audio).");

        StartDialogue("Aku tidak bisa berkata-kata");

        yield return WaitForDialogueToFinish();

        interactionBusy = false;
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
        interactionBusy = true;

        // Sembunyikan ArrowBack selama event.
        SetArrowBack(false);

        Debug.Log("Selendang diklik.");

        StartCoroutine(ScarfSequence());
    }

    private IEnumerator ScarfSequence()
    {
        Debug.Log("Selendang diperiksa.");

        // Tunggu sebelum suara langkah kaki.
        yield return new WaitForSeconds(footstepsDelay);

        Debug.Log("SFX: suara langkah kaki (belum ada file audio).");

        // Mulai dialog setelah suara langkah kaki.
        StartDialogue("Siapa di sana?");

        // Tunggu sampai dialog benar-benar selesai.
        yield return WaitForDialogueToFinish();

        interactionBusy = false;

        // Tampilkan kembali ArrowBack setelah dialog selesai.
        SetArrowBack(true);

        Debug.Log("Interaksi selendang selesai.");
    }

    // =========================================
    // ARROW BACK
    // =========================================

    public bool TryCloseSpecialZoom()
    {
        if (!mirrorZoomOpen && !scarfZoomOpen)
            return false;

        // Jangan tutup zoom selama jumpscare atau dialog.
        if (interactionBusy)
        {
            Debug.Log("Selesaikan interaksi terlebih dahulu.");
            return true;
        }

        if (mirrorZoomOpen)
        {
            mirrorZoomOpen = false;

            SetActiveSafe(mirrorZoom, false);
            SetActiveSafe(mirrorZoomBroken, false);
            SetActiveSafe(mirrorJumpscare, false);

            // Tampilkan kembali gambar cermin yang sesuai.
            SetActiveSafe(mirrorRoom, !mirrorIsBroken);
            SetActiveSafe(mirrorBrokenRoom, mirrorIsBroken);

            SetArrowBack(true);

            Debug.Log(mirrorIsBroken
                ? "Kembali ke kamar: cermin sudah pecah."
                : "Kembali ke kamar: cermin masih utuh.");

            return true;
        }

        if (scarfZoomOpen)
        {
            scarfZoomOpen = false;

            SetActiveSafe(scarfZoom, false);
            SetActiveSafe(scarfRoom, true);

            SetArrowBack(true);

            Debug.Log("Zoom selendang ditutup.");

            return true;
        }

        return false;
    }

    // =========================================
    // DIALOG
    // =========================================

    private void StartDialogue(string line)
    {
        if (DialogueSystem.Instance == null)
        {
            Debug.LogError(
                "DialogueSystem tidak ditemukan di scene."
            );
            return;
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
        // Event atau dialog khusus masih berjalan.
        if (interactionBusy)
            return true;

        // Zoom cermin atau selendang sedang terbuka.
        if (mirrorZoomOpen || scarfZoomOpen)
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
