using UnityEngine;
using UnityEngine.EventSystems;

public class LoreOrderDropZone : MonoBehaviour, IDropHandler
{
    [Header("Identitas Slot")]
    [SerializeField] private string expectedItemID;

    [Header("Puzzle Controller")]
    [SerializeField] private LoreOrderPuzzleController puzzleController;

    private LoreOrderDragItem currentItem;

    public string ExpectedItemID => expectedItemID;
    public LoreOrderDragItem CurrentItem => currentItem;

    public void SetCurrentItem(LoreOrderDragItem item)
    {
        currentItem = item;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (puzzleController == null)
            return;

        GameObject draggedObject = eventData.pointerDrag;

        if (draggedObject == null)
            return;

        LoreOrderDragItem dragItem =
            draggedObject.GetComponent<LoreOrderDragItem>();

        if (dragItem == null)
            return;

        puzzleController.TryPlaceItem(dragItem, this);
    }
}