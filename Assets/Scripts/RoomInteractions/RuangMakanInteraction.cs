
using System.Collections;
using UnityEngine;

public class RuangMakanInteraction : MonoBehaviour, IInteractionBlocker
{
    [Header("Ruang Makan Audio")]
    [SerializeField] private AudioSource alatMakanSource;
    [SerializeField] private AudioSource gelasSource;
    [SerializeField] private AudioSource tempayanSource;
    [SerializeField] private AudioSource bisikanTempayanSource;

    [Header("Dialog")]
    [SerializeField] private DialogueSystem dialogueSystem;

    [Header("Arrow Back")]
    [SerializeField] private GameObject arrowBack;

    [Header("Artifact Zoom")]
    [SerializeField] private ArtifactZoomController artifactZoomController;

    [Header("Alat Makan")]
    [SerializeField] private GameObject alatMakan;

    [Header("Gelas")]
    [SerializeField] private GameObject gelas;
    [SerializeField] private GameObject gelasZoom;
    [SerializeField] private GameObject gelasDimmer;
    [SerializeField] private float gelasFallDelay = 1.4f;
    [SerializeField] private float gelasFallDuration = 0.1f;
    [SerializeField] private float gelasFallDistance = 1200f;

    [Header("Tempayan")]
    [SerializeField] private GameObject tempayan;
    [SerializeField] private GameObject tempayanZoom;
    [SerializeField] private GameObject tempayanDimmer;

    [Header("Mayang Hide Spot")]
    [SerializeField] private MayangHideSpot mayangHideSpot;

    private bool interactionBusy;
    private bool zoomOpen;

    private bool alatMakanClickedOnce;
    private bool gelasClickedOnce;
    private bool tempayanClickedOnce;

    private void Awake()
    {
        alatMakanClickedOnce =
            NiskalaProgress.alatMakanClickedOnce;

        gelasClickedOnce =
            NiskalaProgress.gelasClickedOnce;

        tempayanClickedOnce =
            NiskalaProgress.tempayanClickedOnce;

        SetActiveSafe(gelasZoom, false);
        SetActiveSafe(tempayanZoom, false);

        SetActiveSafe(gelasDimmer, false);
        SetActiveSafe(tempayanDimmer, false);

        // Jika Gelas sudah pernah pecah,
        // sembunyikan Gelas asli secara permanen.
        if (gelasClickedOnce)
        {
            SetActiveSafe(gelas, false);
        }

        SetArrowBack(true);
    }

    // =========================================
    // ALAT MAKAN
    // =========================================

    public void ClickAlatMakan()
    {
        if (IsInteractionBlocked())
            return;

        if (!alatMakanClickedOnce)
        {
            alatMakanClickedOnce = true;
            NiskalaProgress.alatMakanClickedOnce = true;

            interactionBusy = true;

            BeginInteraction();

            SetArrowBack(false);

            if (alatMakanSource != null)
                alatMakanSource.Play();

            StartCoroutine(PlayAlatMakanEvent());
        }
    }

    private IEnumerator PlayAlatMakanEvent()
    {
        // Beri waktu untuk suara sebelum dialog.
        yield return new WaitForSeconds(2f);

        yield return StartDialogue(
            "Delapan orang...",
            "Jumlahnya sama dengan yang di lukisan..."
        );

        interactionBusy = false;
        EndInteraction();

        SetArrowBack(true);
    }

    // =========================================
    // GELAS
    // =========================================

    public void ClickGelas()
    {
        if (IsInteractionBlocked())
            return;

        if (gelasClickedOnce)
            return;

        gelasClickedOnce = true;
        NiskalaProgress.gelasClickedOnce = true;

        interactionBusy = true;

        BeginInteraction();

        SetActiveSafe(gelasDimmer, true);

        OpenZoom(gelas, gelasZoom);

        SetArrowBack(false);

        StartCoroutine(PlayGelasEvent());
        StartCoroutine(PlayGelasFall());
    }

    private IEnumerator PlayGelasEvent()
    {
        yield return new WaitForSeconds(1.5f);

        if (gelasSource != null)
            gelasSource.Play();
    }

    private IEnumerator PlayGelasFall()
    {
        // Tunggu 1 detik sebelum Gelas mulai jatuh.
        yield return new WaitForSeconds(gelasFallDelay);

        if (gelasZoom == null)
        {
            Debug.LogError("Gelas_Zoom belum dihubungkan.");
            yield break;
        }

        RectTransform gelasRect =
            gelasZoom.GetComponent<RectTransform>();

        if (gelasRect == null)
        {
            Debug.LogError(
                "Gelas_Zoom tidak memiliki RectTransform."
            );
            yield break;
        }

        Vector2 startPosition = gelasRect.anchoredPosition;

        Vector2 endPosition = startPosition +
            Vector2.down * gelasFallDistance;

        float elapsedTime = 0f;

        while (elapsedTime < gelasFallDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsedTime / gelasFallDuration
            );

            gelasRect.anchoredPosition = Vector2.Lerp(
                startPosition,
                endPosition,
                progress
            );

            yield return null;
        }

        gelasRect.anchoredPosition = endPosition;

        // Nonaktifkan Gelas_Zoom setelah jatuh.
        gelasZoom.SetActive(false);

        // Matikan Dimmer_Gelas.
        SetActiveSafe(gelasDimmer, false);

        // Zoom sudah selesai.
        zoomOpen = false;

        interactionBusy = false;

        EndInteraction();

        // Tampilkan ArrowBack setelah Gelas_Zoom menghilang.
        SetArrowBack(true);
    }

    // =========================================
    // TEMPAYAN
    // =========================================

    public void ClickTempayan()
    {
        if (IsInteractionBlocked())
            return;

        SetActiveSafe(tempayanDimmer, true);

        OpenZoom(tempayan, tempayanZoom);

        if (!tempayanClickedOnce)
        {
            tempayanClickedOnce = true;
            NiskalaProgress.tempayanClickedOnce = true;

            interactionBusy = true;

            BeginInteraction();

            StartCoroutine(PlayTempayanEvent());
        }
        else
        {
            SetArrowBack(true);
        }
    }

    private IEnumerator PlayTempayanEvent()
    {
        yield return new WaitForSeconds(1.5f);
        if (tempayanSource != null)
        {
            tempayanSource.Play();
        }
        // Dialog pertama.
        yield return new WaitForSeconds(2f);

        yield return StartDialogue("Kosong...");

        // Suara bisikan muncul setelah dialog pertama selesai.
        if (bisikanTempayanSource != null)
        {
            bisikanTempayanSource.Play();
        }

        yield return new WaitForSeconds(5f);

        // Dialog kedua.
        yield return StartDialogue(
            "...?",
            "Apakah benda ini untuk menyimpan air?"
        );

        interactionBusy = false;
        EndInteraction();

        SetArrowBack(true);
    }

    // =========================================
    // ZOOM DAN ARROW BACK
    // =========================================

    private void OpenZoom(
        GameObject originalObject,
        GameObject zoomObject)
    {
        if (originalObject == null || zoomObject == null)
        {
            Debug.LogError(
                "Objek asli atau objek zoom belum dihubungkan."
            );
            return;
        }

        originalObject.SetActive(false);
        zoomObject.SetActive(true);

        zoomOpen = true;

        SetArrowBack(false);

        Debug.Log("Zoom dibuka: " + zoomObject.name);
    }

    public void ClickArrowBack()
    {
        // Jika Arka sedang bersembunyi, keluar dari POV terlebih dahulu.
        if (mayangHideSpot != null && mayangHideSpot.IsHiding)
        {
            mayangHideSpot.ExitHideSpot();
            return;
        }

        if (interactionBusy)
            return;

        // Tutup zoom inventory terlebih dahulu.
        if (artifactZoomController != null &&
            artifactZoomController.TryCloseArtifactZoom())
        {
            return;
        }

        // Jika zoom sedang terbuka, tutup zoom terlebih dahulu.
        if (zoomOpen)
        {
            CloseAllZoom();

            Debug.Log("Zoom RuangMakan ditutup.");
            return;
        }

        // Jika tidak ada zoom, kembali ke RuangTamu.
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene("RuangTamu");
        }
        else
        {
            Debug.LogError("SceneTransitionManager tidak ditemukan.");
        }
    }

    private void CloseAllZoom()
    {
        if (gelasZoom != null && gelasZoom.activeSelf)
        {
            gelasZoom.SetActive(false);
            SetActiveSafe(gelas, true);
        }

        if (tempayanZoom != null && tempayanZoom.activeSelf)
        {
            tempayanZoom.SetActive(false);
            SetActiveSafe(tempayan, true);
        }

        // Matikan kedua Dimmer.
        SetActiveSafe(gelasDimmer, false);
        SetActiveSafe(tempayanDimmer, false);

        zoomOpen = false;

        // Tampilkan kembali ArrowBack setelah Zoom ditutup.
        SetArrowBack(true);
    }

    // =========================================
    // DIALOG
    // =========================================

    private IEnumerator StartDialogue(params string[] lines)
    {
        if (dialogueSystem == null)
        {
            Debug.LogError("DialogueSystem belum dihubungkan.");
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
    }

    private void EndInteraction()
    {
        GameEvents.RaiseInteractionFinished();
    }

    public bool IsInteractionBlocked()
    {
        // Blokir interaksi saat Zoom artefak terbuka.
        if (artifactZoomController != null &&
            artifactZoomController.IsArtifactZoomOpen)
        {
            return true;
        }

        if (interactionBusy || zoomOpen)
            return true;

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
