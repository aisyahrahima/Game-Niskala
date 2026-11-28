
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ExteriorController : MonoBehaviour
{
    [Header("Dialog")]
    [SerializeField] private DialogueSystem dialogueSystem;

    [Header("Tombol Masuk Bangunan")]
    [SerializeField] private Button enterButton;

    [Header("Pengaturan Scene")]
    [SerializeField] private string nextSceneName = "RuangTamu";
    [SerializeField] private float dialogueStartDelay = 2f;

    [Header("Audio Ambience")]
    [SerializeField] private AudioSource ambienceMalamSource;
    [SerializeField] private AudioSource ambienceGledekSource;

    [Header("Fade Ambience Malam")]
    [SerializeField] private AudioFadeController ambienceMalamFade;

    [Header("Audio Interaksi")]
    [SerializeField] private AudioSource interactionSource;
    [SerializeField] private AudioClip doorOpenSound;

    private bool dialogueFinished;
    private bool isTransitioning;

    private void Awake()
    {
        if (enterButton != null)
        {
            enterButton.gameObject.SetActive(false);
            enterButton.onClick.AddListener(EnterBuilding);
        }
    }

    private void Start()
    {
        if (dialogueSystem == null)
        {
            Debug.LogError(
                "DialogueSystem belum dihubungkan pada ExteriorController."
            );
            return;
        }

        if (enterButton == null)
        {
            Debug.LogError(
                "EnterButton belum dihubungkan pada ExteriorController."
            );
            return;
        }

        // Memulai ambience malam dengan fade-in.
        if (ambienceMalamSource != null)
        {
            ambienceMalamSource.loop = true;

            if (ambienceMalamFade != null)
            {
                ambienceMalamFade.FadeIn();
            }
            else
            {
                ambienceMalamSource.Play();

                Debug.LogWarning(
                    "AudioFadeController Ambience Malam belum dihubungkan."
                );
            }
        }

        // Memulai ambience gledek.
        if (ambienceGledekSource != null)
        {
            ambienceGledekSource.loop = true;
            ambienceGledekSource.Play();
        }

        StartCoroutine(ExteriorSequence());
    }

    private IEnumerator ExteriorSequence()
    {
        // Jeda sebelum dialog pembuka Exterior.
        yield return new WaitForSeconds(dialogueStartDelay);

        Debug.Log("Dialog pembuka Exterior dimulai.");

        bool started = dialogueSystem.StartDialogue(
            "Arka",
            "Jadi... ini bangunannya.",
            "Dari luar saja sudah kelihatan tua banget.",
            "Katanya tempat ini sudah lama kosong.",
            "Tapi masih banyak cerita aneh yang beredar tentang tempat ini.",
            "Kalau memang ada sesuatu di dalam, aku harus tahu apa itu.",
            "Dan..Sepertinya sebentar lagi hujan. Aku harus cepat-cepat masuk."
        );

        if (!started)
        {
            Debug.LogError(
                "Dialog pembuka Exterior gagal dimulai."
            );
            yield break;
        }

        // Menunggu sampai seluruh dialog selesai.
        while (dialogueSystem.IsDialogueActive)
        {
            yield return null;
        }

        dialogueFinished = true;

        // Tampilkan tombol masuk setelah dialog selesai.
        enterButton.gameObject.SetActive(true);

        Debug.Log("Dialog Exterior selesai. EnterButton ditampilkan.");
    }

        public void EnterBuilding()
    {
        if (!dialogueFinished || isTransitioning)
            return;

        isTransitioning = true;

        StartCoroutine(EnterBuildingSequence());
    }

    private IEnumerator EnterBuildingSequence()
    {
        // Memulai fade-out ambience malam.
        if (ambienceMalamSource != null)
        {
            if (ambienceMalamFade != null)
            {
                ambienceMalamFade.FadeOut();
            }
            else
            {
                ambienceMalamSource.Stop();
            }
        }

        // Hentikan ambience gledek seperti sebelumnya.
        if (ambienceGledekSource != null)
            ambienceGledekSource.Stop();

        // Putar suara pintu langsung tanpa menunggu fade-out.
        float doorSoundDuration = 0f;

        if (interactionSource != null && doorOpenSound != null)
        {
            interactionSource.PlayOneShot(doorOpenSound);

            doorSoundDuration = doorOpenSound.length;
        }

        // Tunggu sampai fade-out ambience dan suara pintu selesai.
        float waitDuration = Mathf.Max(
            1.5f,
            doorSoundDuration
        );

        yield return new WaitForSecondsRealtime(waitDuration);

        // Pindah ke scene RuangTamu.
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(nextSceneName);
        }
        else
        {
            isTransitioning = false;

            Debug.LogError(
                "SceneTransitionManager tidak ditemukan."
            );
        }
    }

    private void OnDestroy()
    {
        if (enterButton != null)
            enterButton.onClick.RemoveListener(EnterBuilding);

        // Memastikan ambience berhenti saat scene dihancurkan.
        if (ambienceMalamSource != null)
            ambienceMalamSource.Stop();

        if (ambienceGledekSource != null)
            ambienceGledekSource.Stop();
    }
}
