
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
        }
        else
        {
            // Fase bahaya berakhir.
            // Periksa apakah Arka sedang bersembunyi.

            if (!isPlayerHiding)
            {
                Debug.Log("Arka tidak bersembunyi!");

                StopChase();

                OnChaseFailed?.Invoke();

                HandleChaseFailed();

                return;
            }

            // Arka berhasil selamat.
            isDangerPhase = false;
            phaseTimer = safeDuration;

            OnPhaseChanged?.Invoke(isDangerPhase);

            Debug.Log("Arka berhasil selamat dari Mayang.");
            Debug.Log("Fase aman dimulai kembali.");
        }
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

        // Tandai bahwa pemain harus mengulang pengejaran.
        NiskalaProgress.pendingChaseRetryIntro = true;

        Debug.Log(
            "Arka tertangkap. Kembali ke checkpoint: " +
            checkpoint
        );

        // Kembali ke scene checkpoint.
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(checkpoint);
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
