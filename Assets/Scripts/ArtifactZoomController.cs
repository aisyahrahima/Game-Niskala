using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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

    private void Start()
    {
        if (!ArtifactLoreFlow.pendingReturn)
            return;

        string currentScene =
            SceneManager.GetActiveScene().name;

        // Pastikan hanya scene asal yang menjalankan pemulihan.
        if (currentScene != ArtifactLoreFlow.returnSceneName)
            return;

        RestoreArtifactAfterLore();
    }

    // =========================================
    // BUKA ARTEFAK DARI INVENTORY
    // =========================================

    public void OpenTepakSirihZoom()
    {
        if (!NiskalaProgress.tepakSirihCollected)
            return;

        OpenArtifactZoom(tepakSirihZoom);

        if (NiskalaProgress.tepakSirihInvestigated &&
        buttonInvestigasiArtefak != null)
        {
            buttonInvestigasiArtefak.SetActive(false);
        }
    }

    public void OpenBakulZoom()
    {
        if (!NiskalaProgress.bakulCollected)
            return;

        OpenArtifactZoom(bakulZoom);

        if (NiskalaProgress.bakulInvestigated &&
        buttonInvestigasiArtefak != null)
        {
            buttonInvestigasiArtefak.SetActive(false);
        }
    }

    public void OpenCatatanCikSimaZoom()
    {
        if (!NiskalaProgress.catatanCikSimaCollected)
            return;

        OpenArtifactZoom(catatanCikSimaZoom);

        if (NiskalaProgress.catatanCikSimaInvestigated &&
        buttonInvestigasiArtefak != null)
        {
            buttonInvestigasiArtefak.SetActive(false);
        }
    }

    // =========================================
    // INVESTIGASI ARTEFAK
    // =========================================

    public void InvestigateActiveArtifact()
    {
        // Jika activeZoom belum terisi atau objeknya sudah tidak aktif,
        // cari kembali Zoom artefak yang sedang aktif di Hierarchy.
        if (activeZoom == null || !activeZoom.activeSelf)
        {
            activeZoom = FindActiveArtifactZoom();
        }

        // Pastikan ada Zoom artefak yang sedang terbuka.
        if (activeZoom == null)
        {
            Debug.LogWarning(
                "Tidak ada Zoom artefak yang sedang terbuka."
            );
            return;
        }

        string artifactID = "";
        string targetScene = "";

        // Tentukan artefak dan scene LoreFragment.
        if (activeZoom == tepakSirihZoom)
        {
            if (NiskalaProgress.tepakSirihInvestigated)
                return;

            artifactID = "TepakSirih";
            targetScene = "LoreFragmentA";

            NiskalaProgress.tepakSirihInvestigated = true;
        }
        else if (activeZoom == bakulZoom)
        {
            if (NiskalaProgress.bakulInvestigated)
                return;

            artifactID = "Bakul";
            targetScene = "LoreFragmentB";

            NiskalaProgress.bakulInvestigated = true;
        }
        else if (activeZoom == catatanCikSimaZoom)
        {
            if (NiskalaProgress.catatanCikSimaInvestigated)
                return;

            artifactID = "CatatanCikSima";
            targetScene = "LoreFragmentC";

            NiskalaProgress.catatanCikSimaInvestigated = true;
        }
        else
        {
            Debug.LogWarning(
                "Zoom yang aktif bukan artefak yang dapat diinvestigasi."
            );
            return;
        }

        // Simpan kondisi ArrowBack sebelum berpindah scene.
        ArtifactLoreFlow.arrowBackPreviousState =
            arrowBackStateSaved
            ? arrowBackPreviousState
            : (arrowBack != null && arrowBack.activeSelf);

        ArtifactLoreFlow.hasArrowBackState = true;

        // Simpan informasi scene asal dan artefak.
        ArtifactLoreFlow.artifactID = artifactID;

        ArtifactLoreFlow.returnSceneName =
            SceneManager.GetActiveScene().name;

        ArtifactLoreFlow.pendingReturn = true;

        // Sembunyikan tombol investigasi.
        if (buttonInvestigasiArtefak != null)
            buttonInvestigasiArtefak.SetActive(false);

        Debug.Log(
            "Investigasi " + artifactID +
            " dimulai. Kembali ke scene: " +
            ArtifactLoreFlow.returnSceneName
        );

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(targetScene);
        }
        else
        {
            Debug.LogError(
                "SceneTransitionManager tidak ditemukan."
            );
        }
    }

    private void RestoreArtifactAfterLore()
    {
        switch (ArtifactLoreFlow.artifactID)
        {
            case "TepakSirih":
                activeZoom = tepakSirihZoom;
                break;

            case "Bakul":
                activeZoom = bakulZoom;
                break;

            case "CatatanCikSima":
                activeZoom = catatanCikSimaZoom;
                break;

            default:
                Debug.LogError(
                    "ID artefak untuk pemulihan tidak dikenal."
                );
                return;
        }

        if (activeZoom == null)
        {
            Debug.LogError(
                "Objek Zoom artefak belum dihubungkan."
            );
            return;
        }

        // Tampilkan kembali Zoom artefak.
        activeZoom.SetActive(true);

        // Tombol investigasi tetap disembunyikan.
        if (buttonInvestigasiArtefak != null)
            buttonInvestigasiArtefak.SetActive(false);

        // Tampilkan ArrowBack agar pemain dapat menutup Zoom.
        if (arrowBack != null)
            arrowBack.SetActive(true);

        // Pulihkan kondisi ArrowBack setelah Zoom ditutup.
        if (ArtifactLoreFlow.hasArrowBackState)
        {
            arrowBackPreviousState =
                ArtifactLoreFlow.arrowBackPreviousState;

            arrowBackStateSaved = true;
        }

        Debug.Log(
            "Zoom artefak berhasil dipulihkan setelah LoreFragment."
        );

        StartCoroutine(PlayArtifactReturnDialogue());
    }


    private IEnumerator PlayArtifactReturnDialogue()
    {
        // Tunggu sampai DialogueSystem tersedia di scene asal.
        int waitFrame = 0;

        while (DialogueSystem.Instance == null && waitFrame < 120)
        {
            waitFrame++;
            yield return null;
        }

        if (DialogueSystem.Instance == null)
        {
            Debug.LogError(
                "DialogueSystem tidak ditemukan setelah kembali dari LoreFragment."
            );

            // Aktifkan kembali ArrowBack jika DialogueSystem tidak ditemukan.
            if (arrowBack != null)
                arrowBack.SetActive(true);

            yield break;
        }

        string[] lines = null;

        switch (ArtifactLoreFlow.artifactID)
        {
            case "TepakSirih":
                lines = new string[]
                {
                "Tepak sirih?",
                "Apa hubungannya dengan orang-orang itu?"
                };
                break;

            case "Bakul":
                lines = new string[]
                {
                "Peperangan?",
                "Kenapa itu bisa terjadi?",
                "Dan... kenapa benda ini ada di sini?"
                };
                break;

            case "CatatanCikSima":
                lines = new string[]
                {
                "...",
                "Wanita itu terlihat sangat sedih..."
                };
                break;
        }

        if (lines == null)
        {
            Debug.LogError(
                "Dialog artefak tidak ditemukan."
            );

            if (arrowBack != null)
                arrowBack.SetActive(true);

            yield break;
        }

        // Nonaktifkan ArrowBack sebelum dialog dimulai.
        if (arrowBack != null)
            arrowBack.SetActive(false);

        bool started = DialogueSystem.Instance.StartDialogue(
            "Arka",
            lines
        );

        if (!started)
        {
            Debug.LogError(
                "Dialog setelah LoreFragment gagal dimulai."
            );

            // Aktifkan kembali ArrowBack jika dialog gagal dimulai.
            if (arrowBack != null)
                arrowBack.SetActive(true);

            yield break;
        }

        // Tunggu sampai seluruh dialog selesai.
        while (DialogueSystem.Instance != null &&
               DialogueSystem.Instance.IsDialogueActive)
        {
            yield return null;
        }

        // Aktifkan kembali ArrowBack setelah dialog selesai.
        if (arrowBack != null)
            arrowBack.SetActive(true);

        // Bersihkan status setelah dialog selesai.
        ArtifactLoreFlow.Clear();

        Debug.Log(
            "Dialog setelah investigasi artefak selesai."
        );
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