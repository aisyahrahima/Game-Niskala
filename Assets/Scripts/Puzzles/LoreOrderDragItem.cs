using UnityEngine;
using UnityEngine.EventSystems;

public class LoreOrderDragItem : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("Identitas Teks")]
    [SerializeField] private string itemID;

    [Header("Puzzle Controller")]
    [SerializeField] private LoreOrderPuzzleController puzzleController;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas rootCanvas;

    // Posisi awal saat scene pertama kali dimulai.
    private Transform initialParent;
    private Vector2 initialPosition;
    private int initialSiblingIndex;

    // Posisi sebelum drag terakhir dimulai.
    private Transform dragStartParent;
    private Vector2 dragStartPosition;
    private int dragStartSiblingIndex;

    private LoreOrderDropZone currentDropZone;

    private bool canDrag;
    private bool isDragging;
    private bool wasDropped;

    public string ItemID => itemID;
    public bool IsPlaced => currentDropZone != null;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        rootCanvas = GetComponentInParent<Canvas>();

        if (rootCanvas != null)
            rootCanvas = rootCanvas.rootCanvas;

        initialParent = transform.parent;
        initialPosition = rectTransform.anchoredPosition;
        initialSiblingIndex = transform.GetSiblingIndex();
    }

    public void SetCanDrag(bool state)
    {
        canDrag = state;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag || puzzleController == null ||
            !puzzleController.CanInteract() ||
            rootCanvas == null)
        {
            return;
        }

        isDragging = true;
        wasDropped = false;

        dragStartParent = transform.parent;
        dragStartPosition = rectTransform.anchoredPosition;
        dragStartSiblingIndex = transform.GetSiblingIndex();

        // Pindahkan sementara ke Canvas utama.
        transform.SetParent(rootCanvas.transform, true);
        transform.SetAsLastSibling();

        // Agar Drop Zone tetap menerima raycast.
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || rootCanvas == null)
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

        // Jika tidak berhasil masuk Drop Zone,
        // kembalikan ke posisi sebelum drag.
        if (!wasDropped)
        {
            transform.SetParent(dragStartParent, false);

            rectTransform.anchoredPosition =
                dragStartPosition;

            transform.SetSiblingIndex(
                dragStartSiblingIndex
            );
        }
    }

    public bool PlaceAt(LoreOrderDropZone targetZone)
    {
        if (targetZone == null)
            return false;

        // Jangan menimpa teks yang sudah berada di slot lain.
        if (targetZone.CurrentItem != null &&
            targetZone.CurrentItem != this)
        {
            return false;
        }

        // Kosongkan Drop Zone sebelumnya jika teks berpindah.
        if (currentDropZone != null &&
            currentDropZone != targetZone)
        {
            currentDropZone.SetCurrentItem(null);
        }

        currentDropZone = targetZone;

        targetZone.SetCurrentItem(this);

        // Jadikan teks sebagai child Drop Zone.
        transform.SetParent(targetZone.transform, false);

        rectTransform.anchoredPosition = Vector2.zero;

        wasDropped = true;
        canvasGroup.blocksRaycasts = true;

        Debug.Log(
            itemID + " ditempatkan pada " + targetZone.name
        );

        return true;
    }

    public void ResetToOriginalPosition()
    {
        if (currentDropZone != null)
        {
            currentDropZone.SetCurrentItem(null);
            currentDropZone = null;
        }

        transform.SetParent(initialParent, false);

        rectTransform.anchoredPosition = initialPosition;

        transform.SetSiblingIndex(initialSiblingIndex);

        isDragging = false;
        wasDropped = false;
        canvasGroup.blocksRaycasts = true;
    }
}