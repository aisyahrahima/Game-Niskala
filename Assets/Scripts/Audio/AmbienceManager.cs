using UnityEngine;
using UnityEngine.SceneManagement;

public class AmbienceManager : MonoBehaviour
{
    public static AmbienceManager Instance { get; private set; }

    private AudioSource ambienceSource;
    private AudioFadeController ambienceFadeController;

    private bool ambienceDisabled;

    // Scene yang menggunakan ambience misterius.
    private readonly string[] ambienceScenes =
    {
        "RuangTamu",
        "RuangMakan",
        "RuangTengah",
        "KamarTidur",
        "RuangPenyimpanan",
        "LoreOrder"
    };

    private void Awake()
    {
        // Hindari duplikasi manager.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Pertahankan manager selama pergantian scene.
        DontDestroyOnLoad(gameObject);

        // Ambil AudioSource ambience.
        ambienceSource = GetComponentInChildren<AudioSource>(true);

        if (ambienceSource == null)
        {
            Debug.LogError(
                "AudioSource Ambience tidak ditemukan!"
            );
        }
        else
        {
            // Ambil AudioFadeController.
            ambienceFadeController =
                ambienceSource.GetComponent<AudioFadeController>();

            if (ambienceFadeController == null)
            {
                Debug.LogError(
                    "AudioFadeController Ambience tidak ditemukan!"
                );
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // Atur ambience berdasarkan scene awal.
        UpdateAmbienceForScene(
            SceneManager.GetActiveScene().name
        );
    }

    // Dipanggil ketika scene baru selesai dimuat.
    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        UpdateAmbienceForScene(scene.name);
    }

    // Mengatur ambience berdasarkan scene.
    private void UpdateAmbienceForScene(
        string sceneName
    )
    {
        // Jika ambience telah dinonaktifkan,
        // jangan putar kembali.
        if (ambienceDisabled)
        {
            StopAmbience();
            return;
        }

        // Periksa apakah scene menggunakan ambience misterius.
        bool shouldPlayAmbience = false;

        foreach (string ambienceScene in ambienceScenes)
        {
            if (sceneName == ambienceScene)
            {
                shouldPlayAmbience = true;
                break;
            }
        }

        if (shouldPlayAmbience)
        {
            PlayAmbience();
        }
        else
        {
            StopAmbience();
        }
    }

    // Memutar ambience dengan fade-in.
    public void PlayAmbience()
    {
        if (ambienceDisabled)
            return;

        if (ambienceSource == null)
            return;

        if (ambienceFadeController == null)
        {
            ambienceSource.loop = true;

            if (!ambienceSource.isPlaying)
                ambienceSource.Play();

            return;
        }

        if (!ambienceSource.isPlaying)
        {
            ambienceSource.loop = true;
            ambienceFadeController.FadeIn();
        }
    }

    // Menghentikan ambience dengan fade-out.
    public void StopAmbience()
    {
        if (ambienceSource == null)
            return;

        if (ambienceSource.isPlaying)
        {
            if (ambienceFadeController != null)
            {
                ambienceFadeController.FadeOut();
            }
            else
            {
                ambienceSource.Stop();
            }
        }
    }

    // Menonaktifkan ambience untuk sementara.
    public void DisableAmbience()
    {
        ambienceDisabled = true;
        StopAmbience();
    }

    // Mengembalikan ambience ke kondisi awal sesi baru.
    public void ResetAmbience()
    {
        ambienceDisabled = false;

        // Hentikan audio dan fade yang sedang berjalan.
        if (ambienceSource != null)
        {
            ambienceSource.Stop();
        }

        // Sesuaikan ambience dengan scene yang aktif.
        UpdateAmbienceForScene(
            SceneManager.GetActiveScene().name
        );
    }
}