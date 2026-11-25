
using UnityEngine;

public class MainMenuBGM : MonoBehaviour
{
    public static MainMenuBGM Instance { get; private set; }

    [Header("Main Menu Audio")]
    [SerializeField] private AudioSource bgmSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (bgmSource != null && !bgmSource.isPlaying)
        {
            bgmSource.loop = true;
            bgmSource.Play();
        }
        else if (bgmSource == null)
        {
            Debug.LogError("BGM Source belum dihubungkan!");
        }
    }

    public void StopBGM()
    {
        if (bgmSource != null)
            bgmSource.Stop();

        Destroy(gameObject);
    }
}
