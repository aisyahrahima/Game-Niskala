using UnityEngine;

public class StorageRainAmbience : MonoBehaviour
{
    public static StorageRainAmbience Instance { get; private set; }

    [Header("Rain Ambience Audio")]
    [SerializeField] private AudioSource rainAudioSource;

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
        // Putar ambience hujan.
        if (rainAudioSource != null)
        {
            rainAudioSource.loop = true;
            rainAudioSource.Play();
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
            rainAudioSource.Stop();
        }
    }

    public void PlayRain()
    {
        if (rainAudioSource != null &&
            !rainAudioSource.isPlaying)
        {
            rainAudioSource.Play();
        }
    }

    public bool IsRainPlaying()
    {
        return rainAudioSource != null &&
               rainAudioSource.isPlaying;
    }
}