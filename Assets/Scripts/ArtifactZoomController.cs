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

    [Header("Interaction Check")]
    [SerializeField] private RoomController roomController;
    [SerializeField] private MonoBehaviour roomInteractionController;

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

        // Periksa apakah ketiga artefak sudah diinvestigasi.
        bool allArtifactsInvestigated =
            NiskalaProgress.tepakSirihInvestigated &&
            NiskalaProgress.bakulInvestigated &&
            NiskalaProgress.catatanCikSimaInvestigated;

        // Jika ini investigasi artefak ketiga,
        // simpan ruangan tempat investigasi dilakukan.
        if (allArtifactsInvestigated)
        {
            LoreOrderFlow.returnRoomAfterFullLore =
                SceneManager.GetActiveScene().name;

            LoreOrderFlow.pendingFullLoreReturn = true;

            Debug.Log(
                "Ketiga artefak sudah diinvestigasi. " +
                "Ruangan terakhir: " +
                LoreOrderFlow.returnRoomAfterFullLore
            );
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
        // Tunggu sampai DialogueSystem tersedia.
        int waitFrame = 0;

        while (DialogueSystem.Instance == null &&
               waitFrame < 120)
        {
            waitFrame++;
            yield return null;
        }

        if (DialogueSystem.Instance == null)
        {
            Debug.LogError(
                "DialogueSystem tidak ditemukan setelah kembali dari LoreFragment."
            );

            if (arrowBack != null)
                arrowBack.SetActive(true);

            yield break;
        }

        string[] lines = null;

        // Tentukan dialog berdasarkan artefak.
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

        // Nonaktifkan ArrowBack selama dialog.
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

            if (arrowBack != null)
                arrowBack.SetActive(true);

            yield break;
        }

        // Tunggu sampai dialog selesai.
        while (DialogueSystem.Instance != null &&
               DialogueSystem.Instance.IsDialogueActive)
        {
            yield return null;
        }

        // Aktifkan kembali ArrowBack.
        // Pemain harus menutup zoom secara manual.
        if (arrowBack != null)
            arrowBack.SetActive(true);

        // Periksa apakah ketiga artefak sudah diinvestigasi.
        bool allArtifactsInvestigated =
            NiskalaProgress.tepakSirihInvestigated &&
            NiskalaProgress.bakulInvestigated &&
            NiskalaProgress.catatanCikSimaInvestigated;

        // Tandai bahwa pemain harus menutup zoom
        // sebelum melanjutkan ke LoreOrder.
        if (allArtifactsInvestigated)
        {
            LoreOrderFlow.pendingLoreOrderIntro = true;

            Debug.Log(
                "Ketiga artefak sudah diinvestigasi. " +
                "Menunggu pemain menutup Zoom artefak."
            );
        }

        // Bersihkan data investigasi LoreFragment.
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
        // Blokir jika interaksi ruangan masih berlangsung.
        if (roomController != null &&
            roomController.IsBusy)
        {
            Debug.Log(
                "Artefak inventory tidak dapat dibuka karena interaksi masih berlangsung."
            );
            return;
        }

        // Periksa apakah zoom atau interaksi objek ruangan masih aktif.
        if (roomInteractionController != null)
        {
            IInteractionBlocker blocker =
                roomInteractionController as IInteractionBlocker;

            if (blocker == null)
            {
                Debug.LogError(
                    "Room Interaction Controller tidak memiliki IInteractionBlocker."
                );
                return;
            }

            if (blocker.IsInteractionBlocked())
            {
                Debug.Log(
                    "Artefak inventory tidak dapat dibuka karena zoom atau interaksi ruangan masih aktif."
                );
                return;
            }
        }

        // Blokir jika dialog masih berlangsung.
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

        // Simpan kondisi ArrowBack sebelum Zoom dibuka.
        if (!IsAnyArtifactZoomActive())
        {
            if (arrowBack != null)
            {
                arrowBackPreviousState =
                    arrowBack.activeSelf;

                arrowBackStateSaved = true;
            }
        }

        // Tutup Zoom sebelumnya.
        CloseCurrentArtifactZoom();

        activeZoom = zoomObject;

        activeZoom.SetActive(true);

        if (buttonInvestigasiArtefak != null)
            buttonInvestigasiArtefak.SetActive(true);

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
        if (activeZoom == null || !activeZoom.activeSelf)
        {
            activeZoom = FindActiveArtifactZoom();
        }

        if (activeZoom == null)
            return false;

        // Tutup seluruh Zoom artefak.
        CloseCurrentArtifactZoom();

        // Kembalikan ArrowBack ke kondisi sebelum Zoom dibuka.
        if (arrowBack != null && arrowBackStateSaved)
        {
            arrowBack.SetActive(arrowBackPreviousState);
        }

        arrowBackStateSaved = false;

        Debug.Log("Artifact Zoom berhasil ditutup.");

        // Jika ketiga artefak sudah diinvestigasi,
        // mulai dialog pengantar menuju LoreOrder.
        if (LoreOrderFlow.pendingLoreOrderIntro)
        {
            StartCoroutine(PlayLoreOrderTransitionDialogue());
        }

        return true;
    }

    private IEnumerator PlayLoreOrderTransitionDialogue()
    {
        // Pastikan Zoom sudah benar-benar tertutup.
        if (IsAnyArtifactZoomActive())
            yield break;

        // Nonaktifkan ArrowBack selama dialog berlangsung.
        if (arrowBack != null)
            arrowBack.SetActive(false);

        // Tunggu sampai DialogueSystem tersedia.
        int waitFrame = 0;

        while (DialogueSystem.Instance == null &&
               waitFrame < 120)
        {
            waitFrame++;
            yield return null;
        }

        if (DialogueSystem.Instance == null)
        {
            Debug.LogError(
                "DialogueSystem tidak ditemukan untuk dialog pengantar LoreOrder."
            );

            if (arrowBack != null)
                arrowBack.SetActive(true);

            yield break;
        }

        string[] lines =
        {
        "Aku sudah menemukan tiga artefak yang tampaknya saling berkaitan.",
        "Setiap benda mengungkap sedikit demi sedikit kejadian di masa lalu."
    };

        bool started = DialogueSystem.Instance.StartDialogue(
            "Arka",
            lines
        );

        if (!started)
        {
            Debug.LogError(
                "Dialog pengantar LoreOrder gagal dimulai."
            );

            if (arrowBack != null)
                arrowBack.SetActive(true);

            yield break;
        }

        // Tunggu sampai kedua dialog selesai.
        while (DialogueSystem.Instance != null &&
               DialogueSystem.Instance.IsDialogueActive)
        {
            yield return null;
        }

        Debug.Log(
            "Dialog pengantar LoreOrder selesai. Berpindah scene."
        );

        // Bersihkan penanda agar dialog tidak terulang.
        LoreOrderFlow.pendingLoreOrderIntro = false;

        // Pindah ke scene LoreOrder.
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene("LoreOrder");
        }
        else
        {
            Debug.LogError(
                "SceneTransitionManager tidak ditemukan."
            );

            if (arrowBack != null)
                arrowBack.SetActive(true);
        }
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