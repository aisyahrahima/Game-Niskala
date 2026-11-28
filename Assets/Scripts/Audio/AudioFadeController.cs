
using System.Collections;
using UnityEngine;

public class AudioFadeController : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Fade Settings")]
    [SerializeField] private float fadeInDuration = 1.5f;
    [SerializeField] private float fadeOutDuration = 1.5f;

    private Coroutine fadeCoroutine;
    private float targetVolume;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
            targetVolume = audioSource.volume;
    }

    public void FadeIn()
    {
        if (audioSource == null)
            return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        targetVolume = audioSource.volume;

        audioSource.volume = 0f;

        if (!audioSource.isPlaying)
            audioSource.Play();

        fadeCoroutine = StartCoroutine(
            FadeVolume(0f, targetVolume, fadeInDuration)
        );
    }

    public void FadeOut()
    {
        if (audioSource == null)
            return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(
            FadeOutSequence()
        );
    }

    private IEnumerator FadeOutSequence()
    {
        yield return StartCoroutine(
            FadeVolume(
                audioSource.volume,
                0f,
                fadeOutDuration
            )
        );

        audioSource.Stop();
        audioSource.volume = targetVolume;

        fadeCoroutine = null;
    }

    private IEnumerator FadeVolume(
        float startVolume,
        float endVolume,
        float duration
    )
    {
        float elapsed = 0f;

        duration = Mathf.Max(0.01f, duration);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            audioSource.volume = Mathf.Lerp(
                startVolume,
                endVolume,
                Mathf.Clamp01(elapsed / duration)
            );

            yield return null;
        }

        audioSource.volume = endVolume;

        fadeCoroutine = null;
    }
}
