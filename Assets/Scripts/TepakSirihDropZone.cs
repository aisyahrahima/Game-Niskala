
using UnityEngine;
using UnityEngine.EventSystems;

public class TepakSirihDropZone : MonoBehaviour, IDropHandler
{
    [Header("Identitas Target")]
    [SerializeField] private string acceptedItemID;

    [Header("Pengaturan Area Drop")]
    [SerializeField, Range(0f, 0.5f)]
    private float overlapPadding = 0.15f;

    [Header("Puzzle Controller")]
    [SerializeField] private TepakSirihPuzzle puzzleController;

    private RectTransform rectTransform;

    public string AcceptedItemID => acceptedItemID;
    public RectTransform TargetRect => rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public float GetOverlapArea(RectTransform draggedRect)
    {
        if (draggedRect == null || rectTransform == null)
            return 0f;

        Rect itemBounds = GetWorldRect(draggedRect);
        Rect zoneBounds = GetWorldRect(rectTransform);

        // Perluas area DropZone agar lebih mudah menerima objek.
        float extraWidth = zoneBounds.width * overlapPadding;
        float extraHeight = zoneBounds.height * overlapPadding;

        zoneBounds.xMin -= extraWidth;
        zoneBounds.xMax += extraWidth;

        zoneBounds.yMin -= extraHeight;
        zoneBounds.yMax += extraHeight;

        float overlapWidth =
            Mathf.Min(itemBounds.xMax, zoneBounds.xMax) -
            Mathf.Max(itemBounds.xMin, zoneBounds.xMin);

        float overlapHeight =
            Mathf.Min(itemBounds.yMax, zoneBounds.yMax) -
            Mathf.Max(itemBounds.yMin, zoneBounds.yMin);

        if (overlapWidth <= 0 || overlapHeight <= 0)
            return 0f;

        return overlapWidth * overlapHeight;
    }

    private Rect GetWorldRect(RectTransform target)
    {
        Vector3[] corners = new Vector3[4];

        target.GetWorldCorners(corners);

        float minX = corners[0].x;
        float maxX = corners[0].x;
        float minY = corners[0].y;
        float maxY = corners[0].y;

        for (int i = 1; i < 4; i++)
        {
            minX = Mathf.Min(minX, corners[i].x);
            maxX = Mathf.Max(maxX, corners[i].x);
            minY = Mathf.Min(minY, corners[i].y);
            maxY = Mathf.Max(maxY, corners[i].y);
        }

        return Rect.MinMaxRect(minX, minY, maxX, maxY);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (puzzleController == null)
            return;

        GameObject draggedObject = eventData.pointerDrag;

        if (draggedObject == null)
            return;

        TepakSirihDragItem dragItem =
            draggedObject.GetComponent<TepakSirihDragItem>();

        if (dragItem == null)
            return;

        puzzleController.TryPlaceItem(
            dragItem,
            this,
            rectTransform
        );
    }
}
