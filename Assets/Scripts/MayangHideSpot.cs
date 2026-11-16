
using UnityEngine;
using System.Collections;

public class MayangHideSpot : MonoBehaviour
{
    [Header("Tampilan Tempat Persembunyian")]
    [SerializeField] private GameObject safeView;
    [SerializeField] private GameObject dangerView;

    [Header("Waktu Kemunculan Bahaya")]
    [SerializeField] private float warningStartTime = 7f;

    [Header("Fade Tampilan Bahaya")]
    [SerializeField] private CanvasGroup dangerCanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    private bool isHiding;
    private bool currentDangerState;
    private Coroutine transitionCoroutine;

    public bool IsHiding => isHiding;

    private void Update()
    {
        if (!isHiding || MayangChaseManager.Instance == null)
            return;

        bool shouldShowDanger = ShouldShowDangerView();

        if (currentDangerState != shouldShowDanger)
        {
            currentDangerState = shouldShowDanger;

            if (transitionCoroutine != null)
                StopCoroutine(transitionCoroutine);

            transitionCoroutine =
                StartCoroutine(ChangeDangerView(shouldShowDanger));
        }
    }

    public void EnterHideSpot()
    {
        if (isHiding || MayangChaseManager.Instance == null)
            return;

        if (!MayangChaseManager.Instance.IsChaseRunning)
            return;

        isHiding = true;

        MayangChaseManager.Instance.SetPlayerHiding(true);

        // Tampilan aman selalu aktif.
        if (safeView != null)
            safeView.SetActive(true);

        // Awalnya tampilan bahaya disembunyikan.
        if (dangerView != null)
            dangerView.SetActive(false);

        if (dangerCanvasGroup != null)
            dangerCanvasGroup.alpha = 0f;

        currentDangerState = false;

        Debug.Log("Arka mulai bersembunyi.");

        // Periksa apakah peringatan atau zona bahaya
        // sudah berlangsung ketika pemain masuk POV.
        if (ShouldShowDangerView())
        {
            currentDangerState = true;

            transitionCoroutine =
                StartCoroutine(ChangeDangerView(true));
        }
    }

    public void ExitHideSpot()
    {
        if (!isHiding)
            return;

        isHiding = false;

        if (MayangChaseManager.Instance != null)
        {
            MayangChaseManager.Instance.SetPlayerHiding(false);
        }

        currentDangerState = false;

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }

        if (safeView != null)
            safeView.SetActive(false);

        if (dangerView != null)
            dangerView.SetActive(false);

        if (dangerCanvasGroup != null)
            dangerCanvasGroup.alpha = 0f;

        Debug.Log("Arka keluar dari tempat persembunyian.");
    }

    private bool ShouldShowDangerView()
    {
        MayangChaseManager manager = MayangChaseManager.Instance;

        if (manager == null || !manager.IsChaseRunning)
            return false;

        // Peringatan dimulai saat tersisa 3 detik
        // dari fase aman.
        bool warningStarted =
            manager.PhaseTimeRemaining <=
            (manager.SafeDuration - warningStartTime);

        return manager.IsDangerPhase || warningStarted;
    }

    private IEnumerator ChangeDangerView(bool showDanger)
    {
        if (dangerView == null || dangerCanvasGroup == null)
        {
            transitionCoroutine = null;
            yield break;
        }

        if (showDanger)
        {
            // Aktifkan tampilan bahaya sebelum Fade In.
            dangerView.SetActive(true);
        }

        float startAlpha = dangerCanvasGroup.alpha;
        float targetAlpha = showDanger ? 1f : 0f;

        float duration = Mathf.Max(0.01f, fadeDuration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            dangerCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                Mathf.Clamp01(elapsed / duration)
            );

            yield return null;
        }

        dangerCanvasGroup.alpha = targetAlpha;

        // Setelah Fade Out selesai, nonaktifkan tampilan bahaya.
        if (!showDanger)
            dangerView.SetActive(false);

        transitionCoroutine = null;
    }
}
