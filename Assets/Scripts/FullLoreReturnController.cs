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

    private DialogueSystem dialogueSystem;
    private bool introRunning;

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

            if (arrowBack != null)
                arrowBack.SetActive(false);

            if (petunjuk != null)
                petunjuk.SetActive(true);

            Debug.Log("Pemain mengulang pengejaran. Menunggu tombol Mengerti.");

            return;
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
