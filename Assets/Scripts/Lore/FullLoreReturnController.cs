using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FullLoreReturnController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject arrowBack;
    [SerializeField] private GameObject petunjuk;
    [SerializeField] private Button buttonMengerti;

    [Header("Mayang")]
    [SerializeField] private GameObject mayangMulai;
    [SerializeField] private CanvasGroup mayangCanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Objek yang Tidak Bisa Diinteraksi")]
    [SerializeField] private GameObject[] nonKeyObjects;

    [Header("Penguncian Gameplay")]
    [SerializeField] private CanvasGroup gameplayCanvasGroup;
    [SerializeField] private CanvasGroup inventoryCanvasGroup;

    // =========================================
    // FINAL ENDING - MAYANG
    // =========================================

    [Header("Final Ending - Mayang")]
    [SerializeField] private GameObject mayangFinalRoot;
    [SerializeField] private GameObject mayangDiam;
    [SerializeField] private GameObject mayangBicara;
    [SerializeField] private GameObject mayangSenyum;
    [SerializeField] private CanvasGroup mayangFinalCanvasGroup;
    [SerializeField] private float finalMayangFadeDuration = 0.5f;

    // =========================================
    // FINAL ENDING - TRANSISI
    // =========================================

    [Header("Final Ending - Transisi")]
    [SerializeField] private CanvasGroup endingFadeCanvasGroup;
    [SerializeField] private float endingFadeDuration = 1f;
    [SerializeField] private string endingSceneName = "Ending";

    private DialogueSystem dialogueSystem;
    private bool introRunning;
    private bool endingSequenceRunning;

    private void Awake()
    {
        if (petunjuk != null)
            petunjuk.SetActive(false);

        if (mayangMulai != null)
            mayangMulai.SetActive(false);

        if (buttonMengerti != null)
            buttonMengerti.onClick.AddListener(OnButtonMengerti);
    }

    private void Start()
    {
        dialogueSystem = DialogueSystem.Instance;

        // Jika pemain baru saja tertangkap,
        // tampilkan Petunjuk tanpa mengulang dialog cerita.
        if (NiskalaProgress.pendingChaseRetryIntro)
        {
            NiskalaProgress.pendingChaseRetryIntro = false;

            SetGameplayLocked(true);

            // Kunci kembali interaksi objek non-key.
            DisableNonKeyObjects();

            if (arrowBack != null)
                arrowBack.SetActive(false);

            if (petunjuk != null)
                petunjuk.SetActive(true);

            Debug.Log("Pemain mengulang pengejaran. Menunggu tombol Mengerti.");

            return;
        }

        // Jika pemain berpindah ke ruangan lain
        // ketika pengejaran masih berlangsung,
        // kunci juga objek non-key di ruangan tersebut.
        if (MayangChaseManager.Instance != null &&
            MayangChaseManager.Instance.IsChaseRunning)
        {
            DisableNonKeyObjects();

            Debug.Log(
                "Objek non-key dikunci di scene: " +
                SceneManager.GetActiveScene().name
            );
        }

        // Jika bukan kembali dari FullLore, tidak perlu menjalankan intro.
        if (!NiskalaProgress.pendingFullLoreIntro)
            return;

        NiskalaProgress.pendingFullLoreIntro = false;

        StartCoroutine(PlayFullLoreReturnSequence());
    }

    private IEnumerator PlayFullLoreReturnSequence()
    {
        if (introRunning)
            yield break;

        introRunning = true;

        // Kunci gameplay dan nonaktifkan ArrowBack.
        SetGameplayLocked(true);

        if (arrowBack != null)
            arrowBack.SetActive(false);

        // Nonaktifkan interaksi non-key object.
        DisableNonKeyObjects();

        // Jeda sebelum dialog Arka.
        yield return new WaitForSecondsRealtime(1f);

        yield return StartDialogue(
            "Arka",
            "Jadi...itu semua yang terjadi.",
            "Sekarang aku sudah tahu apa yang pernah terjadi di tempat ini."
        );

        // Mayang muncul bersamaan dengan dialognya.
        if (mayangMulai != null)
            mayangMulai.SetActive(true);

        yield return StartCoroutine(FadeMayang(0f, 1f));

        yield return StartDialogue(
            "Mayang",
            "Jangan pergi."
        );

        yield return StartDialogue(
            "Arka",
            "...!?",
            "M-Mayang Sari...?"
        );

        yield return StartDialogue(
            "Mayang",
            "Aku tahu kenapa kau datang.",
            "Kau ingin mengambil apa yang tersisa dari tempat ini."
        );

        yield return StartDialogue(
            "Arka",
            "...",
            "Bukan begitu."
        );

        yield return StartDialogue(
            "Mayang",
            "Tempat ini adalah peninggalan keluargaku.",
            "Aku tidak akan membiarkanmu merusaknya."
        );

        // Mayang menghilang.
        yield return StartCoroutine(FadeMayang(1f, 0f));

        if (mayangMulai != null)
            mayangMulai.SetActive(false);

        yield return StartDialogue(
            "Arka",
            "...",
            "Dia pikir aku datang untuk mengambil peninggalan ini.",
            "Aku harus membuatnya mengerti kalau aku tidak berniat mengambilnya.",
            "Kalau begitu... aku harus mengembalikan semua Artefak ini.."
        );

        // Tampilkan Petunjuk.
        if (petunjuk != null)
            petunjuk.SetActive(true);

        Debug.Log("Dialog pengenalan Mayang selesai. Menunggu Button Mengerti.");
    }

    public void BeginArtifactReturnEnding()
    {
        if (endingSequenceRunning)
            return;

        StartCoroutine(PlayArtifactReturnEndingSequence());
    }

    private IEnumerator PlayArtifactReturnEndingSequence()
    {
        if (endingSequenceRunning)
            yield break;

        endingSequenceRunning = true;

        // Beri kesempatan proses drag artefak selesai.
        yield return null;

        // 1. Kunci seluruh interaksi gameplay.
        SetGameplayLocked(true);
        DisableNonKeyObjects();

        // Nonaktifkan ArrowBack.
        if (arrowBack != null)
            arrowBack.SetActive(false);

        Debug.Log("Final ending dimulai.");

        // =========================================
        // 2. LAYAR PERLAHAN MENGGELAP
        // =========================================

        yield return StartCoroutine(FadeEndingScreen());

        // Tunggu selama 1.5 detik dalam keadaan gelap.
        yield return new WaitForSeconds(1.5f);

        // =========================================
        // 3. MAYANG MUNCUL
        // =========================================

        if (mayangFinalRoot != null)
            mayangFinalRoot.SetActive(true);

        SetFinalMayangVisual(mayangDiam);

        // Mayang muncul dengan fade-in.
        yield return StartCoroutine(
            FadeFinalMayang(0f, 1f)
        );

        // Layar perlahan kembali terang.
        yield return StartCoroutine(
            FadeEndingScreenFromBlack()
        );

        // =========================================
        // 3. DIALOG ARKA
        // =========================================

        SetFinalMayangVisual(mayangDiam);

        yield return StartDialogue(
            "Arka",
            "Aku mengembalikan semuanya.",
            "Aku tidak datang untuk mengambil atau merusak apapun.",
            "Aku hanya ingin tahu apa yang sebenarnya terjadi di tempat ini."
        );

        // =========================================
        // 4. DIALOG MAYANG PERTAMA
        // =========================================

        yield return new WaitForSeconds(1f);

        yield return StartMayangFinalDialogue(
            "Sekarang aku tahu niatmu.",
            "Pergilah."
        );

        // =========================================
        // 5. DIALOG ARKA
        // =========================================

        SetFinalMayangVisual(mayangDiam);

        yield return StartDialogue(
            "Arka",
            "Terima kasih."
        );

        // =========================================
        // 6. DIALOG MAYANG KEDUA
        // =========================================

        yield return StartMayangFinalDialogue(
            "Dan ingat...",
            "Kisah yang kau temukan di sini adalah bagian dari tempat ini.",
            "Jangan biarkan kisah itu hilang."
        );

        // =========================================
        // 7. MAYANG TERSENYUM DAN MENGHILANG
        // =========================================

        SetFinalMayangVisual(mayangSenyum);

        yield return StartCoroutine(
            FadeFinalMayang(1f, 0f)
        );

        if (mayangFinalRoot != null)
            mayangFinalRoot.SetActive(false);

        // =========================================
        // 8. DIALOG TERAKHIR ARKA
        // =========================================

        yield return StartDialogue(
            "Arka",
            "Aku akan mengingatnya.",
            "Semoga kalian beristirahat dengan tenang."
        );

        // =========================================
        // 9. LAYAR MENGHITAM
        // =========================================

        yield return StartCoroutine(FadeEndingScreen());

        // =========================================
        // 10. PINDAH KE SCENE ENDING
        // =========================================

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(
                endingSceneName
            );
        }
        else
        {
            Debug.LogError(
                "SceneTransitionManager tidak ditemukan."
            );
        }
    }

    private IEnumerator StartMayangFinalDialogue(params string[] lines)
    {
        if (dialogueSystem == null)
            dialogueSystem = DialogueSystem.Instance;

        if (dialogueSystem == null)
        {
            Debug.LogError("DialogueSystem tidak ditemukan.");
            yield break;
        }

        bool started = dialogueSystem.StartDialogue(
            "Mayang",
            lines
        );

        if (!started)
        {
            Debug.LogError("Dialog Mayang gagal dimulai.");
            yield break;
        }

        float switchTimer = 0f;
        bool isSpeaking = false;

        while (dialogueSystem != null &&
               dialogueSystem.IsDialogueActive)
        {
            switchTimer += Time.deltaTime;

            // Ganti visual setiap 0.35 detik.
            if (switchTimer >= 0.35f)
            {
                switchTimer = 0f;
                isSpeaking = !isSpeaking;

                SetFinalMayangVisual(
                    isSpeaking ? mayangBicara : mayangDiam
                );
            }

            yield return null;
        }

        // Setelah dialog selesai, Mayang kembali diam.
        SetFinalMayangVisual(mayangDiam);
    }

    private IEnumerator FadeFinalMayang(
    float startAlpha,
    float targetAlpha)
    {
        if (mayangFinalCanvasGroup == null)
        {
            Debug.LogError(
                "CanvasGroup Final Mayang belum dihubungkan."
            );
            yield break;
        }

        float elapsed = 0f;
        float duration = Mathf.Max(
            0.01f,
            finalMayangFadeDuration
        );

        mayangFinalCanvasGroup.alpha = startAlpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            mayangFinalCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                Mathf.Clamp01(elapsed / duration)
            );

            yield return null;
        }

        mayangFinalCanvasGroup.alpha = targetAlpha;
    }

    private IEnumerator FadeEndingScreen()
    {
        if (endingFadeCanvasGroup == null)
        {
            Debug.LogError(
                "Ending Fade CanvasGroup belum dihubungkan."
            );
            yield break;
        }

        endingFadeCanvasGroup.gameObject.SetActive(true);
        endingFadeCanvasGroup.alpha = 0f;

        float elapsed = 0f;
        float duration = Mathf.Max(
            0.01f,
            endingFadeDuration
        );

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            endingFadeCanvasGroup.alpha = Mathf.Lerp(
                0f,
                1f,
                Mathf.Clamp01(elapsed / duration)
            );

            yield return null;
        }

        endingFadeCanvasGroup.alpha = 1f;
    }

    private IEnumerator FadeEndingScreenFromBlack()
    {
        if (endingFadeCanvasGroup == null)
        {
            Debug.LogError(
                "Ending Fade CanvasGroup belum dihubungkan."
            );
            yield break;
        }

        float elapsed = 0f;
        float duration = Mathf.Max(
            0.01f,
            endingFadeDuration
        );

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            endingFadeCanvasGroup.alpha = Mathf.Lerp(
                1f,
                0f,
                Mathf.Clamp01(elapsed / duration)
            );

            yield return null;
        }

        endingFadeCanvasGroup.alpha = 0f;
    }

    private void SetFinalMayangVisual(GameObject activeVisual)
    {
        if (mayangDiam != null)
            mayangDiam.SetActive(false);

        if (mayangBicara != null)
            mayangBicara.SetActive(false);

        if (mayangSenyum != null)
            mayangSenyum.SetActive(false);

        if (activeVisual != null)
            activeVisual.SetActive(true);
    }

    private IEnumerator StartDialogue(
        string speaker,
        params string[] lines)
    {
        if (dialogueSystem == null)
            dialogueSystem = DialogueSystem.Instance;

        if (dialogueSystem == null)
        {
            Debug.LogError("DialogueSystem tidak ditemukan.");
            yield break;
        }

        bool started = dialogueSystem.StartDialogue(
            speaker,
            lines
        );

        if (!started)
        {
            Debug.LogError("Dialog gagal dimulai: " + speaker);
            yield break;
        }

        while (dialogueSystem != null &&
               dialogueSystem.IsDialogueActive)
        {
            yield return null;
        }
    }

    private IEnumerator FadeMayang(float startAlpha, float targetAlpha)
    {
        if (mayangCanvasGroup == null)
        {
            Debug.LogError("CanvasGroup Mayang belum dihubungkan.");
            yield break;
        }

        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, fadeDuration);

        mayangCanvasGroup.alpha = startAlpha;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            mayangCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                elapsed / duration
            );

            yield return null;
        }

        mayangCanvasGroup.alpha = targetAlpha;
    }

    private void DisableNonKeyObjects()
    {
        foreach (GameObject obj in nonKeyObjects)
        {
            if (obj == null)
                continue;

            CanvasGroup group = obj.GetComponent<CanvasGroup>();

            if (group == null)
                group = obj.AddComponent<CanvasGroup>();

            // Visual tetap terlihat, tetapi tidak menerima raycast.
            group.blocksRaycasts = false;
            group.interactable = false;
        }
    }

    private void SetGameplayLocked(bool locked)
    {
        if (gameplayCanvasGroup != null)
        {
            gameplayCanvasGroup.interactable = !locked;
            gameplayCanvasGroup.blocksRaycasts = !locked;
        }

        if (inventoryCanvasGroup != null)
        {
            inventoryCanvasGroup.interactable = !locked;
            inventoryCanvasGroup.blocksRaycasts = !locked;
        }
    }

    private void OnButtonMengerti()
    {
        // 1. Tutup pop-up Petunjuk.
        if (petunjuk != null)
            petunjuk.SetActive(false);

        // 2. Buka kembali gameplay.
        SetGameplayLocked(false);

        // 3. Tampilkan ArrowBack hanya jika ada ruangan sebelumnya.
        if (arrowBack != null)
        {
            string currentScene =
                SceneManager.GetActiveScene().name;

            arrowBack.SetActive(currentScene != "RuangTamu");
        }

        // 4. Simpan scene sebagai checkpoint.
        NiskalaProgress.chaseCheckpointScene =
            SceneManager.GetActiveScene().name;

        NiskalaProgress.chaseStarted = true;

        // 5. Mulai pengejaran Mayang.
        if (MayangChaseManager.Instance != null)
        {
            MayangChaseManager.Instance.StartChase();
        }
        else
        {
            Debug.LogError(
                "MayangChaseManager tidak ditemukan."
            );
        }

        introRunning = false;

        Debug.Log(
            "Pengejaran dimulai dari checkpoint: " +
            NiskalaProgress.chaseCheckpointScene
        );
    }

    private void OnDestroy()
    {
        if (buttonMengerti != null)
            buttonMengerti.onClick.RemoveListener(OnButtonMengerti);
    }
}
