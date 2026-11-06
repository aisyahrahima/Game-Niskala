using UnityEngine;
using UnityEngine.EventSystems;

public class ChapterHoverController : MonoBehaviour
{
    [Header("Hover Selected Objects")]
    [SerializeField] private GameObject hoverSelected1;
    [SerializeField] private GameObject hoverSelected2;
    [SerializeField] private GameObject hoverSelected3;
    [SerializeField] private GameObject hoverSelected4;

    private void Awake()
    {
        HideAllHoverSelected();
    }

    public void ShowHoverSelected(int chapterNumber)
    {
        HideAllHoverSelected();

        switch (chapterNumber)
        {
            case 1:
                hoverSelected1.SetActive(true);
                break;

            case 2:
                hoverSelected2.SetActive(true);
                break;

            case 3:
                hoverSelected3.SetActive(true);
                break;

            case 4:
                hoverSelected4.SetActive(true);
                break;
        }
    }

    public void HideAllHoverSelected()
    {
        if (hoverSelected1 != null)
            hoverSelected1.SetActive(false);

        if (hoverSelected2 != null)
            hoverSelected2.SetActive(false);

        if (hoverSelected3 != null)
            hoverSelected3.SetActive(false);

        if (hoverSelected4 != null)
            hoverSelected4.SetActive(false);
    }
}