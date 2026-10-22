using UnityEngine;

public class RoomBackButton : MonoBehaviour
{
    [Header("Scene Tujuan")]
    [SerializeField] private string targetSceneName;

    public void GoBack()
    {
        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            Debug.LogError(
                "Nama scene tujuan belum diisi pada " + gameObject.name
            );
            return;
        }

        if (SceneTransitionManager.Instance == null)
        {
            Debug.LogError(
                "SceneTransitionManager tidak ditemukan."
            );
            return;
        }

        SceneTransitionManager.Instance.LoadScene(targetSceneName);
    }
}