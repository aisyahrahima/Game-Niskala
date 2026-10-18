using UnityEngine;
using UnityEngine.EventSystems;

public class CursorHover : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CursorManager.Instance != null)
            CursorManager.Instance.SetGrab();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // CursorManager akan menentukan cursor yang tepat
        // berdasarkan objek di bawah pointer pada Update.
    }
}