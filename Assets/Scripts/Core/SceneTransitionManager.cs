using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    private bool isTransitioning;

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

    public void LoadScene(string sceneName)
    {
        if (isTransitioning)
            return;

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("Nama scene belum diisi.");
            return;
        }

        if (SceneManager.GetActiveScene().name == sceneName)
        {
            Debug.LogWarning(
                "Scene " + sceneName + " sedang aktif."
            );
            return;
        }

        isTransitioning = true;

        Debug.Log("Berpindah ke scene: " + sceneName);

        SceneManager.LoadScene(
            sceneName,
            LoadSceneMode.Single
        );
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isTransitioning = false;

        Debug.Log("Scene berhasil dimuat: " + scene.name);
    }
}