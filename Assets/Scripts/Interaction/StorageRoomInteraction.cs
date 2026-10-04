
using System.Collections;
using UnityEngine;

public class StorageRoomInteraction : MonoBehaviour
{
    [Header("Catatan")]
    [SerializeField] private GameObject catatan;
    [SerializeField] private GameObject catatanZoom;
    [SerializeField] private GameObject arrowBack;

    [Header("Dialog")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject dialogueNextButton;
    [SerializeField] private DialogueUI dialogueUI;

    [Header("Gentong")]
    [SerializeField] private GameObject gentong;
    [SerializeField] private GameObject jumpscare;
    [SerializeField] private GameObject bakul;

    [Header("Animasi Jumpscare")]
    [SerializeField] private CanvasGroup jumpscareCanvasGroup;
    [SerializeField] private float jumpscareFadeInDuration = 0.2f;
    [SerializeField] private float jumpscareHoldDuration = 1.1f;
    [SerializeField] private float jumpscareFadeOutDuration = 0.4f;

    [Header("Animasi Gentong")]
    [SerializeField] private RectTransform gentongRectTransform;
    [SerializeField] private CanvasGroup gentongCanvasGroup;
    [SerializeField] private Vector2 gentongShiftedPosition;
    [SerializeField] private float gentongFadeDuration = 0.5f;

    [Header("Bakul")]
    [SerializeField] private GameObject bakulZoom;
    [SerializeField] private GameObject bakulInventory;
    [SerializeField] private GameObject buttonInvestigasiArtefak;

    private bool eventRunning;
    private string currentEvent = "";
    private int gentongClickCount;
    private int catatanDialogueIndex;
    private bool bakulUnlocked;

    private bool IsInteractionBlocked()
    {
        // Jangan izinkan interaksi saat event atau dialog berlangsung.
        if (eventRunning)
            return true;

        // Jangan izinkan interaksi saat zoom Catatan terbuka.
        if (catatanZoom != null && catatanZoom.activeSelf)
            return true;

        // Jangan izinkan interaksi objek lain saat zoom Bakul terbuka.
        if (bakulZoom != null && bakulZoom.activeSelf)
            return true;

        return false;
    }

    private readonly string[] catatanDialogue =
    {
        "hmm...",
        "Sepertinya ada maksud dari dokumen ini..."
    };

    private void Awake()
    {
        // Sembunyikan dialog saat scene mulai.
        HideDialogue();

        if (jumpscare != null)
        {
            jumpscare.SetActive(false);

            if (jumpscareCanvasGroup == null)
                jumpscareCanvasGroup =
                    jumpscare.GetComponent<CanvasGroup>();
        }

        if (catatanZoom != null)
            catatanZoom.SetActive(false);

        if (bakulZoom != null)
            bakulZoom.SetActive(false);

        if (buttonInvestigasiArtefak != null)
            buttonInvestigasiArtefak.SetActive(false);

        // Ambil komponen dari objek Gentong jika belum diisi.
        if (gentong != null)
        {
            if (gentongRectTransform == null)
                gentongRectTransform =
                    gentong.GetComponent<RectTransform>();

            if (gentongCanvasGroup == null)
                gentongCanvasGroup =
                    gentong.GetComponent<CanvasGroup>();
        }
    }

    // =====================================================
    // DIALOG
    // =====================================================

    private void ShowDialogue(string speaker, string line)
    {
        if (dialoguePanel == null ||
            dialogueNextButton == null ||
            dialogueUI == null)
        {
            Debug.LogError(
                "Referensi DialoguePanel, DialogueNextButton, " +
                "atau DialogueUI belum lengkap di Inspector."
            );
            return;
        }

        dialoguePanel.SetActive(true);
        dialogueNextButton.SetActive(false);

        dialogueUI.ShowLine(speaker, line);

        // Tombol muncul bersama dialog.
        // Jika teks sedang diketik, klik pertama menyelesaikan ketikan.
        dialogueNextButton.SetActive(true);
    }

    private void HideDialogue()
    {
        if (dialogueNextButton != null)
            dialogueNextButton.SetActive(false);

        if (dialogueUI != null)
            dialogueUI.Hide();

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    public void NextDialogue()
    {
        if (!eventRunning)
            return;

        // Klik pertama menyelesaikan animasi ketik,
        // jika teks masih belum selesai.
        if (dialogueUI != null &&
            dialogueUI.TryCompleteTyping())
        {
            return;
        }

        if (currentEvent == "Catatan")
        {
            NextCatatanDialogue();
            return;
        }

        if (currentEvent.StartsWith("Gentong"))
        {
            FinishGentongEvent();
            return;
        }

        if (currentEvent == "BakulTerhalang")
        {
            FinishBlockedBakulEvent();
        }
    }

    private void FinishCurrentEvent()
    {
        HideDialogue();

        eventRunning = false;
        currentEvent = "";

        if (arrowBack != null)
            arrowBack.SetActive(true);
    }

    // =====================================================
    // CATATAN
    // =====================================================

    public void OpenCatatan()
    {
        if (IsInteractionBlocked())
            return;

        eventRunning = true;
        currentEvent = "Catatan";
        catatanDialogueIndex = 0;

        HideDialogue();

        catatan.SetActive(false);
        catatanZoom.SetActive(true);
        arrowBack.SetActive(false);

        StartCoroutine(StartCatatanDialogue());
    }

    private IEnumerator StartCatatanDialogue()
    {
        yield return new WaitForSeconds(2f);

        if (currentEvent != "Catatan")
            yield break;

        ShowCatatanDialogue();
    }

    private void ShowCatatanDialogue()
    {
        ShowDialogue(
            "Arka",
            catatanDialogue[catatanDialogueIndex]
        );
    }

    private void NextCatatanDialogue()
    {
        if (currentEvent != "Catatan")
            return;

        catatanDialogueIndex++;

        if (catatanDialogueIndex < catatanDialogue.Length)
        {
            ShowCatatanDialogue();
        }
        else
        {
            FinishCatatanEvent();
        }
    }

    private void FinishCatatanEvent()
    {
        Debug.Log("Event Catatan selesai.");
        FinishCurrentEvent();
    }

    public void CloseCatatanZoom()
    {
        if (eventRunning)
            return;

        catatanZoom.SetActive(false);
        catatan.SetActive(true);

        if (arrowBack != null)
            arrowBack.SetActive(true);
    }

    // =====================================================
    // GENTONG
    // =====================================================

    public void ClickGentong()
    {
        if (IsInteractionBlocked())
            return;

        eventRunning = true;
        arrowBack.SetActive(false);

        HideDialogue();

        gentongClickCount++;

        currentEvent = "Gentong" + gentongClickCount;

        Debug.Log("Gentong diklik: " + gentongClickCount);

        StartCoroutine(GentongEvent());
    }

    private IEnumerator GentongEvent()
    {
        switch (gentongClickCount)
        {
            case 1:
                Debug.Log("Event Gentong 1 dimulai.");

                yield return new WaitForSeconds(1f);

                Debug.Log("SFX: Suara benda jatuh");

                yield return new WaitForSeconds(1f);

                ShowDialogue("Arka", "Apa itu?");
                break;

            case 2:
                Debug.Log("Event Gentong 2 dimulai.");

                yield return new WaitForSeconds(1f);

                Debug.Log("SFX: Bisikan 'Mayang...'");

                yield return new WaitForSeconds(1f);

                ShowDialogue("Arka", "Siapa...?");
                break;

            case 3:
                Debug.Log("Event Gentong 3 dimulai.");

                yield return new WaitForSeconds(3f);

                yield return StartCoroutine(PlayJumpscare());

                yield return new WaitForSeconds(1f);

                ShowDialogue("Arka", "Tadi... apa itu?");
                break;

            case 4:
                Debug.Log("Event Gentong 4 dimulai.");

                yield return new WaitForSeconds(0.5f);

                // Gentong bergeser dengan efek fade.
                yield return StartCoroutine(ShiftGentong());

                bakulUnlocked = true;
                bakul.SetActive(true);

                Debug.Log("Gentong bergeser. Bakul terbuka.");

                yield return new WaitForSeconds(1f);

                ShowDialogue("Arka", "Bakul...?");
                break;

            default:
                // Tidak ada event tambahan setelah klik keempat.
                FinishCurrentEvent();
                break;
        }
    }

    private IEnumerator PlayJumpscare()
    {
        if (jumpscare == null || jumpscareCanvasGroup == null)
        {
            Debug.LogError(
                "Jumpscare atau CanvasGroup Jumpscare belum dihubungkan."
            );
            yield break;
        }

        float fadeIn = Mathf.Max(0.01f, jumpscareFadeInDuration);
        float hold = Mathf.Max(0f, jumpscareHoldDuration);
        float fadeOut = Mathf.Max(0.01f, jumpscareFadeOutDuration);

        // Aktifkan jumpscare dalam keadaan transparan.
        jumpscareCanvasGroup.alpha = 0f;
        jumpscare.SetActive(true);

        Debug.Log("JUMPSCARE FADE IN");

        // Fade in: transparan menjadi terlihat.
        float elapsed = 0f;

        while (elapsed < fadeIn)
        {
            elapsed += Time.unscaledDeltaTime;

            jumpscareCanvasGroup.alpha =
                Mathf.Clamp01(elapsed / fadeIn);

            yield return null;
        }

        jumpscareCanvasGroup.alpha = 1f;

        // Tahan jumpscare agar pemain sempat melihatnya.
        yield return new WaitForSecondsRealtime(hold);

        Debug.Log("JUMPSCARE FADE OUT");

        // Fade out: terlihat menjadi transparan.
        elapsed = 0f;

        while (elapsed < fadeOut)
        {
            elapsed += Time.unscaledDeltaTime;

            jumpscareCanvasGroup.alpha =
                1f - Mathf.Clamp01(elapsed / fadeOut);

            yield return null;
        }

        jumpscareCanvasGroup.alpha = 0f;
        jumpscare.SetActive(false);

        Debug.Log("JUMPSCARE SELESAI");
    }

    private IEnumerator ShiftGentong()
    {
        if (gentong == null || gentongRectTransform == null)
        {
            Debug.LogError(
                "Gentong atau RectTransform Gentong belum dihubungkan."
            );
            yield break;
        }

        Vector2 originalPosition =
            gentongRectTransform.anchoredPosition;

        // Tanpa CanvasGroup, geser langsung sebagai fallback.
        if (gentongCanvasGroup == null)
        {
            Debug.LogWarning(
                "CanvasGroup Gentong belum ada. " +
                "Gentong akan bergeser tanpa fade."
            );

            gentongRectTransform.anchoredPosition =
                gentongShiftedPosition;

            yield break;
        }

        float duration = Mathf.Max(0.01f, gentongFadeDuration);
        float elapsed = 0f;

        // Fade keluar.
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            gentongCanvasGroup.alpha =
                Mathf.Lerp(1f, 0f, elapsed / duration);

            yield return null;
        }

        gentongCanvasGroup.alpha = 0f;

        // Pindahkan Gentong agar tidak menutupi Bakul.
        gentongRectTransform.anchoredPosition =
            gentongShiftedPosition;

        // Fade masuk.
        elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            gentongCanvasGroup.alpha =
                Mathf.Lerp(0f, 1f, elapsed / duration);

            yield return null;
        }

        gentongCanvasGroup.alpha = 1f;

        Debug.Log(
            "Gentong berpindah dari " + originalPosition +
            " ke " + gentongShiftedPosition
        );
    }

    private void FinishGentongEvent()
    {
        Debug.Log(currentEvent + " selesai.");
        FinishCurrentEvent();
    }

    // =====================================================
    // BAKUL TERHALANG
    // =====================================================

    public void HandleBakulClick()
    {
        if (IsInteractionBlocked())
            return;

        if (bakulUnlocked)
        {
            ClickBakul();
        }
        else
        {
            ClickBlockedBakul();
        }
    }

    public void ClickBlockedBakul()
    {
        if (IsInteractionBlocked())
            return;

        eventRunning = true;
        currentEvent = "BakulTerhalang";

        arrowBack.SetActive(false);
        HideDialogue();

        ShowDialogue(
            "Arka",
            "Benda ini terhalang sesuatu."
        );
    }

    private void FinishBlockedBakulEvent()
    {
        Debug.Log("Event Bakul terhalang selesai.");
        FinishCurrentEvent();
    }

    // =====================================================
    // BAKUL ZOOM DAN INVENTORY
    // =====================================================

    public void ClickBakul()
    {
        if (IsInteractionBlocked())
            return;

        Debug.Log("Bakul diklik.");

        // Bakul di ruangan menghilang setelah diambil.
        bakul.SetActive(false);

        // Bakul masuk ke inventory.
        bakulInventory.SetActive(true);

        OpenBakulZoom();
    }

    public void ClickBakulInventory()
    {
        if (IsInteractionBlocked())
            return;

        Debug.Log("Bakul di inventory diklik.");
        OpenBakulZoom();
    }

    private void OpenBakulZoom()
    {
        bakulZoom.SetActive(true);

        if (buttonInvestigasiArtefak != null)
            buttonInvestigasiArtefak.SetActive(true);

        if (arrowBack != null)
            arrowBack.SetActive(true);
    }

    public void CloseBakulZoom()
    {
        if (eventRunning)
            return;

        bakulZoom.SetActive(false);

        if (buttonInvestigasiArtefak != null)
            buttonInvestigasiArtefak.SetActive(false);

        if (arrowBack != null)
            arrowBack.SetActive(true);
    }

    // =====================================================
    // ARROW BACK
    // =====================================================

    public void ClickArrowBack()
    {
        if (eventRunning)
            return;

        if (bakulZoom.activeSelf)
        {
            CloseBakulZoom();
            return;
        }

        if (catatanZoom.activeSelf)
        {
            CloseCatatanZoom();
        }
    }
}