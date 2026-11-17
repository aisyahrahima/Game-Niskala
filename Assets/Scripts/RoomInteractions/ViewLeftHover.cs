using UnityEngine;
using UnityEngine.EventSystems;

public class ViewLeftHover : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private RoomViewController roomViewController;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (roomViewController == null)
            return;

        if (!roomViewController.CanChangeView())
            return;

        roomViewController.MoveToLeftView();
    }
}