using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TertangkapOverlayPersistence : MonoBehaviour
{
    public static TertangkapOverlayPersistence Instance { get; private set; }

    [Header("Tertangkap Screen")]
    [SerializeField] private GameObject tertangkapScreen;
    [SerializeField] private CanvasGroup tertangkapCanvasGroup;

    [Header("Fade")]
    [SerializeField] private float fadeInDuration = 1.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float delayAfterCheckpointLoaded = 1f;

    private bool waitingForCheckpoint;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        // Hindari duplikasi overlay.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Pertahankan Canvas selama pergantian scene.
        DontDestroyOnLoad(gameObject);

        if (tertangkapScreen != null)
            tertangkapScreen.SetActive(false);

        if (tertangkapCanvasGroup != null)
            tertangkapCanvasGroup.alpha = 0f;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Dipanggil sebelum checkpoint di-reload.
    public void WaitForCheckpointReload()
    {
        waitingForCheckpoint = true;
    }

    // Menampilkan Tertangkap_Screen dan melakukan fade in.
    public IEnumerator ShowOverlay()
    {
        if (tertangkapScreen != null)
            tertangkapScreen.SetActive(true);

        if (tertangkapCanvasGroup != null)
        {
            tertangkapCanvasGroup.alpha = 0f;
            tertangkapCanvasGroup.interactable = false;
            tertangkapCanvasGroup.blocksRaycasts = false;
        }

        yield return StartCoroutine(
            FadeOverlay(0f, 1f, fadeInDuration)
        );
    }

    // Dipanggil setelah scene baru selesai dimuat.
    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        if (!waitingForCheckpoint)
            return;

        waitingForCheckpoint = false;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(
            HideOverlayAfterCheckpointLoaded()
        );
    }

    private IEnumerator HideOverlayAfterCheckpointLoaded()
    {
        // Tunggu sampai UI scene checkpoint diperbarui.
        yield return new WaitForSecondsRealtime(
            delayAfterCheckpointLoaded
        );

        // Fade out Tertangkap_Screen.
        yield return StartCoroutine(
            FadeOverlay(1f, 0f, fadeOutDuration)
        );

        if (tertangkapScreen != null)
            tertangkapScreen.SetActive(false);

        fadeCoroutine = null;
    }

    // Fade layar.
    public IEnumerator FadeOverlay(
    float startAlpha,
    float targetAlpha,
    float duration
)
    {
        if (tertangkapCanvasGroup == null)
            yield break;

        float elapsed = 0f;

        tertangkapCanvasGroup.alpha = startAlpha;

        if (duration <= 0f)
        {
            tertangkapCanvasGroup.alpha = targetAlpha;
            yield break;
        }

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(
                elapsed / duration
            );

            tertangkapCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                progress
            );

            yield return null;
        }

        tertangkapCanvasGroup.alpha = targetAlpha;
    }
}