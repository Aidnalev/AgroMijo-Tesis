using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Coloca este script en un GameObject hijo del Canvas principal.
// Necesita 4 paneles Image oscuros (hijos de este objeto) y un panel de mensaje.
//
// Jerarquía sugerida en Unity:
// Canvas
//   TutorialOverlay  ← este script aquí
//     PanelArriba    ← Image negro, Alpha 180
//     PanelAbajo     ← Image negro, Alpha 180
//     PanelIzquierda ← Image negro, Alpha 180
//     PanelDerecha   ← Image negro, Alpha 180
//     PanelMensaje   ← panel blanco con Txt_Mensaje y Btn_Siguiente
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
    public TMP_Text   textoMensaje;
    public Button     botonSiguiente;

    [Header("Ajuste")]
    [Tooltip("Espacio extra alrededor del elemento destacado")]
    public float padding = 8f;

    private RectTransform rootRT;

    private void Awake()
    {
        Instancia = this;
        rootRT = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        Ocultar();
    }

    public void MostrarPaso(string mensaje, RectTransform objetivo, bool mostrarBoton)
    {
        gameObject.SetActive(true);
        textoMensaje.text = mensaje;
        botonSiguiente.gameObject.SetActive(mostrarBoton);
        panelMensaje.SetActive(true);

        if (objetivo == null)
            SetFullOverlay();
        else
            SetSpotlight(objetivo);
    }

    public void Ocultar()
    {
        SetPanel(panelArriba,    0, 0, 0, 0);
        SetPanel(panelAbajo,     0, 0, 0, 0);
        SetPanel(panelIzquierda, 0, 0, 0, 0);
        SetPanel(panelDerecha,   0, 0, 0, 0);
        panelMensaje.SetActive(false);
        gameObject.SetActive(false);
    }

    // Oscurece toda la pantalla (para pasos de bienvenida o explicación general)
    private void SetFullOverlay()
    {
        Rect r = rootRT.rect;
        SetPanel(panelArriba,    0, 0, r.width, r.height);
        SetPanel(panelAbajo,     0, 0, 0, 0);
        SetPanel(panelIzquierda, 0, 0, 0, 0);
        SetPanel(panelDerecha,   0, 0, 0, 0);
    }

    // Posiciona los 4 paneles alrededor del objetivo, dejando un "hueco" encima de él
    private void SetSpotlight(RectTransform objetivo)
    {
        Vector3[] corners = new Vector3[4];
        objetivo.GetWorldCorners(corners);

        // Convertir esquinas del mundo al espacio local del canvas raíz
        Rect canvas = rootRT.rect;
        Vector2 bl = rootRT.InverseTransformPoint(corners[0]); // bottom-left
        Vector2 tr = rootRT.InverseTransformPoint(corners[2]); // top-right

        // Convertir de coordenadas centradas en 0 a coordenadas desde la esquina inferior-izquierda
        float minX = bl.x - canvas.xMin - padding;
        float minY = bl.y - canvas.yMin - padding;
        float maxX = tr.x - canvas.xMin + padding;
        float maxY = tr.y - canvas.yMin + padding;
        float w    = canvas.width;
        float h    = canvas.height;

        SetPanel(panelArriba,    0,    maxY, w,        h - maxY);
        SetPanel(panelAbajo,     0,    0,    w,        minY);
        SetPanel(panelIzquierda, 0,    minY, minX,     maxY - minY);
        SetPanel(panelDerecha,   maxX, minY, w - maxX, maxY - minY);
    }

    private void SetPanel(RectTransform panel, float x, float y, float w, float h)
    {
        panel.anchorMin        = Vector2.zero;
        panel.anchorMax        = Vector2.zero;
        panel.pivot            = Vector2.zero;
        panel.anchoredPosition = new Vector2(x, y);
        panel.sizeDelta        = new Vector2(w, h);
    }

    // Conecta al botón "Siguiente" del panel de mensaje
    public void OnClickSiguiente()
    {
        TutorialManager.Instancia.AvanzarPaso();
    }
}
