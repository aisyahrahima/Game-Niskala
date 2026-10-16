
using System;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; }

    public static event Action DialogueStarted;
    public static event Action DialogueFinished;

    [Header("UI")]
    [SerializeField] private DialogueUI dialogueUI;

    private string currentSpeaker;
    private string[] currentLines;
    private int currentLineIndex;
    private bool isDialogueActive;

    public bool IsDialogueActive => isDialogueActive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError(
                "Ada lebih dari satu DialogueSystem dalam scene aktif."
            );
            enabled = false;
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public bool StartDialogue(string speaker, params string[] lines)
    {
        if (isDialogueActive)
        {
            Debug.LogWarning(
                "Dialog baru ditolak karena dialog lain masih berlangsung."
            );
            return false;
        }

        if (dialogueUI == null)
        {
            Debug.LogError(
                "DialogueUI belum dihubungkan ke DialogueSystem."
            );
            return false;
        }

        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("Dialog tidak memiliki baris teks.");
            return false;
        }

        currentSpeaker = speaker;
        currentLines = lines;
        currentLineIndex = 0;
        isDialogueActive = true;

        DialogueStarted?.Invoke();

        ShowCurrentLine();

        return true;
    }

    public void NextLine()
    {
        if (!isDialogueActive)
            return;

        // Klik pertama saat teks sedang berjalan hanya
        // menyelesaikan animasi typewriter.
        if (dialogueUI.IsTyping)
        {
            dialogueUI.TryCompleteTyping();
            return;
        }

        currentLineIndex++;

        if (currentLineIndex >= currentLines.Length)
        {
            FinishDialogue();
            return;
        }

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        dialogueUI.ShowLine(
            currentSpeaker,
            currentLines[currentLineIndex]
        );
    }

    public void FinishDialogue()
    {
        if (!isDialogueActive)
            return;

        isDialogueActive = false;

        dialogueUI.Hide();

        currentSpeaker = null;
        currentLines = null;
        currentLineIndex = 0;

        DialogueFinished?.Invoke();
    }
}
