using UnityEngine;

public class DoorSceneTransition : MonoBehaviour
{
    [Header("Tujuan Pintu")]
    [SerializeField] private string targetSceneName;

    [Header("Pengaturan")]
    [SerializeField] private bool transitionEnabled = true;

    public void EnterDoor()
    {
        if (!transitionEnabled)
            return;

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