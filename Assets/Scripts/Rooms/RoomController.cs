using UnityEngine;

public class RoomController : MonoBehaviour
{
    public bool IsBusy { get; private set; }

    private void OnEnable()
    {
        GameEvents.InteractionStarted += HandleInteractionStarted;
        GameEvents.InteractionFinished += HandleInteractionFinished;
    }

    private void OnDisable()
    {
        GameEvents.InteractionStarted -= HandleInteractionStarted;
        GameEvents.InteractionFinished -= HandleInteractionFinished;

        IsBusy = false;
    }

    private void HandleInteractionStarted()
    {
        IsBusy = true;

        Debug.Log("RoomController: Interaksi dimulai.");
    }

    private void HandleInteractionFinished()
    {
        IsBusy = false;

        Debug.Log("RoomController: Interaksi selesai.");
    }
}