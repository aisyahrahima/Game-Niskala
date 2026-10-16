using System;

public static class GameEvents
{
    public static event Action<string> ItemCollected;

    public static void RaiseItemCollected(string itemId)
    {
        ItemCollected?.Invoke(itemId);
    }

    public static event Action DialogueStarted;
    public static event Action DialogueFinished;

    public static void RaiseDialogueStarted()
    {
        DialogueStarted?.Invoke();
    }

    public static void RaiseDialogueFinished()
    {
        DialogueFinished?.Invoke();
    }
}