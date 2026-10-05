using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button nextButton;

    [Header("Typewriter Settings")]
    [SerializeField] private float characterDelay = 0.035f;

    private Coroutine typingCoroutine;
    private string currentLine = "";
    private bool isTyping;

    public bool IsTyping => isTyping;

    private void Awake()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    public void ShowLine(string speaker, string line)
    {
        Debug.Log(
            "DIALOGUE UI: ShowLine dipanggil. Speaker = " +
            speaker +
            ", Line = " +
            line
        );

        if (dialoguePanel == null ||
            nameText == null ||
            dialogueText == null)
        {
            Debug.LogError(
                "DIALOGUE UI: Referensi UI dialog belum lengkap."
            );

            return;
        }

        Debug.Log(
            "DIALOGUE UI: DialoguePanel ditemukan: " +
            dialoguePanel.name
        );

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        nameText.text = speaker;
        currentLine = line;
        dialogueText.text = "";

        Debug.Log(
            "DIALOGUE UI: Sebelum SetActive = " +
            dialoguePanel.activeSelf
        );

        dialoguePanel.SetActive(true);

        Debug.Log(
            "DIALOGUE UI: Setelah SetActive(true) = " +
            dialoguePanel.activeSelf +
            " | activeInHierarchy = " +
            dialoguePanel.activeInHierarchy
        );

        Debug.Log(
            "DIALOGUE UI: Sesudah SetActive = " +
            dialoguePanel.activeSelf
        );

        if (nextButton != null)
            nextButton.interactable = true;

        typingCoroutine = StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char character in currentLine)
        {
            dialogueText.text += character;
            yield return new WaitForSecondsRealtime(characterDelay);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    public bool TryCompleteTyping()
    {
        if (!isTyping)
            return false;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = currentLine;
        typingCoroutine = null;
        isTyping = false;

        return true;
    }

    public void Hide()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = null;
        isTyping = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }
}