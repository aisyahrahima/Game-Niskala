using UnityEngine;

public class RoomController : MonoBehaviour
{
    private int activeInteractionCount;

    public bool IsBusy => activeInteractionCount > 0;

    private void OnEnable()
    {
        GameEvents.InteractionStarted += HandleInteractionStarted;
        GameEvents.InteractionFinished += HandleInteractionFinished;
    }

    private void OnDisable()
    {
        GameEvents.InteractionStarted -= HandleInteractionStarted;
        GameEvents.InteractionFinished -= HandleInteractionFinished;

        activeInteractionCount = 0;
    }

    private void HandleInteractionStarted()
    {
        activeInteractionCount++;

        Debug.Log(
            "RoomController: Interaksi dimulai. " +
            "Jumlah interaksi aktif: " +
            activeInteractionCount
        );
    }

    private void HandleInteractionFinished()
    {
        if (activeInteractionCount <= 0)
        {
            Debug.LogWarning(
                "RoomController: InteractionFinished diterima " +
                "ketika tidak ada interaksi aktif."
            );

            return;
        }

        activeInteractionCount--;

        Debug.Log(
            "RoomController: Interaksi selesai. " +
            "Jumlah interaksi aktif: " +
            activeInteractionCount
        );
    }
}