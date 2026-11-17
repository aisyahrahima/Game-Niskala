
using UnityEngine;
using UnityEngine.EventSystems;

public class ArtifactReturnDragItem : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("Identitas Artefak")]
    [SerializeField] private string artifactID;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas rootCanvas;

    private Transform originalParent;
    private Vector3 originalPosition;
    private int originalSiblingIndex;

    private bool isDragging;
    private bool returnedSuccessfully;

    public string ArtifactID => artifactID;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        rootCanvas = GetComponentInParent<Canvas>();

        if (rootCanvas != null)
            rootCanvas = rootCanvas.rootCanvas;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!NiskalaProgress.chaseStarted)
            return;

        if (IsAlreadyReturned())
            return;

        if (rootCanvas == null)
        {
            Debug.LogError("Root Canvas tidak ditemukan.");
            return;
        }

        isDragging = true;
        returnedSuccessfully = false;

        originalParent = transform.parent;
        originalPosition = rectTransform.position;
        originalSiblingIndex = transform.GetSiblingIndex();

        transform.SetParent(rootCanvas.transform, true);

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        isDragging = false;

        canvasGroup.blocksRaycasts = true;

        if (returnedSuccessfully)
        {
            gameObject.SetActive(false);
            return;
        }

        transform.SetParent(originalParent, false);

        rectTransform.position = originalPosition;

        transform.SetSiblingIndex(originalSiblingIndex);
    }

    public bool IsAlreadyReturned()
    {
        switch (artifactID)
        {
            case "TepakSirih":
                return NiskalaProgress.tepakSirihReturned;

            case "Bakul":
                return NiskalaProgress.bakulReturned;

            case "CatatanCikSima":
                return NiskalaProgress.catatanCikSimaReturned;
        }

        return false;
    }

    public void CompleteReturn()
    {
        returnedSuccessfully = true;

        switch (artifactID)
        {
            case "TepakSirih":
                NiskalaProgress.tepakSirihReturned = true;
                break;

            case "Bakul":
                NiskalaProgress.bakulReturned = true;
                break;

            case "CatatanCikSima":
                NiskalaProgress.catatanCikSimaReturned = true;
                break;
        }

        Debug.Log("Artefak berhasil dikembalikan: " + artifactID);
    }
}
