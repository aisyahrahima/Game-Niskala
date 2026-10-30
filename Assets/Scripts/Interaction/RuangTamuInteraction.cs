
using System.Collections;
using UnityEngine;

public class RuangTamuInteraction : MonoBehaviour
{
    [Header("Dialog")]
    [SerializeField] private DialogueSystem dialogueSystem;

    [Header("Arrow Back")]
    [SerializeField] private GameObject arrowBack;

    [Header("Lukisan")]
    [SerializeField] private GameObject lukisan;
    [SerializeField] private GameObject lukisanZoom;

    [Header("Perhiasan")]
    [SerializeField] private GameObject perhiasan;
    [SerializeField] private GameObject perhiasanZoom;
    [SerializeField] private GameObject jumpscare;

    [Header("Sisir")]
    [SerializeField] private GameObject sisir;
    [SerializeField] private GameObject sisirZoom;
    [SerializeField] private GameObject sisirRambutZoom;

    [Header("Senjata")]
    [SerializeField] private GameObject senjata;
    [SerializeField] private GameObject senjataZoom;

    [Header("Efek Getaran")]
    [SerializeField] private RectTransform gameplay;
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeMagnitude = 8f;

    private bool interactionBusy;
    private bool zoomOpen;

    private bool lukisanClickedOnce;
    private bool perhiasanClickedOnce;
    private bool sisirClickedOnce;
    private bool senjataClickedOnce;

    private Vector2 originalGameplayPosition;

    private void Awake()
    {
        SetActiveSafe(lukisanZoom, false);
        SetActiveSafe(perhiasanZoom, false);
        SetActiveSafe(sisirZoom, false);
        SetActiveSafe(sisirRambutZoom, false);
        SetActiveSafe(senjataZoom, false);
        SetActiveSafe(jumpscare, false);

        SetArrowBack(false);

        if (gameplay != null)
            originalGameplayPosition = gameplay.anchoredPosition;
    }

    private void Start()
    {
        StartCoroutine(OpeningSequence());
    }

    // =========================================
    // DIALOG PEMBUKA RUANG TAMU
    // =========================================

    private IEnumerator OpeningSequence()
    {
        interactionBusy = true;
        BeginInteraction();

        // Jeda 2 detik sebelum dialog pembuka.
        yield return new WaitForSecondsRealtime(2f);

        Debug.Log("SFX: Suara pintu terkunci.");

        if (dialogueSystem != null)
        {
            bool started = dialogueSystem.StartDialogue(
                "Arka",
                "Hah?",
                "Kenapa tiba-tiba pintunya langsung terkunci?",
                "....",
                "Yah... mau nggak mau aku harus tetap di sini untuk sementara.",
                "Sekalian saja aku cari tahu apa yang sebenarnya terjadi di tempat ini."
            );

            if (started)
                yield return WaitForDialogue();
        }
        else
        {
            Debug.LogError("DialogueSystem belum dihubungkan.");
        }

        interactionBusy = false;
        EndInteraction();

        Debug.Log("Dialog pembuka RuangTamu selesai.");
    }

    // =========================================
    // LUKISAN
    // =========================================

    public void ClickLukisan()
    {
        if (IsInteractionBlocked())
            return;

        bool firstClick = !lukisanClickedOnce;

        OpenZoom(lukisan, lukisanZoom);

        if (firstClick)
        {
            lukisanClickedOnce = true;
            interactionBusy = true;
            BeginInteraction();

            Debug.Log("SFX: Suara perempuan tertawa.");

            StartCoroutine(PlayLukisanDialogue());
        }
        else
        {
            SetArrowBack(true);
        }
    }

    private IEnumerator PlayLukisanDialogue()
    {
        // Memberikan waktu untuk event suara tertawa.
        yield return new WaitForSecondsRealtime(2f);

        yield return StartDialogue(
            "Darimana suara itu berasal?"
        );

        interactionBusy = false;
        EndInteraction();

        SetArrowBack(true);
    }

    // =========================================
    // PERHIASAN
    // =========================================

    public void ClickPerhiasan()
    {
        if (IsInteractionBlocked())
            return;

        bool firstClick = !perhiasanClickedOnce;

        OpenZoom(perhiasan, perhiasanZoom);

        if (firstClick)
        {
            perhiasanClickedOnce = true;
            interactionBusy = true;
            BeginInteraction();

            StartCoroutine(PlayPerhiasanEvent());
        }
        else
        {
            SetArrowBack(true);
        }
    }

    private IEnumerator PlayPerhiasanEvent()
    {
        Debug.Log("SFX: Efek suara jumpscare Perhiasan.");
        Debug.Log("Efek getaran layar dimulai.");
        Debug.Log("Jumpscare: Siluet perempuan muncul.");

        yield return StartCoroutine(ShakeScreen());

        if (jumpscare != null)
        {
            jumpscare.SetActive(true);
            yield return new WaitForSecondsRealtime(0.7f);
            jumpscare.SetActive(false);
        }

        Debug.Log("Jumpscare Perhiasan selesai.");

        // Jeda setelah event sebelum dialog.
        yield return new WaitForSecondsRealtime(2f);

        yield return StartDialogue("Apa itu...?");

        interactionBusy = false;
        EndInteraction();

        SetArrowBack(true);
    }

    private IEnumerator ShakeScreen()
    {
        if (gameplay == null)
            yield break;

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(
                -shakeMagnitude,
                shakeMagnitude
            );

            float y = Random.Range(
                -shakeMagnitude,
                shakeMagnitude
            );

            gameplay.anchoredPosition =
                originalGameplayPosition + new Vector2(x, y);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        gameplay.anchoredPosition = originalGameplayPosition;
    }

    // =========================================
    // SISIR
    // =========================================

    public void ClickSisir()
    {
        if (IsInteractionBlocked())
            return;

        bool firstClick = !sisirClickedOnce;

        OpenZoom(sisir, sisirZoom);

        if (firstClick)
        {
            sisirClickedOnce = true;
            interactionBusy = true;
            BeginInteraction();

            StartCoroutine(PlaySisirEvent());
        }
        else
        {
            SetArrowBack(true);
        }
    }

    private IEnumerator PlaySisirEvent()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        if (sisirZoom != null)
            sisirZoom.SetActive(false);

        if (sisirRambutZoom != null)
            sisirRambutZoom.SetActive(true);

        Debug.Log("Tampilan Sisir_Rambut_Zoom muncul.");

        yield return new WaitForSecondsRealtime(1f);

        if (sisirRambutZoom != null)
            sisirRambutZoom.SetActive(false);

        if (sisirZoom != null)
            sisirZoom.SetActive(true);

        Debug.Log("Tampilan Sisir_Zoom kembali.");

        // Jeda setelah pergantian tampilan sebelum dialog.
        yield return new WaitForSecondsRealtime(2f);

        yield return StartDialogue(
            "Kenapa tiba-tiba ada rambut?"
        );

        interactionBusy = false;
        EndInteraction();

        SetArrowBack(true);
    }

    // =========================================
    // SENJATA
    // =========================================

    public void ClickSenjata()
    {
        if (IsInteractionBlocked())
            return;

        bool firstClick = !senjataClickedOnce;

        OpenZoom(senjata, senjataZoom);

        if (firstClick)
        {
            senjataClickedOnce = true;

            Debug.Log("SFX: Suara langkah kaki prajurit.");
        }

        // Senjata tidak memiliki event berdurasi atau dialog.
        SetArrowBack(true);
    }

    // =========================================
    // ZOOM DAN ARROW BACK
    // =========================================

    private void OpenZoom(
        GameObject originalObject,
        GameObject zoomObject)
    {
        if (originalObject == null || zoomObject == null)
        {
            Debug.LogError(
                "Objek asli atau objek zoom belum dihubungkan."
            );
            return;
        }

        originalObject.SetActive(false);
        zoomObject.SetActive(true);

        zoomOpen = true;

        // ArrowBack baru ditampilkan setelah event selesai.
        SetArrowBack(false);

        Debug.Log("Zoom dibuka: " + zoomObject.name);
    }

    public void ClickArrowBack()
    {
        if (interactionBusy)
            return;

        if (!zoomOpen)
            return;

        CloseAllZoom();

        // Tidak memanggil EndInteraction di sini karena
        // zoom kedua dan Senjata tidak memulai interaction lock.
        Debug.Log("Zoom RuangTamu ditutup.");
    }

    private void CloseAllZoom()
    {
        if (lukisanZoom != null && lukisanZoom.activeSelf)
        {
            lukisanZoom.SetActive(false);
            SetActiveSafe(lukisan, true);
        }

        if (perhiasanZoom != null && perhiasanZoom.activeSelf)
        {
            perhiasanZoom.SetActive(false);
            SetActiveSafe(perhiasan, true);
        }

        if (sisirZoom != null && sisirZoom.activeSelf)
        {
            sisirZoom.SetActive(false);
            SetActiveSafe(sisir, true);
        }

        if (sisirRambutZoom != null)
            sisirRambutZoom.SetActive(false);

        if (senjataZoom != null && senjataZoom.activeSelf)
        {
            senjataZoom.SetActive(false);
            SetActiveSafe(senjata, true);
        }

        zoomOpen = false;
        SetArrowBack(false);
    }

    // =========================================
    // DIALOG
    // =========================================

    private IEnumerator StartDialogue(string line)
    {
        if (dialogueSystem == null)
        {
            Debug.LogError("DialogueSystem belum dihubungkan.");
            yield break;
        }

        bool started = dialogueSystem.StartDialogue(
            "Arka",
            line
        );

        if (started)
            yield return WaitForDialogue();
    }

    private IEnumerator WaitForDialogue()
    {
        while (dialogueSystem != null &&
               dialogueSystem.IsDialogueActive)
        {
            yield return null;
        }
    }

    // =========================================
    // INTERACTION LOCK
    // =========================================

    private void BeginInteraction()
    {
        GameEvents.RaiseInteractionStarted();
    }

    private void EndInteraction()
    {
        GameEvents.RaiseInteractionFinished();
    }

    public bool IsInteractionBlocked()
    {
        if (interactionBusy || zoomOpen)
            return true;

        if (dialogueSystem != null &&
            dialogueSystem.IsDialogueActive)
            return true;

        return false;
    }

    // =========================================
    // BANTUAN
    // =========================================

    private void SetArrowBack(bool visible)
    {
        if (arrowBack != null)
            arrowBack.SetActive(visible);
    }

    private void SetActiveSafe(GameObject target, bool state)
    {
        if (target != null)
            target.SetActive(state);
    }
}
