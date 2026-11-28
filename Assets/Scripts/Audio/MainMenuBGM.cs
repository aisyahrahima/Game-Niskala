using System.Collections;
using UnityEngine;

public class MainMenuBGM : MonoBehaviour
{
    public static MainMenuBGM Instance { get; private set; }

    [Header("Main Menu Audio")]
    [SerializeField] private AudioSource bgmSource;

    [Header("Audio Fade")]
    [SerializeField] private AudioFadeController audioFadeController;
    [SerializeField] private float fadeOutDestroyDelay = 1.5f;

    private bool isStopping;

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
        if (bgmSource == null)
        {
            Debug.LogError("BGM Source belum dihubungkan!");
            return;
        }

        bgmSource.loop = true;

        if (audioFadeController != null)
        {
            audioFadeController.FadeIn();
        }
        else
        {
            Debug.LogError(
                "AudioFadeController belum dihubungkan!"
            );
        }
    }

    public void StopBGM()
    {
        if (isStopping)
            return;

        isStopping = true;

        if (audioFadeController != null)
        {
            audioFadeController.FadeOut();
            StartCoroutine(DestroyAfterFade());
        }
        else
        {
            if (bgmSource != null)
                bgmSource.Stop();

            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyAfterFade()
    {
        yield return new WaitForSecondsRealtime(
            fadeOutDestroyDelay
        );

        if (Instance == this)
            Instance = null;

        Destroy(gameObject);
    }
}