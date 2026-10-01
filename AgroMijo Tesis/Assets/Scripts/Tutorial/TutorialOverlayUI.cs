using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialOverlayUI : MonoBehaviour
{
    public static TutorialOverlayUI Instancia { get; private set; }

    [Header("Los 4 paneles oscuros")]
    public RectTransform panelArriba;
    public RectTransform panelAbajo;
    public RectTransform panelIzquierda;
    public RectTransform panelDerecha;

    [Header("Panel de mensaje")]
    public GameObject panelMensaje;
    public TMP_Text textoMensaje;
    public Button botonSiguiente;

    [Header("Ajuste")]
    [Tooltip("Espacio extra alrededor del elemento destacado")]
    public float padding = 8f;

    private RectTransform rootRT;

    private void Awake()
    {
        Instancia = this;

        rootRT = GetComponentInParent<Canvas>()
            .GetComponent<RectTransform>();

        Ocultar();
    }

    public void MostrarPaso(
        string mensaje,
        RectTransform objetivo,
        Transform objetivoMundo,
        bool mostrarBoton)
    {
        gameObject.SetActive(true);

        textoMensaje.text = mensaje;

        botonSiguiente.gameObject.SetActive(mostrarBoton);
        panelMensaje.SetActive(true);

        // Primero intenta usar un objetivo UI.
        if (objetivo != null)
        {
            SetSpotlight(objetivo);
        }
        // Si no hay objetivo UI, intenta usar uno 3D.
        else if (objetivoMundo != null)
        {
            SetSpotlightMundo(objetivoMundo);
        }
        // Si no hay ningún objetivo, oscurece toda la pantalla.
        else
        {
            SetFullOverlay();
        }
    }

    public void Ocultar()
    {
        SetPanel(panelArriba, 0, 0, 0, 0);
        SetPanel(panelAbajo, 0, 0, 0, 0);
        SetPanel(panelIzquierda, 0, 0, 0, 0);
        SetPanel(panelDerecha, 0, 0, 0, 0);

        panelMensaje.SetActive(false);
        gameObject.SetActive(false);
    }

    // =========================================================
    // OVERLAY COMPLETO
    // =========================================================

    private void SetFullOverlay()
    {
        Rect r = rootRT.rect;

        SetPanel(
            panelArriba,
            0,
            0,
            r.width,
            r.height
        );

        SetPanel(panelAbajo, 0, 0, 0, 0);
        SetPanel(panelIzquierda, 0, 0, 0, 0);
        SetPanel(panelDerecha, 0, 0, 0, 0);
    }

    // =========================================================
    // OBJETIVO UI
    // =========================================================

    private void SetSpotlight(RectTransform objetivo)
    {
        Vector3[] corners = new Vector3[4];

        objetivo.GetWorldCorners(corners);

        Rect canvas = rootRT.rect;

        Vector2 bl = rootRT.InverseTransformPoint(corners[0]);
        Vector2 tr = rootRT.InverseTransformPoint(corners[2]);

        float minX = bl.x - canvas.xMin - padding;
        float minY = bl.y - canvas.yMin - padding;

        float maxX = tr.x - canvas.xMin + padding;
        float maxY = tr.y - canvas.yMin + padding;

        float w = canvas.width;
        float h = canvas.height;

        SetSpotlightDesdeRect(
            minX,
            minY,
            maxX,
            maxY,
            w,
            h
        );
    }

    // =========================================================
    // OBJETIVO 3D
    // =========================================================

    private void SetSpotlightMundo(Transform objetivo)
    {
        Renderer rendererObjetivo =
            objetivo.GetComponent<Renderer>();

        if (rendererObjetivo == null)
        {
            Debug.LogWarning(
                "TutorialOverlayUI: El objetivo 3D '" +
                objetivo.name +
                "' no tiene Renderer."
            );

            SetFullOverlay();
            return;
        }

        Bounds bounds = rendererObjetivo.bounds;

        Vector3[] corners =
        {
            new Vector3(bounds.min.x, bounds.min.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),

            new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.max.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.max.y, bounds.max.z)
        };

        Camera cam = Camera.main;

        if (cam == null)
        {
            Debug.LogWarning(
                "TutorialOverlayUI: No se encontró Camera.main."
            );

            SetFullOverlay();
            return;
        }

        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        foreach (Vector3 corner in corners)
        {
            Vector3 screenPoint =
                cam.WorldToScreenPoint(corner);

            Vector2 localPoint;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rootRT,
                screenPoint,
                null,
                out localPoint
            );

            minX = Mathf.Min(minX, localPoint.x);
            minY = Mathf.Min(minY, localPoint.y);
            maxX = Mathf.Max(maxX, localPoint.x);
            maxY = Mathf.Max(maxY, localPoint.y);
        }

        Rect canvas = rootRT.rect;

        minX = minX - canvas.xMin - padding;
        minY = minY - canvas.yMin - padding;
        maxX = maxX - canvas.xMin + padding;
        maxY = maxY - canvas.yMin + padding;

        SetSpotlightDesdeRect(
            minX,
            minY,
            maxX,
            maxY,
            canvas.width,
            canvas.height
        );
    }

    // =========================================================
    // CONSTRUCCIÓN DEL SPOTLIGHT
    // =========================================================

    private void SetSpotlightDesdeRect(
        float minX,
        float minY,
        float maxX,
        float maxY,
        float w,
        float h)
    {
        SetPanel(
            panelArriba,
            0,
            maxY,
            w,
            h - maxY
        );

        SetPanel(
            panelAbajo,
            0,
            0,
            w,
            minY
        );

        SetPanel(
            panelIzquierda,
            0,
            minY,
            minX,
            maxY - minY
        );

        SetPanel(
            panelDerecha,
            maxX,
            minY,
            w - maxX,
            maxY - minY
        );
    }

    // =========================================================
    // PANEL
    // =========================================================

    private void SetPanel(
        RectTransform panel,
        float x,
        float y,
        float w,
        float h)
    {
        panel.anchorMin = Vector2.zero;
        panel.anchorMax = Vector2.zero;
        panel.pivot = Vector2.zero;

        panel.anchoredPosition =
            new Vector2(x, y);

        panel.sizeDelta =
            new Vector2(w, h);
    }

    // =========================================================
    // BOTÓN SIGUIENTE
    // =========================================================

    public void OnClickSiguiente()
    {
        TutorialManager.Instancia.AvanzarPaso();
    }
}