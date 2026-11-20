
using UnityEngine;

public class MainMenuBGM : MonoBehaviour
{
    public static MainMenuBGM Instance;

    private AudioSource bgmSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        bgmSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (bgmSource != null)
        {
            bgmSource.loop = true;
            bgmSource.Play();
        }
        else
        {
            Debug.LogError("AudioSource tidak ditemukan!");
        }
    }

    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }

        Destroy(gameObject);
    }
}
