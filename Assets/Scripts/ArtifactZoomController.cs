using UnityEngine;

public class ArtifactZoomController : MonoBehaviour
{
    [Header("Artifact Zoom")]
    [SerializeField] private GameObject tepakSirihZoom;
    [SerializeField] private GameObject bakulZoom;
    [SerializeField] private GameObject catatanCikSimaZoom;

    [Header("UI")]
    [SerializeField] private GameObject buttonInvestigasiArtefak;
    [SerializeField] private GameObject arrowBack;

    private GameObject activeZoom;

    // Menyimpan kondisi ArrowBack sebelum zoom dibuka.
    private bool arrowBackPreviousState;

    private bool arrowBackStateSaved;

    public bool IsArtifactZoomOpen
    {
        get
        {
            // Periksa status objek zoom secara langsung.
            // Ini juga mengantisipasi zoom yang diaktifkan
            // oleh script lain.
            return IsAnyArtifactZoomActive();
        }
    }

    private void Awake()
    {
        SetActiveSafe(tepakSirihZoom, false);
        SetActiveSafe(bakulZoom, false);
        SetActiveSafe(catatanCikSimaZoom, false);

        SetActiveSafe(buttonInvestigasiArtefak, false);
    }

    // =========================================
    // BUKA ARTEFAK DARI INVENTORY
    // =========================================

    public void OpenTepakSirihZoom()
    {
        if (!NiskalaProgress.tepakSirihCollected)
            return;

        OpenArtifactZoom(tepakSirihZoom);
    }

    public void OpenBakulZoom()
    {
        if (!NiskalaProgress.bakulCollected)
            return;

        OpenArtifactZoom(bakulZoom);
    }

    public void OpenCatatanCikSimaZoom()
    {
        if (!NiskalaProgress.catatanCikSimaCollected)
            return;

        OpenArtifactZoom(catatanCikSimaZoom);
    }

    // =========================================
    // PROSES MEMBUKA ZOOM
    // =========================================

    private void OpenArtifactZoom(GameObject zoomObject)
    {
        if (DialogueSystem.Instance != null &&
            DialogueSystem.Instance.IsDialogueActive)
        {
            return;
        }

        if (zoomObject == null)
        {
            Debug.LogError(
                "Objek Zoom artefak belum dihubungkan."
            );
            return;
        }

        // Jika belum ada zoom yang terbuka,
        // simpan kondisi awal ArrowBack.
        if (!IsAnyArtifactZoomActive())
        {
            if (arrowBack != null)
            {
                arrowBackPreviousState =
                    arrowBack.activeSelf;

                arrowBackStateSaved = true;
            }
        }

        // Tutup zoom sebelumnya tanpa mengubah
        // kondisi ArrowBack yang sudah disimpan.
        CloseCurrentArtifactZoom();

        activeZoom = zoomObject;

        activeZoom.SetActive(true);

        if (buttonInvestigasiArtefak != null)
            buttonInvestigasiArtefak.SetActive(true);

        // ArrowBack harus muncul selama zoom terbuka,
        // termasuk ketika artefak dibuka di Ruang Tamu.
        if (arrowBack != null)
            arrowBack.SetActive(true);

        Debug.Log(
            "Artifact Zoom dibuka: " + activeZoom.name
        );
    }

    // =========================================
    // TUTUP ZOOM
    // =========================================

    public bool TryCloseArtifactZoom()
    {
        // Cari objek zoom yang aktif.
        // Berguna jika sebelumnya zoom diaktifkan
        // langsung oleh controller ruangan.
        if (activeZoom == null || !activeZoom.activeSelf)
        {
            activeZoom = FindActiveArtifactZoom();
        }

        if (activeZoom == null)
            return false;

        CloseCurrentArtifactZoom();

        // Kembalikan ArrowBack ke kondisi sebelum zoom.
        if (arrowBack != null && arrowBackStateSaved)
        {
            arrowBack.SetActive(arrowBackPreviousState);
        }

        arrowBackStateSaved = false;

        Debug.Log("Artifact Zoom berhasil ditutup.");

        return true;
    }

    private void CloseCurrentArtifactZoom()
    {
        SetActiveSafe(tepakSirihZoom, false);
        SetActiveSafe(bakulZoom, false);
        SetActiveSafe(catatanCikSimaZoom, false);

        SetActiveSafe(buttonInvestigasiArtefak, false);

        activeZoom = null;
    }

    // =========================================
    // PENGECEKAN ZOOM
    // =========================================

    private bool IsAnyArtifactZoomActive()
    {
        return
            (tepakSirihZoom != null && tepakSirihZoom.activeSelf) ||
            (bakulZoom != null && bakulZoom.activeSelf) ||
            (catatanCikSimaZoom != null && catatanCikSimaZoom.activeSelf);
    }

    private GameObject FindActiveArtifactZoom()
    {
        if (tepakSirihZoom != null &&
            tepakSirihZoom.activeSelf)
        {
            return tepakSirihZoom;
        }

        if (bakulZoom != null &&
            bakulZoom.activeSelf)
        {
            return bakulZoom;
        }

        if (catatanCikSimaZoom != null &&
            catatanCikSimaZoom.activeSelf)
        {
            return catatanCikSimaZoom;
        }

        return null;
    }

    // =========================================
    // BANTUAN
    // =========================================

    private void SetActiveSafe(GameObject target, bool state)
    {
        if (target != null)
            target.SetActive(state);
    }
}