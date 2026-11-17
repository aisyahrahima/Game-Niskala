
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArtifactReturnDropZone : MonoBehaviour, IDropHandler
{
    [Header("Identitas Artefak")]
    [SerializeField] private string acceptedArtifactID;

    [Header("Objek yang Diaktifkan")]
    [SerializeField] private GameObject resultObject;

    [Header("Objek yang Dinonaktifkan")]
    [SerializeField] private GameObject targetToDeactivate;

    [Header("Pengaturan Tambahan")]
    [SerializeField] private bool disableTargetButtonDuringChase;

    private Button targetButton;
    private bool visualStateInitialized;
    private bool lastReturnedState;

    private void Awake()
    {
        targetButton = GetComponent<Button>();
    }

    private void Update()
    {
        if (!disableTargetButtonDuringChase || targetButton == null)
            return;

        targetButton.interactable = !NiskalaProgress.chaseStarted;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!NiskalaProgress.chaseStarted)
            return;

        ArtifactReturnDragItem draggedItem =
            eventData.pointerDrag != null
            ? eventData.pointerDrag.GetComponent<ArtifactReturnDragItem>()
            : null;

        if (draggedItem == null)
            return;

        if (draggedItem.ArtifactID != acceptedArtifactID)
            return;

        if (draggedItem.IsAlreadyReturned())
            return;

        // Aktifkan objek pengganti.
        if (resultObject != null)
            resultObject.SetActive(true);

        // Tandai artefak sudah dikembalikan.
        draggedItem.CompleteReturn();

        // Nonaktifkan objek target jika diperlukan.
        if (targetToDeactivate != null)
            targetToDeactivate.SetActive(false);

        Debug.Log(
            "Pengembalian artefak berhasil: " + acceptedArtifactID
        );

        CheckAllArtifactsReturned();
    }

    private void CheckAllArtifactsReturned()
    {
        bool allReturned =
            NiskalaProgress.tepakSirihReturned &&
            NiskalaProgress.bakulReturned &&
            NiskalaProgress.catatanCikSimaReturned;

        if (!allReturned)
            return;

        NiskalaProgress.chaseStarted = false;

        if (MayangChaseManager.Instance != null)
            MayangChaseManager.Instance.StopChase();

        Debug.Log("Semua artefak berhasil dikembalikan.");
    }

    private void LateUpdate()
    {
        bool isReturned = IsArtifactReturned();

        // Hindari mengatur ulang objek setiap frame
        // jika status pengembalian tidak berubah.
        if (visualStateInitialized &&
            lastReturnedState == isReturned)
        {
            return;
        }

        visualStateInitialized = true;
        lastReturnedState = isReturned;

        // Pulihkan tampilan objek hasil pengembalian.
        if (resultObject != null)
            resultObject.SetActive(isReturned);

        // Pulihkan tampilan objek target.
        if (targetToDeactivate != null)
            targetToDeactivate.SetActive(!isReturned);
    }

    private bool IsArtifactReturned()
    {
        switch (acceptedArtifactID)
        {
            case "TepakSirih":
                return NiskalaProgress.tepakSirihReturned;

            case "Bakul":
                return NiskalaProgress.bakulReturned;

            case "CatatanCikSima":
                return NiskalaProgress.catatanCikSimaReturned;

            default:
                return false;
        }
    }
}
