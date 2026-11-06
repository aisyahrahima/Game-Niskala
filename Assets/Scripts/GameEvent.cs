using System;

public static class GameEvents
{
    // Item dan inventory
    public static event Action<string> ItemCollected;

    // Dialog
    public static event Action DialogueStarted;
    public static event Action DialogueFinished;

    // Interaksi umum
    public static event Action InteractionStarted;
    public static event Action InteractionFinished;

    // Perpindahan scene
    public static event Action<string> SceneTransitionStarted;

    public static void RaiseItemCollected(string itemId)
    {
        ItemCollected?.Invoke(itemId);
    }

    public static void RaiseDialogueStarted()
    {
        DialogueStarted?.Invoke();
    }

    public static void RaiseDialogueFinished()
    {
        DialogueFinished?.Invoke();
    }

    public static void RaiseInteractionStarted()
    {
        InteractionStarted?.Invoke();
    }

    public static void RaiseInteractionFinished()
    {
        InteractionFinished?.Invoke();
    }

    public static void RaiseSceneTransitionStarted(string sceneName)
    {
        SceneTransitionStarted?.Invoke(sceneName);
    }
}