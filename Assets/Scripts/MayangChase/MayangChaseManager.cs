
using System;
using UnityEngine;

public class MayangChaseManager : MonoBehaviour
{
    public static MayangChaseManager Instance { get; private set; }

    [Header("Pengaturan Waktu")]
    [SerializeField] private float safeDuration = 10f;
    [SerializeField] private float dangerDuration = 8f;

    [Header("Mayang Chase Audio")]
    [SerializeField] private AudioSource chaseBGMSource;
    [SerializeField] private AudioFadeController chaseBGMFadeController;
    [SerializeField] private AudioSource heartbeatSource;
    [SerializeField] private AudioSource warningSource;
    [SerializeField] private AudioSource ancangAncangSource;
    [SerializeField] private AudioSource jumpscareSource;

    private float phaseTimer;
    private bool chaseRunning;
    private bool isDangerPhase;
    private bool isPlayerHiding;
    private bool failureSequenceActive;
    private bool warningPlayedThisSafePhase = false;

    public bool IsChaseRunning => chaseRunning;
    public bool IsDangerPhase => isDangerPhase;
    public bool IsPlayerHiding => isPlayerHiding;

    public float SafeDuration => safeDuration;

    public float PhaseTimeRemaining =>
        Mathf.Max(0f, phaseTimer);

    public event Action<bool> OnPhaseChanged;
    public event Action OnChaseFailed;

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

    private void Update()
    {
        if (!chaseRunning)
            return;

        phaseTimer -= Time.deltaTime;

        // Putar warning saat fase aman hampir berakhir.
        if (!isDangerPhase &&
            !warningPlayedThisSafePhase &&
            phaseTimer <= 3f)
        {
            if (warningSource != null)
                warningSource.Play();

            warningPlayedThisSafePhase = true;
        }

        if (phaseTimer <= 0f)
        {
            ChangePhase();
        }
    }

    public void StartChase()
    {
        if (chaseRunning)
            return;

        chaseRunning = true;
        isDangerPhase = false;
        isPlayerHiding = false;
        failureSequenceActive = false;
        warningPlayedThisSafePhase = false;
        if (warningSource != null && warningSource.isPlaying)
            warningSource.Stop();
        phaseTimer = safeDuration;

        OnPhaseChanged?.Invoke(isDangerPhase);

        // Hentikan ambience hujan jika berada di Ruang Penyimpanan.
        if (StorageRainAmbience.Instance != null)
        {
            StorageRainAmbience.Instance.StopRain();
        }

        // Putar musik latar pengejaran dengan fade-in.
        if (chaseBGMSource != null)
        {
            if (chaseBGMFadeController != null)
            {
                chaseBGMFadeController.FadeIn();
            }
            else
            {
                chaseBGMSource.Play();
            }
        }

        Debug.Log("Pengejaran Mayang dimulai.");
        Debug.Log("Fase aman: " + safeDuration + " detik.");
    }

    public void SetPlayerHiding(bool hiding)
    {
        isPlayerHiding = hiding;

        Debug.Log(
            "Status persembunyian Arka: " +
            (isPlayerHiding ? "Bersembunyi" : "Tidak bersembunyi")
        );

        // Jika Arka keluar saat fase bahaya berlangsung,
        // langsung picu kegagalan pengejaran.
        if (!isPlayerHiding &&
            chaseRunning &&
            isDangerPhase)
        {
            TriggerChaseFailure();
        }
    }

    private void TriggerChaseFailure()
    {
        // Pastikan pengejaran masih berjalan
        // dan kegagalan belum pernah dipicu.
        if (!chaseRunning || failureSequenceActive)
            return;

        Debug.Log("Arka tertangkap Mayang!");

        // Hentikan pengejaran.
        StopChase();

        // Tandai bahwa animasi kegagalan sedang berlangsung.
        failureSequenceActive = true;

        // Jalankan animasi jumpscare.
        OnChaseFailed?.Invoke();
    }

    private void ChangePhase()
    {
        if (!isDangerPhase)
        {
            // Fase aman berubah menjadi fase bahaya.
            isDangerPhase = true;
            phaseTimer = dangerDuration;

            OnPhaseChanged?.Invoke(isDangerPhase);

            // Putar suara detak jantung.
            if (heartbeatSource != null && !heartbeatSource.isPlaying)
                heartbeatSource.Play();

            // Hentikan warning sound.
            if (warningSource != null && warningSource.isPlaying)
                warningSource.Stop();

            Debug.Log("Fase bahaya dimulai.");

            // Periksa apakah Arka bersembunyi
            // ketika fase bahaya dimulai.
            if (!isPlayerHiding)
            {
                TriggerChaseFailure();
                return;
            }
        }
        else
        {
            // Fase bahaya selesai dan Arka berhasil selamat.
            isDangerPhase = false;
            phaseTimer = safeDuration;

            OnPhaseChanged?.Invoke(isDangerPhase);

            // Hentikan suara detak jantung.
            if (heartbeatSource != null && heartbeatSource.isPlaying)
                heartbeatSource.Stop();

            // Reset warning untuk fase aman berikutnya.
            warningPlayedThisSafePhase = false;

            Debug.Log("Arka berhasil selamat dari Mayang.");
            Debug.Log("Fase aman dimulai kembali.");
        }
    }

    public void CompleteChaseFailure()
    {
        if (!failureSequenceActive)
            return;

        failureSequenceActive = false;

        Debug.Log("Animasi jumpscare selesai.");

        HandleChaseFailed();
    }

    private void HandleChaseFailed()
    {
        // Ambil nama scene checkpoint.
        string checkpoint =
            NiskalaProgress.chaseCheckpointScene;

        // Pastikan checkpoint tersedia.
        if (string.IsNullOrWhiteSpace(checkpoint))
        {
            Debug.LogError(
                "Checkpoint pengejaran belum ditentukan."
            );

            return;
        }

        // Reset seluruh status pengembalian artefak.
        NiskalaProgress.tepakSirihReturned = false;
        NiskalaProgress.bakulReturned = false;
        NiskalaProgress.catatanCikSimaReturned = false;

        Debug.Log(
            "Seluruh status pengembalian artefak direset."
        );

        // Tandai bahwa pemain harus mengulang pengejaran.
        NiskalaProgress.pendingChaseRetryIntro = true;

        Debug.Log(
            "Arka tertangkap. Kembali ke checkpoint: " +
            checkpoint
        );

        // Kembali ke scene checkpoint.
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.ReloadScene(checkpoint);
        }
        else
        {
            Debug.LogError(
                "SceneTransitionManager tidak ditemukan."
            );
        }
    }

    public void StopChase()
    {
        chaseRunning = false;
        phaseTimer = 0f;

        // Hentikan BGM pengejaran dengan fade-out.
        if (chaseBGMSource != null &&
            chaseBGMSource.isPlaying)
        {
            if (chaseBGMFadeController != null)
            {
                chaseBGMFadeController.FadeOut();
            }
            else
            {
                chaseBGMSource.Stop();
            }
        }

        // Hentikan warning.
        if (warningSource != null && warningSource.isPlaying)
            warningSource.Stop();

        // Hentikan heartbeat.
        if (heartbeatSource != null && heartbeatSource.isPlaying)
            heartbeatSource.Stop();

        Debug.Log("Pengejaran Mayang dihentikan.");
    }

    public void StopAllChaseAudio()
    {
        // Hentikan BGM pengejaran dengan fade-out.
        if (chaseBGMSource != null &&
            chaseBGMSource.isPlaying)
        {
            if (chaseBGMFadeController != null)
            {
                chaseBGMFadeController.FadeOut();
            }
            else
            {
                chaseBGMSource.Stop();
            }
        }

        // Hentikan heartbeat.
        if (heartbeatSource != null)
            heartbeatSource.Stop();

        // Hentikan warning.
        if (warningSource != null)
            warningSource.Stop();

        // Hentikan audio ancang-ancang.
        if (ancangAncangSource != null)
            ancangAncangSource.Stop();

        // Hentikan audio jumpscare.
        if (jumpscareSource != null)
            jumpscareSource.Stop();

        Debug.Log("Seluruh audio pengejaran Mayang dihentikan.");
    }

    public void ResetChaseState()
    {
        // Hentikan seluruh audio pengejaran.
        StopAllChaseAudio();

        // Reset status pengejaran.
        chaseRunning = false;
        isDangerPhase = false;
        isPlayerHiding = false;
        failureSequenceActive = false;
        warningPlayedThisSafePhase = false;

        // Reset waktu fase.
        phaseTimer = 0f;

        // Beri tahu sistem visual bahwa fase kembali aman.
        OnPhaseChanged?.Invoke(false);

        Debug.Log("Status pengejaran Mayang berhasil direset.");
    }

    public void PlayJumpscareSound()
    {
        if (jumpscareSource != null)
            jumpscareSource.Play();
    }

    public void PlayAncangAncangSound()
    {
        if (ancangAncangSource != null)
            ancangAncangSource.Play();
    }

    public void StopAncangAncangSound()
    {
        if (ancangAncangSource != null &&
            ancangAncangSource.isPlaying)
        {
            ancangAncangSource.Stop();
        }
    }

    public void StopChaseBGM()
    {
        if (chaseBGMSource != null &&
            chaseBGMSource.isPlaying)
        {
            if (chaseBGMFadeController != null)
            {
                chaseBGMFadeController.FadeOut();
            }
            else
            {
                chaseBGMSource.Stop();
            }
        }
    }

}
