using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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

    private readonly List<RaycastResult> raycastResults =
        new List<RaycastResult>();

    private bool isGrabCursor;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        scaledPointer = CreateScaledCursor(
            pointerCursor,
            pointerScale
        );

        scaledGrab = CreateScaledCursor(
            grabCursor,
            grabScale
        );

        SetPointer();
    }

    private void Update()
    {
        bool overInteractable = IsPointerOverInteractable();

        // Grab ketika menyorot objek interaktif.
        // Pointer ketika tidak menyorot objek interaktif.
        if (overInteractable)
        {
            SetGrab();
        }
        else
        {
            SetPointer();
        }
    }

    private bool IsPointerOverInteractable()
    {
        EventSystem eventSystem = EventSystem.current;

        if (eventSystem == null)
            return false;

        if (Mouse.current == null)
            return false;

        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Mouse.current.position.ReadValue();

        // Bersihkan hasil raycast sebelumnya.
        raycastResults.Clear();

        // Periksa objek UI di bawah mouse.
        eventSystem.RaycastAll(pointerData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            GameObject hitObject = result.gameObject;

            if (hitObject == null)
                continue;

            // Periksa objek yang terkena raycast dan parent-nya.
            Transform current = hitObject.transform;

            while (current != null)
            {
                // Periksa Button, Toggle, Slider, Dropdown, dan Selectable lain.
                Selectable selectable =
                    current.GetComponent<Selectable>();

                if (selectable != null &&
                    selectable.IsInteractable() &&
                    selectable.gameObject.activeInHierarchy)
                {
                    return true;
                }

                // Periksa komponen yang menangani klik.
                MonoBehaviour[] components =
                    current.GetComponents<MonoBehaviour>();

                foreach (MonoBehaviour component in components)
                {
                    if (component == null ||
                        !component.isActiveAndEnabled)
                    {
                        continue;
                    }

                    if (component is IPointerClickHandler)
                        return true;
                }

                current = current.parent;
            }
        }

        return false;
    }

    private Texture2D CreateScaledCursor(
        Texture2D source,
        float scale)
    {
        if (source == null)
        {
            Debug.LogError("Gambar cursor belum dihubungkan.");
            return null;
        }

        int width = Mathf.Max(
            1,
            Mathf.RoundToInt(source.width * scale)
        );

        int height = Mathf.Max(
            1,
            Mathf.RoundToInt(source.height * scale)
        );

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
        if (scaledPointer == null || isGrabCursor == false)
        {
            if (scaledPointer == null)
                return;
        }

        if (isGrabCursor == false)
        {
            Cursor.SetCursor(
                scaledPointer,
                pointerHotspot * pointerScale,
                CursorMode.Auto
            );
        }

        isGrabCursor = false;
    }

    public void SetGrab()
    {
        if (scaledGrab == null)
            return;

        if (isGrabCursor)
            return;

        Cursor.SetCursor(
            scaledGrab,
            grabHotspot * grabScale,
            CursorMode.Auto
        );

        isGrabCursor = true;
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        Cursor.SetCursor(
            null,
            Vector2.zero,
            CursorMode.Auto
        );

        if (scaledPointer != null)
            Destroy(scaledPointer);

        if (scaledGrab != null)
            Destroy(scaledGrab);

        Instance = null;
    }
}