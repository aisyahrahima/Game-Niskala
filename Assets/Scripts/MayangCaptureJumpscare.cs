
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
    [SerializeField] private float riseDuration = 1.5f;

    [Header("Efek Lampu Berkedip")]
    [SerializeField] private float flickerInterval = 0.09f;
    [SerializeField] private float flickerAlpha = 0.45f;

    [Header("Transisi Gelap")]
    [SerializeField] private float blackoutDuration = 0.4f;
    [SerializeField] private float finalFadeDuration = 0.3f;

    [Header("Efek Getaran")]
    [SerializeField] private float shakeDuration = 0.8f;
    [SerializeField] private float shakeStrength = 22f;

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

        jumpscareCoroutine = StartCoroutine(
            PlayJumpscareSequence()
        );
    }

    private IEnumerator PlayJumpscareSequence()
    {
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

        // 1. Ancang-ancang bergerak dari bawah ke atas
        // sambil menampilkan efek lampu berkedip.
        float elapsed = 0f;
        float flickerTimer = 0f;
        bool flickerState = false;

        while (elapsed < riseDuration)
        {
            float delta = Time.unscaledDeltaTime;

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

        yield return new WaitForSecondsRealtime(
            blackoutDuration
        );

        // 3. Ganti visual ketika layar masih gelap.
        if (jumpscareAncangAncang != null)
            jumpscareAncangAncang.gameObject.SetActive(false);

        if (jumpscareMulai != null)
            jumpscareMulai.SetActive(true);

        // 4. Ruangan kembali terang.
        if (gelapCanvasGroup != null)
            gelapCanvasGroup.alpha = 0f;

        // 5. Visual Jumpscare_Mulai bergetar.
        yield return StartCoroutine(ShakeJumpscare());

        // 6. Layar kembali gelap sebelum pindah scene.
        yield return StartCoroutine(
            FadeToBlack()
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

    private IEnumerator ShakeJumpscare()
    {
        if (jumpscareMulaiRect == null)
            yield break;

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

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
            elapsed += Time.unscaledDeltaTime;

            gelapCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                1f,
                Mathf.Clamp01(elapsed / finalFadeDuration)
            );

            yield return null;
        }

        gelapCanvasGroup.alpha = 1f;
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
}
