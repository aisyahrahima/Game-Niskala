
using UnityEngine;
using UnityEngine.SceneManagement;

public class AmbienceManager : MonoBehaviour
{
    public static AmbienceManager Instance;

    private AudioSource ambienceSource;
    private bool ambienceDisabled;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        ambienceSource = GetComponentInChildren<AudioSource>(true);

        if (ambienceSource == null)
        {
            Debug.LogError("AudioSource Ambience tidak ditemukan!");
        }
    }

    private void Start()
    {
        // Jangan putar ambience misterius di Ruang Penyimpanan.
        if (SceneManager.GetActiveScene().name == "RuangPenyimpanan")
            return;

        PlayAmbience();
    }

    public void PlayAmbience()
    {
        if (ambienceDisabled)
            return;

        if (ambienceSource != null && !ambienceSource.isPlaying)
        {
            ambienceSource.loop = true;
            ambienceSource.Play();
        }
    }

    public void StopAmbience()
    {
        if (ambienceSource != null)
        {
            ambienceSource.Stop();
        }
    }

    public void DisableAmbience()
    {
        ambienceDisabled = true;
        StopAmbience();
    }
}
