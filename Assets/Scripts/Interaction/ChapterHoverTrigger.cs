using UnityEngine;
using UnityEngine.EventSystems;

public class ChapterHoverTrigger : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private ChapterHoverController hoverController;
    [SerializeField] private int chapterNumber;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverController != null)
            hoverController.ShowHoverSelected(chapterNumber);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverController != null)
            hoverController.HideAllHoverSelected();
    }
}