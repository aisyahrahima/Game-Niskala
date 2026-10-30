using UnityEngine;

public class DoorSceneTransition : MonoBehaviour
{
    [Header("Tujuan Pintu")]
    [SerializeField] private string targetSceneName;

    [Header("Pengaturan")]
    [SerializeField] private bool transitionEnabled = true;

    [Header("Pengecekan Dialog")]
    [SerializeField] private DialogueSystem dialogueSystem;

    [Header("Pengecekan Interaksi Ruangan")]
    [SerializeField] private MonoBehaviour interactionController;

    public void EnterDoor()
    {
        if (!transitionEnabled)
            return;

        // Periksa apakah dialog sedang berlangsung.
        if (dialogueSystem != null &&
            dialogueSystem.IsDialogueActive)
        {
            Debug.Log("Pintu tidak dapat digunakan karena dialog masih berlangsung.");
            return;
        }

        // Periksa apakah interaksi ruangan sedang berlangsung.
        if (interactionController != null)
        {
            IInteractionBlocker blocker =
                interactionController as IInteractionBlocker;

            if (blocker == null)
            {
                Debug.LogError(
                    "Interaction Controller tidak memiliki IInteractionBlocker."
                );
                return;
            }

            if (blocker.IsInteractionBlocked())
            {
                Debug.Log("Pintu tidak dapat digunakan karena interaksi masih berlangsung.");
                return;
            }
        }

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