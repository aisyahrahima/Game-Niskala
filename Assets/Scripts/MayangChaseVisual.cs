
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class MayangChaseVisual : MonoBehaviour
{
    [Header("Red Vignette")]
    [SerializeField] private CanvasGroup redVignette;

    [Header("Waktu Kemunculan")]
    [SerializeField] private float vignetteStartTime = 7f;

    [Header("Fade")]
    [SerializeField] private float fadeSpeed = 2f;

    [Header("Efek Denyut")]
    [SerializeField] private float minimumAlpha = 0.2f;
    [SerializeField] private float maximumAlpha = 0.65f;
    [SerializeField] private float pulseSpeed = 2f;

    private MayangChaseManager chaseManager;

    private void Awake()
    {
        if (redVignette == null)
            redVignette = GetComponent<CanvasGroup>();

        if (redVignette != null)
        {
            redVignette.alpha = 0f;
            redVignette.interactable = false;
            redVignette.blocksRaycasts = false;
        }
    }

    private void Start()
    {
        chaseManager = MayangChaseManager.Instance;

        if (chaseManager == null)
        {
            Debug.LogWarning(
                "MayangChaseManager belum ditemukan."
            );
        }
    }

    private void Update()
    {
        if (redVignette == null || chaseManager == null)
            return;

        float targetAlpha = 0f;

        if (chaseManager.IsChaseRunning)
        {
            bool danger = chaseManager.IsDangerPhase;

            bool warningStarted =
                chaseManager.PhaseTimeRemaining <=
                (chaseManager.SafeDuration - vignetteStartTime);

            // Mulai muncul pada 3 detik terakhir
            // fase aman dan tetap terlihat saat bahaya.
            if (danger || warningStarted)
            {
                float pulse = (Mathf.Sin(
                    Time.time * pulseSpeed
                ) + 1f) / 2f;

                targetAlpha = Mathf.Lerp(
                    minimumAlpha,
                    maximumAlpha,
                    pulse
                );
            }
        }

        // Mengubah alpha secara perlahan.
        redVignette.alpha = Mathf.MoveTowards(
            redVignette.alpha,
            targetAlpha,
            fadeSpeed * Time.deltaTime
        );
    }
}
