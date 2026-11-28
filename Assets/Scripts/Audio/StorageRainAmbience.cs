using UnityEngine;

public class StorageRainAmbience : MonoBehaviour
{
    public static StorageRainAmbience Instance { get; private set; }

    [Header("Rain Ambience Audio")]
    [SerializeField] private AudioSource rainAudioSource;

    [Header("Audio Fade")]
    [SerializeField] private AudioFadeController rainFadeController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // Putar ambience hujan dengan fade-in.
        if (rainAudioSource != null)
        {
            rainAudioSource.loop = true;

            if (rainFadeController != null)
            {
                rainFadeController.FadeIn();
            }
            else
            {
                rainAudioSource.Play();

                Debug.LogWarning(
                    "AudioFadeController Ambience Hujan belum dihubungkan!"
                );
            }
        }
    }

    private void OnDestroy()
    {
        // Hentikan ambience hujan ketika
        // object StorageRainAmbience dihancurkan.
        if (rainAudioSource != null)
        {
            rainAudioSource.Stop();
        }

        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void StopRain()
    {
        if (rainAudioSource != null)
        {
            if (rainFadeController != null &&
                rainAudioSource.isPlaying)
            {
                rainFadeController.FadeOut();
            }
            else
            {
                rainAudioSource.Stop();
            }
        }
    }

    public void PlayRain()
    {
        if (rainAudioSource != null &&
            !rainAudioSource.isPlaying)
        {
            if (rainFadeController != null)
            {
                rainFadeController.FadeIn();
            }
            else
            {
                rainAudioSource.Play();
            }
        }
    }

    public bool IsRainPlaying()
    {
        return rainAudioSource != null &&
               rainAudioSource.isPlaying;
    }
}