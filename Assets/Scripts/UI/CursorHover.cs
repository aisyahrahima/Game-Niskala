
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorHover : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    private bool isHovering;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;

        if (CursorManager.Instance != null)
            CursorManager.Instance.SetGrab();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;

        if (CursorManager.Instance != null)
            CursorManager.Instance.SetPointer();
    }

    private void OnDisable()
    {
        if (isHovering && CursorManager.Instance != null)
            CursorManager.Instance.SetPointer();

        isHovering = false;
    }
}