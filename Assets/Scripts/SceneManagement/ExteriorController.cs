
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

        // Memulai ambience malam.
        if (ambienceMalamSource != null)
        {
            ambienceMalamSource.loop = true;
            ambienceMalamSource.Play();
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
        if (ambienceMalamSource != null)
            ambienceMalamSource.Stop();

        if (ambienceGledekSource != null)
            ambienceGledekSource.Stop();

        if (interactionSource != null && doorOpenSound != null)
        {
            interactionSource.PlayOneShot(doorOpenSound);

            yield return new WaitForSeconds(doorOpenSound.length);
        }

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
