
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [Header("Cursor Images")]
    [SerializeField] private Texture2D pointerCursor;
    [SerializeField] private Texture2D grabCursor;

    [Header("Cursor Scale")]
    [SerializeField, Range(0.1f, 1f)]
    private float pointerScale = 0.5f;

    [SerializeField, Range(0.1f, 1f)]
    private float grabScale = 0.5f;

    [Header("Cursor Hotspots")]
    [SerializeField] private Vector2 pointerHotspot = Vector2.zero;
    [SerializeField] private Vector2 grabHotspot = Vector2.zero;

    private Texture2D scaledPointer;
    private Texture2D scaledGrab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        scaledPointer = CreateScaledCursor(pointerCursor, pointerScale);
        scaledGrab = CreateScaledCursor(grabCursor, grabScale);

        Debug.Log("Pointer hasil resize: " +
            (scaledPointer != null
                ? scaledPointer.width + " x " + scaledPointer.height
                : "null"));

        Debug.Log("Grab hasil resize: " +
            (scaledGrab != null
                ? scaledGrab.width + " x " + scaledGrab.height
                : "null"));

        SetPointer();
    }

    private Texture2D CreateScaledCursor(Texture2D source, float scale)
    {
        if (source == null)
        {
            Debug.LogError("Gambar kursor belum dihubungkan.");
            return null;
        }

        int width = Mathf.Max(1, Mathf.RoundToInt(source.width * scale));
        int height = Mathf.Max(1, Mathf.RoundToInt(source.height * scale));

        RenderTexture renderTexture = RenderTexture.GetTemporary(
            width,
            height,
            0,
            RenderTextureFormat.ARGB32
        );

        RenderTexture previous = RenderTexture.active;

        Graphics.Blit(source, renderTexture);
        RenderTexture.active = renderTexture;

        Texture2D result = new Texture2D(
            width,
            height,
            TextureFormat.RGBA32,
            false
        );

        result.ReadPixels(
            new Rect(0, 0, width, height),
            0,
            0
        );

        result.Apply();
        result.filterMode = FilterMode.Bilinear;
        result.wrapMode = TextureWrapMode.Clamp;

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTexture);

        result.name = source.name + "_Scaled";
        return result;
    }

    public void SetPointer()
    {
        if (scaledPointer == null) return;

        Vector2 hotspot = pointerHotspot * pointerScale;

        Cursor.SetCursor(
            scaledPointer,
            hotspot,
            CursorMode.Auto
        );
    }

    public void SetGrab()
    {
        if (scaledGrab == null) return;

        Vector2 hotspot = grabHotspot * grabScale;

        Cursor.SetCursor(
            scaledGrab,
            hotspot,
            CursorMode.Auto
        );
    }

    private void OnDestroy()
    {
        if (Instance != this) return;

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        if (scaledPointer != null)
            Destroy(scaledPointer);

        if (scaledGrab != null)
            Destroy(scaledGrab);

        Instance = null;
    }
}