using UnityEngine;
using UnityEngine.Video;

public class LoreFragmentPlayer : MonoBehaviour
{
    [Header("Video Player")]
    [SerializeField] private VideoPlayer videoPlayer;

    private bool hasReturned;

    private void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogError(
                "VideoPlayer belum dihubungkan."
            );
            return;
        }

        // Video tidak boleh diputar berulang.
        videoPlayer.isLooping = false;

        // Daftarkan event ketika video selesai.
        videoPlayer.loopPointReached += OnVideoFinished;

        // Daftarkan event jika video gagal diputar.
        videoPlayer.errorReceived += OnVideoError;

        // Siapkan video sebelum mulai diputar.
        videoPlayer.prepareCompleted += OnVideoPrepared;

        videoPlayer.Prepare();
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        Debug.Log(
            "Video LoreFragment siap diputar."
        );

        source.Play();
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        ReturnToPreviousRoom();
    }

    private void OnVideoError(
        VideoPlayer source,
        string message)
    {
        Debug.LogError(
            "Terjadi kesalahan saat memutar video: " + message
        );

        // Kembali ke ruangan asal agar pemain tidak terjebak.
        ReturnToPreviousRoom();
    }

    private void ReturnToPreviousRoom()
    {
        if (hasReturned)
            return;

        hasReturned = true;

        if (!ArtifactLoreFlow.pendingReturn)
        {
            Debug.LogError(
                "Data scene asal tidak ditemukan."
            );
            return;
        }

        string targetScene =
            ArtifactLoreFlow.returnSceneName;

        Debug.Log(
            "Video selesai. Kembali ke: " + targetScene
        );

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(
                targetScene
            );
        }
        else
        {
            Debug.LogError(
                "SceneTransitionManager tidak ditemukan."
            );
        }
    }

    private void OnDestroy()
    {
        if (videoPlayer == null)
            return;

        videoPlayer.loopPointReached -= OnVideoFinished;
        videoPlayer.errorReceived -= OnVideoError;
        videoPlayer.prepareCompleted -= OnVideoPrepared;
    }
}