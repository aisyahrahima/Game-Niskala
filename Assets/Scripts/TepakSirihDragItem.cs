
using UnityEngine;
using UnityEngine.EventSystems;

public class TepakSirihDragItem : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("Identitas Objek")]
    [SerializeField] private string itemID;

    [Header("Puzzle Controller")]
    [SerializeField] private TepakSirihPuzzle puzzleController;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas rootCanvas;

    private Transform originalParent;
    private Vector2 originalPosition;
    private int originalSiblingIndex;

    private bool isPlaced;
    private bool isDragging;
    private bool canDrag;

    public string ItemID => itemID;
    public bool IsPlaced => isPlaced;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        rootCanvas = GetComponentInParent<Canvas>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void SetCanDrag(bool state)
    {
        canDrag = state;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag || isPlaced)
            return;

        isDragging = true;

        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;
        originalSiblingIndex = transform.GetSiblingIndex();

        // Tampilkan sementara di urutan paling depan
        // selama objek sedang di-drag.
        transform.SetAsLastSibling();

        // Agar DropZone tetap dapat menerima raycast.
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || isPlaced || rootCanvas == null)
            return;

        rectTransform.anchoredPosition +=
            eventData.delta / rootCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        isDragging = false;

        canvasGroup.blocksRaycasts = true;

        // Periksa apakah objek bertumpang tindih
        // dengan salah satu DropZone.
        if (!isPlaced && puzzleController != null)
        {
            puzzleController.TryPlaceByOverlap(
                this,
                rectTransform
            );
        }

        // Jika masih belum berhasil ditempatkan,
        // kembalikan ke posisi awal.
        if (!isPlaced)
        {
            ReturnToOriginalPosition();
        }
    }

    public void PlaceAt(RectTransform target)
    {
        if (target == null)
            return;

        // Posisikan objek tepat pada target.
        rectTransform.position = target.position;

        // Kembalikan urutan objek ke urutan awal Hierarchy.
        if (originalParent != null)
        {
            transform.SetSiblingIndex(
                Mathf.Clamp(
                    originalSiblingIndex,
                    0,
                    originalParent.childCount - 1
                )
            );
        }

        isPlaced = true;
        canvasGroup.blocksRaycasts = true;

        Debug.Log(itemID + " berhasil ditempatkan.");
    }

    private void ReturnToOriginalPosition()
    {
        if (originalParent != null)
        {
            transform.SetParent(originalParent);

            rectTransform.anchoredPosition =
                originalPosition;

            transform.SetSiblingIndex(
                originalSiblingIndex
            );
        }

        Debug.Log(itemID + " dikembalikan ke posisi awal.");
    }
}
