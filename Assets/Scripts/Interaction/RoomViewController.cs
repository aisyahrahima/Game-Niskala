using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomViewController : MonoBehaviour
{
    [Header("Room Content")]
    [SerializeField] private RectTransform roomContent;

    [Header("View Positions")]
    [SerializeField] private float startLeft = -909.906f;
    [SerializeField] private float startRight = -909.906f;

    [SerializeField] private float rightViewLeft = -1114.906f;
    [SerializeField] private float rightViewRight = -704.906f;

    [Header("Common Values")]
    [SerializeField] private float top = -490.043f;
    [SerializeField] private float bottom = -490.043f;

    [Header("Smooth Transition")]
    [SerializeField] private float transitionDuration = 0.5f;

    private bool isRightView;
    private Coroutine moveCoroutine;

    private void Start()
    {
        SetLeftViewImmediate();
    }

    public void MoveToRightView()
    {
        if (isRightView)
            return;

        isRightView = true;
        StartMove(SetRightPosition());
    }

    public void MoveToLeftView()
    {
        if (!isRightView)
            return;

        isRightView = false;
        StartMove(SetLeftPosition());
    }

    private Vector2 GetCurrentOffsetMin()
    {
        return roomContent.offsetMin;
    }

    private Vector2 GetCurrentOffsetMax()
    {
        return roomContent.offsetMax;
    }

    private Vector2 SetLeftPosition()
    {
        return new Vector2(startLeft, bottom);
    }

    private Vector2 SetRightPosition()
    {
        return new Vector2(rightViewLeft, bottom);
    }

    private void StartMove(Vector2 targetOffsetMin)
    {
        if (roomContent == null)
        {
            Debug.LogError("RoomContent belum dihubungkan.");
            return;
        }

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        Vector2 targetOffsetMax = isRightView
            ? new Vector2(-rightViewRight, -top)
            : new Vector2(-startRight, -top);

        moveCoroutine = StartCoroutine(
            SmoothMove(targetOffsetMin, targetOffsetMax)
        );
    }

    private IEnumerator SmoothMove(
        Vector2 targetOffsetMin,
        Vector2 targetOffsetMax)
    {
        Vector2 initialOffsetMin = roomContent.offsetMin;
        Vector2 initialOffsetMax = roomContent.offsetMax;

        float duration = Mathf.Max(0.01f, transitionDuration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            // SmoothStep membuat awal dan akhir gerakan lebih halus.
            t = Mathf.SmoothStep(0f, 1f, t);

            roomContent.offsetMin = Vector2.Lerp(
                initialOffsetMin,
                targetOffsetMin,
                t
            );

            roomContent.offsetMax = Vector2.Lerp(
                initialOffsetMax,
                targetOffsetMax,
                t
            );

            yield return null;
        }

        roomContent.offsetMin = targetOffsetMin;
        roomContent.offsetMax = targetOffsetMax;

        moveCoroutine = null;
    }

    private void SetLeftViewImmediate()
    {
        if (roomContent == null)
        {
            Debug.LogError("RoomContent belum dihubungkan.");
            return;
        }

        roomContent.offsetMin = SetLeftPosition();
        roomContent.offsetMax = new Vector2(-startRight, -top);

        isRightView = false;
    }
}