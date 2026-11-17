
using System;
using UnityEngine;

public class MayangChaseManager : MonoBehaviour
{
    public static MayangChaseManager Instance { get; private set; }

    [Header("Pengaturan Waktu")]
    [SerializeField] private float safeDuration = 10f;
    [SerializeField] private float dangerDuration = 8f;

    private float phaseTimer;
    private bool chaseRunning;
    private bool isDangerPhase;
    private bool isPlayerHiding;
    private bool failureSequenceActive;

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
        phaseTimer = safeDuration;

        OnPhaseChanged?.Invoke(isDangerPhase);

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

        Debug.Log("Pengejaran Mayang dihentikan.");
    }
}
