using UnityEngine;
using UnityEngine.EventSystems;

public class ViewRightHover : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private RoomViewController roomViewController;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (roomViewController != null)
            roomViewController.MoveToRightView();
    }
}