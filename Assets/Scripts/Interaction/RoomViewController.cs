using UnityEngine;

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

    private bool isRightView = false;

    public void MoveToRightView()
    {
        if (isRightView)
            return;

        SetRightView();
        isRightView = true;
    }

    public void MoveToLeftView()
    {
        if (!isRightView)
            return;

        SetLeftView();
        isRightView = false;
    }

    private void SetLeftView()
    {
        roomContent.offsetMin = new Vector2(
            startLeft,
            bottom
        );

        roomContent.offsetMax = new Vector2(
            -startRight,
            -top
        );
    }

    private void SetRightView()
    {
        roomContent.offsetMin = new Vector2(
            rightViewLeft,
            bottom
        );

        roomContent.offsetMax = new Vector2(
            -rightViewRight,
            -top
        );
    }

    private void Start()
    {
        SetLeftView();
        isRightView = false;
    }
}