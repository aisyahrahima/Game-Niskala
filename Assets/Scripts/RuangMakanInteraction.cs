
using System.Collections;
using UnityEngine;

public class RuangMakanInteraction : MonoBehaviour, IInteractionBlocker
{
    [Header("Dialog")]
    [SerializeField] private DialogueSystem dialogueSystem;

    [Header("Arrow Back")]
    [SerializeField] private GameObject arrowBack;

    [Header("Artifact Zoom")]
    [SerializeField] private ArtifactZoomController artifactZoomController;

    [Header("Alat Makan")]
    [SerializeField] private GameObject alatMakan;
    [SerializeField] private GameObject alatMakanZoom;

    [Header("Gelas")]
    [SerializeField] private GameObject gelas;
    [SerializeField] private GameObject gelasZoom;

    [Header("Tempayan")]
    [SerializeField] private GameObject tempayan;
    [SerializeField] private GameObject tempayanZoom;

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

        SetActiveSafe(alatMakanZoom, false);
        SetActiveSafe(gelasZoom, false);
        SetActiveSafe(tempayanZoom, false);

        SetArrowBack(true);
    }

    // =========================================
    // ALAT MAKAN
    // =========================================

    public void ClickAlatMakan()
    {
        if (IsInteractionBlocked())
            return;

        OpenZoom(alatMakan, alatMakanZoom);

        if (!alatMakanClickedOnce)
        {
            alatMakanClickedOnce = true;
            NiskalaProgress.alatMakanClickedOnce = true;

            interactionBusy = true;

            BeginInteraction();

            Debug.Log("SFX: Beberapa perempuan berbicara dan tertawa samar.");

            StartCoroutine(PlayAlatMakanEvent());
        }
        else
        {
            SetArrowBack(true);
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

        OpenZoom(gelas, gelasZoom);

        if (!gelasClickedOnce)
        {
            gelasClickedOnce = true;
            NiskalaProgress.gelasClickedOnce = true;

            Debug.Log("SFX: Suara benda pecah.");
        }

        SetArrowBack(true);
    }

    // =========================================
    // TEMPAYAN
    // =========================================

    public void ClickTempayan()
    {
        if (IsInteractionBlocked())
            return;

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
        Debug.Log("SFX: Suara tetesan air.");

        // Dialog pertama.
        yield return new WaitForSeconds(2f);

        yield return StartDialogue("Kosong...");

        // Suara bisikan muncul setelah dialog pertama selesai.
        Debug.Log("SFX: Suara perempuan berbisik 'Air... haus...'");

        yield return new WaitForSeconds(1f);

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
        if (alatMakanZoom != null && alatMakanZoom.activeSelf)
        {
            alatMakanZoom.SetActive(false);
            SetActiveSafe(alatMakan, true);
        }

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
