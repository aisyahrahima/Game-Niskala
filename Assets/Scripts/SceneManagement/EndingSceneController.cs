
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndingSceneController : MonoBehaviour
{
    [Header("Narasi Ending")]
    [SerializeField] private TMP_Text endingText;

    [TextArea(5, 15)]
    [SerializeField] private string endingNarration;

    [SerializeField] private float characterDelay = 0.04f;

    [Header("Tombol Main Menu")]
    [SerializeField] private Button mainMenuButton;

    private bool narrationFinished;

    private void Awake()
    {
        if (endingText != null)
            endingText.text = "";

        if (mainMenuButton != null)
        {
            mainMenuButton.gameObject.SetActive(false);
            mainMenuButton.onClick.AddListener(BackToMainMenu);
        }
    }

    private void Start()
    {
        if (endingText == null || mainMenuButton == null)
        {
            Debug.LogError(
                "EndingText atau MainMenuButton belum dihubungkan."
            );
            return;
        }

        StartCoroutine(PlayEndingNarration());
    }

    private IEnumerator PlayEndingNarration()
    {
        narrationFinished = false;
        endingText.text = "";

        foreach (char character in endingNarration)
        {
            endingText.text += character;

            yield return new WaitForSecondsRealtime(
                characterDelay
            );
        }

        narrationFinished = true;

        mainMenuButton.gameObject.SetActive(true);

        Debug.Log("Narasi Ending selesai.");
    }

    private void BackToMainMenu()
    {
        if (!narrationFinished)
            return;

        // Hentikan dan hancurkan BGM melankolik.
        if (EndingMelancholicBGM.Instance != null)
        {
            EndingMelancholicBGM.Instance.StopBGM();
        }

        if (SceneTransitionManager.Instance != null)
        {
            
            SceneTransitionManager.Instance.LoadScene("MainMenu");
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
        if (mainMenuButton != null)
            mainMenuButton.onClick.RemoveListener(BackToMainMenu);
    }
}
