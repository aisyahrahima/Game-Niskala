using UnityEngine;
using UnityEngine.Video;

public class FullLorePlayer : MonoBehaviour
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

        videoPlayer.isLooping = false;

        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.errorReceived += OnVideoError;
        videoPlayer.prepareCompleted += OnVideoPrepared;

        videoPlayer.Prepare();
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        Debug.Log("Video FullLore siap diputar.");

        source.Play();
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        ReturnToLastRoom();
    }

    private void OnVideoError(
        VideoPlayer source,
        string message)
    {
        Debug.LogError(
            "Terjadi kesalahan saat memutar FullLore: " + message
        );

        ReturnToLastRoom();
    }

    private void ReturnToLastRoom()
    {
        if (hasReturned)
            return;

        hasReturned = true;

        if (!LoreOrderFlow.pendingFullLoreReturn)
        {
            Debug.LogError(
                "Data ruangan terakhir tidak ditemukan."
            );

            return;
        }

        string targetScene =
            LoreOrderFlow.returnRoomAfterFullLore;

        if (string.IsNullOrWhiteSpace(targetScene))
        {
            Debug.LogError(
                "Nama ruangan terakhir kosong."
            );

            return;
        }

        Debug.Log(
            "FullLore selesai. Kembali ke: " + targetScene
        );

        // Bersihkan data setelah nama scene tujuan disimpan.
        LoreOrderFlow.Clear();

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