using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Settings Popup")]
    [SerializeField] private GameObject settingsPopup;

    [Header("Exit Confirmation Popup")]
    [SerializeField] private GameObject exitConfirmPopup;

    [Header("Audio Settings")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text volumePercentage;

    [Header("Brightness Settings")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private TMP_Text brightnessPercentage;
    [SerializeField] private Image brightnessOverlay;

    private void Awake()
    {
        // Pastikan popup tidak aktif saat MainMenu dibuka.
        if (settingsPopup != null)
            settingsPopup.SetActive(false);

        if (exitConfirmPopup != null)
            exitConfirmPopup.SetActive(false);
    }

    private void Start()
    {
        // Mengambil nilai volume yang tersimpan.
        float savedVolume = PlayerPrefs.GetFloat("GameVolume", 100f);

        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0;
            volumeSlider.maxValue = 100;
            volumeSlider.wholeNumbers = true;

            volumeSlider.SetValueWithoutNotify(savedVolume);
            SetVolume(savedVolume);

            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        // Mengambil nilai kecerahan yang tersimpan.
        float savedBrightness = PlayerPrefs.GetFloat("GameBrightness", 100f);

        if (brightnessSlider != null)
        {
            brightnessSlider.minValue = 0;
            brightnessSlider.maxValue = 100;
            brightnessSlider.wholeNumbers = true;

            brightnessSlider.SetValueWithoutNotify(savedBrightness);
            SetBrightness(savedBrightness);

            brightnessSlider.onValueChanged.AddListener(SetBrightness);
        }
    }

    private void SetVolume(float value)
    {
        AudioListener.volume = value / 100f;

        if (volumePercentage != null)
            volumePercentage.text = Mathf.RoundToInt(value) + "%";

        PlayerPrefs.SetFloat("GameVolume", value);
        PlayerPrefs.Save();
    }

    private void SetBrightness(float value)
    {
        if (brightnessOverlay != null)
        {
            Color color = brightnessOverlay.color;

            // Transparansi hitam maksimal hanya 50%.
            color.a = (1f - (value / 100f)) * 0.8f;

            brightnessOverlay.color = color;
        }

        if (brightnessPercentage != null)
            brightnessPercentage.text = Mathf.RoundToInt(value) + "%";

        PlayerPrefs.SetFloat("GameBrightness", value);
        PlayerPrefs.Save();
    }

    public void StartGame()
    {
        LoadScene("ChapterMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {
        if (settingsPopup != null)
            settingsPopup.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPopup != null)
            settingsPopup.SetActive(false);
    }

    public void OpenExitConfirm()
    {
        if (exitConfirmPopup != null)
            exitConfirmPopup.SetActive(true);
    }

    public void CancelExit()
    {
        if (exitConfirmPopup != null)
            exitConfirmPopup.SetActive(false);
    }

    private void LoadScene(string sceneName)
    {
        if (SceneTransitionManager.Instance == null)
        {
            Debug.LogError(
                "SceneTransitionManager tidak ditemukan."
            );

            return;
        }

        SceneTransitionManager.Instance.LoadScene(sceneName);
    }
}