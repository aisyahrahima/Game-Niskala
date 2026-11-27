
using System.Collections;
using UnityEngine;

public class MayangCaptureJumpscare : MonoBehaviour
{
    [Header("Objek Jumpscare")]
    [SerializeField] private RectTransform jumpscareAncangAncang;
    [SerializeField] private GameObject jumpscareMulai;
    [SerializeField] private RectTransform jumpscareMulaiRect;
    [SerializeField] private GameObject jumpscareGelap;
    [SerializeField] private CanvasGroup gelapCanvasGroup;

    [Header("Gerakan Ancang-Ancang")]
    [SerializeField] private float riseDistance = 900f;
    [SerializeField] private float riseDuration = 2f;

    [Header("Efek Lampu Berkedip")]
    [SerializeField] private float flickerInterval = 0.09f;
    [SerializeField] private float flickerAlpha = 0.45f;

    [Header("Transisi Gelap")]
    [SerializeField] private float blackoutDuration = 0.4f;
    [SerializeField] private float finalFadeDuration = 0.3f;

    [Header("Efek Getaran")]
    [SerializeField] private float shakeDuration = 0.8f;
    [SerializeField] private float shakeStrength = 22f;

    [Header("Penguncian Interaksi")]
    [SerializeField] private CanvasGroup gameplayCanvasGroup;
    [SerializeField] private CanvasGroup inventoryCanvasGroup;
    [SerializeField] private GameObject arrowBack;

    [Header("Tertangkap Screen")]
    [SerializeField] private GameObject tertangkapScreen;
    [SerializeField] private CanvasGroup tertangkapCanvasGroup;
    [SerializeField] private float tertangkapFadeDuration = 0.5f;
    [SerializeField] private float tertangkapDisplayDuration = 3f;

    private MayangChaseManager chaseManager;
    private Coroutine jumpscareCoroutine;
    private Vector2 ancangHomePosition;
    private Vector2 mulaiHomePosition;

    private bool isPlaying;

    private void Awake()
    {
        if (jumpscareAncangAncang != null)
            ancangHomePosition = jumpscareAncangAncang.anchoredPosition;

        if (jumpscareMulaiRect != null)
            mulaiHomePosition = jumpscareMulaiRect.anchoredPosition;

        if (tertangkapScreen != null)
            tertangkapScreen.SetActive(false);

        if (tertangkapCanvasGroup != null)
            tertangkapCanvasGroup.alpha = 0f;

        ResetJumpscare();
    }

    private void OnEnable()
    {
        SubscribeToManager();
    }

    private void Start()
    {
        SubscribeToManager();
    }

    private void SubscribeToManager()
    {
        if (chaseManager != null)
            return;

        chaseManager = MayangChaseManager.Instance;

        if (chaseManager != null)
        {
            chaseManager.OnChaseFailed += PlayCaptureJumpscare;
        }
    }

    private void OnDisable()
    {
        if (chaseManager != null)
        {
            chaseManager.OnChaseFailed -= PlayCaptureJumpscare;
        }
    }

    private void PlayCaptureJumpscare()
    {
        if (isPlaying)
            return;

        isPlaying = true;

        // Kunci seluruh interaksi gameplay.
        LockGameplayInteractions();

        // Sembunyikan ArrowBack selama jumpscare.
        if (arrowBack != null)
            arrowBack.SetActive(false);

        jumpscareCoroutine = StartCoroutine(
            PlayJumpscareSequence()
        );
    }

    private IEnumerator PlayJumpscareSequence()
    {
        // Hentikan BGM pengejaran sebelum ancang-ancang dimulai.
        if (chaseManager != null)
            chaseManager.StopChaseBGM();

        // Persiapkan objek jumpscare.
        if (jumpscareGelap != null)
            jumpscareGelap.SetActive(true);

        if (gelapCanvasGroup != null)
            gelapCanvasGroup.alpha = 0f;

        if (jumpscareMulai != null)
            jumpscareMulai.SetActive(false);

        if (jumpscareAncangAncang != null)
        {
            jumpscareAncangAncang.gameObject.SetActive(true);

            jumpscareAncangAncang.anchoredPosition =
                ancangHomePosition +
                Vector2.down * riseDistance;
        }

        // play audio pas lg mau muncul ancang-ancang
        if (chaseManager != null)
            chaseManager.PlayAncangAncangSound();
        else
            Debug.LogError("MayangChaseManager tidak ditemukan!");

        // 1. Ancang-ancang bergerak dari bawah ke atas
        // sambil menampilkan efek lampu berkedip.
        float elapsed = 0f;
        float flickerTimer = 0f;
        bool flickerState = false;

        while (elapsed < riseDuration)
        {
            float delta = Time.deltaTime;

            elapsed += delta;
            flickerTimer += delta;

            if (jumpscareAncangAncang != null)
            {
                float progress = Mathf.Clamp01(
                    elapsed / riseDuration
                );

                jumpscareAncangAncang.anchoredPosition =
                    Vector2.Lerp(
                        ancangHomePosition +
                        Vector2.down * riseDistance,
                        ancangHomePosition,
                        progress
                    );
            }

            if (gelapCanvasGroup != null &&
                flickerTimer >= flickerInterval)
            {
                flickerTimer = 0f;
                flickerState = !flickerState;

                gelapCanvasGroup.alpha =
                    flickerState ? flickerAlpha : 0.05f;
            }

            yield return null;
        }

        if (jumpscareAncangAncang != null)
        {
            jumpscareAncangAncang.anchoredPosition =
                ancangHomePosition;
        }

        // 2. Ruangan menjadi gelap sementara.
        if (gelapCanvasGroup != null)
            gelapCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(
            blackoutDuration
        );

        // 3. Ganti visual ketika layar masih gelap.
        if (jumpscareAncangAncang != null)
            jumpscareAncangAncang.gameObject.SetActive(false);

        if (jumpscareMulai != null)
            jumpscareMulai.SetActive(true);

        // stop audio ancang
        if (chaseManager != null)
            chaseManager.StopAncangAncangSound();

        // play audio jumsker 
        if (chaseManager != null)
            chaseManager.PlayJumpscareSound();

        // 4. Ruangan kembali terang.
        if (gelapCanvasGroup != null)
            gelapCanvasGroup.alpha = 0f;

        // 5. Visual Jumpscare_Mulai bergetar.
        yield return StartCoroutine(ShakeJumpscare());

        // 6. Layar kembali gelap sebelum pindah scene.
        yield return StartCoroutine(
            FadeToBlack()
        );

        // Tampilkan Tertangkap_Screen terlebih dahulu.
        yield return StartCoroutine(
            PlayTertangkapScreen()
        );

        // Beri tahu manager bahwa animasi selesai.
        if (chaseManager != null)
        {
            chaseManager.CompleteChaseFailure();
        }
        else
        {
            Debug.LogError(
               "MayangChaseManager tidak ditemukan."
            );
        }

        jumpscareCoroutine = null;
    }

    private IEnumerator PlayTertangkapScreen()
    {
        if (tertangkapScreen == null ||
            tertangkapCanvasGroup == null)
        {
            Debug.LogError(
                "Tertangkap_Screen atau CanvasGroup " +
                "belum dihubungkan."
            );

            yield break;
        }

        // Aktifkan Tertangkap_Screen.
        tertangkapScreen.SetActive(true);

        // Mulai dari transparan.
        tertangkapCanvasGroup.alpha = 0f;

        // Pastikan tidak menerima interaksi.
        tertangkapCanvasGroup.interactable = false;
        tertangkapCanvasGroup.blocksRaycasts = false;

        // Fade-in.
        yield return StartCoroutine(
            FadeTertangkapScreen(0f, 1f)
        );

        // Tampilkan layar selama 3 detik.
        yield return new WaitForSecondsRealtime(
            tertangkapDisplayDuration
        );

        // Fade-out.
        yield return StartCoroutine(
            FadeTertangkapScreen(1f, 0f)
        );

        // Nonaktifkan setelah fade-out selesai.
        tertangkapScreen.SetActive(false);
    }

    private IEnumerator FadeTertangkapScreen(
    float startAlpha,
    float targetAlpha)
    {
        float elapsed = 0f;

        float duration = Mathf.Max(
            0.01f,
            tertangkapFadeDuration
        );

        tertangkapCanvasGroup.alpha = startAlpha;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            tertangkapCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                Mathf.Clamp01(elapsed / duration)
            );

            yield return null;
        }

        tertangkapCanvasGroup.alpha = targetAlpha;
    }

    private IEnumerator ShakeJumpscare()
    {
        if (jumpscareMulaiRect == null)
            yield break;

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            Vector2 shakeOffset =
                Random.insideUnitCircle * shakeStrength;

            jumpscareMulaiRect.anchoredPosition =
                mulaiHomePosition + shakeOffset;

            yield return null;
        }

        jumpscareMulaiRect.anchoredPosition =
            mulaiHomePosition;
    }

    private IEnumerator FadeToBlack()
    {
        if (gelapCanvasGroup == null)
            yield break;

        float elapsed = 0f;
        float startAlpha = gelapCanvasGroup.alpha;

        while (elapsed < finalFadeDuration)
        {
            elapsed += Time.deltaTime;

            gelapCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                1f,
                Mathf.Clamp01(elapsed / finalFadeDuration)
            );

            yield return null;
        }

        gelapCanvasGroup.alpha = 2f;
    }

    private void ResetJumpscare()
    {
        if (jumpscareAncangAncang != null)
        {
            jumpscareAncangAncang.gameObject.SetActive(false);
            jumpscareAncangAncang.anchoredPosition =
                ancangHomePosition;
        }

        if (jumpscareMulai != null)
            jumpscareMulai.SetActive(false);

        if (jumpscareGelap != null)
            jumpscareGelap.SetActive(true);

        if (gelapCanvasGroup != null)
            gelapCanvasGroup.alpha = 0f;
    }

    private void LockGameplayInteractions()
    {
        if (gameplayCanvasGroup != null)
        {
            gameplayCanvasGroup.interactable = false;
            gameplayCanvasGroup.blocksRaycasts = false;
        }

        if (inventoryCanvasGroup != null)
        {
            inventoryCanvasGroup.interactable = false;
            inventoryCanvasGroup.blocksRaycasts = false;
        }

        Debug.Log("Seluruh interaksi gameplay dikunci selama jumpscare.");
    }
}
