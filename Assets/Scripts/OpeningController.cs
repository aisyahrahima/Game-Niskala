using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpeningController : MonoBehaviour
{
    [Header("Narasi")]
    [SerializeField] private TMP_Text narrationText;
    [SerializeField] private float characterDelay = 0.04f;

    [Header("Tombol Lanjut")]
    [SerializeField] private Button continueButton;

    [Header("Pengaturan Scene")]
    [SerializeField] private string nextSceneName = "Exterior";

    private string fullNarration;
    private Coroutine typingCoroutine;
    private bool narrationFinished;

    private void Awake()
    {
        fullNarration =
            "Aku berdiri di depan sebuah bangunan yang telah lama dibiarkan kosong. " +
            "Cat yang mengelupas dan jendela-jendela berdebu menjadi satu-satunya tanda bahwa tempat itu pernah dihuni. " +
            "Sebagai seorang jurnalis, aku terbiasa mencari sesuatu yang tersembunyi di balik hal-hal yang tidak banyak diketahui orang.\n\n" +
            "Beberapa hari terakhir, aku mendengar kabar tentang bangunan ini, yang membuat orang-orang enggan mendekatinya. " +
            "Malam ini, aku ingin mengetahui alasannya.";

        if (narrationText != null)
            narrationText.text = "";

        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(false);
            continueButton.onClick.AddListener(ContinueToExterior);
        }
    }

    private void Start()
    {
        if (narrationText == null || continueButton == null)
        {
            Debug.LogError(
                "NarrationText atau ContinueButton belum dihubungkan."
            );
            return;
        }

        typingCoroutine = StartCoroutine(TypeNarration());
    }

    private IEnumerator TypeNarration()
    {
        narrationFinished = false;
        narrationText.text = "";

        // Jeda 2 detik sebelum narasi dimulai
        yield return new WaitForSecondsRealtime(2f);

        foreach (char character in fullNarration)
        {
            narrationText.text += character;
            yield return new WaitForSecondsRealtime(characterDelay);
        }

        narrationFinished = true;
        typingCoroutine = null;
        continueButton.gameObject.SetActive(true);
    }

    private void ContinueToExterior()
    {
        if (!narrationFinished)
            return;

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(nextSceneName);
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
        if (continueButton != null)
            continueButton.onClick.RemoveListener(ContinueToExterior);
    }
}