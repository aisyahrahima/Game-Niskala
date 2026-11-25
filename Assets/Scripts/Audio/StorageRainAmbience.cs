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
         // Hentikan ambience misterius.
        if (AmbienceManager.Instance != null)
        {
            AmbienceManager.Instance.StopAmbience();
        }

        if (rainAudioSource != null)
        {
            rainAudioSource.loop = true;
            rainAudioSource.Play();
        }
    }

   private void OnDestroy()
    {
        if (rainAudioSource != null)
        {
            rainAudioSource.Stop();
        }

        // Kembalikan ambience misterius jika masih diizinkan.
        if (AmbienceManager.Instance != null)
        {
            AmbienceManager.Instance.PlayAmbience();
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
        if (rainAudioSource != null && !rainAudioSource.isPlaying)
        {
            rainAudioSource.Play();
        }
    }

    public bool IsRainPlaying()
    {
        return rainAudioSource != null && rainAudioSource.isPlaying;
    }
}