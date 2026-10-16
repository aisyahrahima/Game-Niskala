using UnityEngine;

public class RoomController : MonoBehaviour
{
    public bool IsBusy { get; private set; }

    public bool TryBeginInteraction()
    {
        if (IsBusy)
            return false;

        IsBusy = true;
        return true;
    }

    public void EndInteraction()
    {
        IsBusy = false;
    }
}
